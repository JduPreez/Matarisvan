module  Matarisvan.Logs

open FSharp.Data
open System
open Matarisvan.Performance

type LogItem = CsvProvider<"C:\Projects\Matarisvan\Matarisvan\AppLogs.csv", Schema=",date option,,,,,,,,,,,">

let load (file : string)  : LogItem.Row seq = 
  let log = LogItem.Load(file)
  log.Rows

// TODO: Calculate responseTime set of LogItems grouped by CorrelationID
let timespan (logItems : LogItem.Row seq) =
  let begins = Seq.minBy<LogItem.Row, DateTime option> (fun li -> li.Date) logItems
  let ends = Seq.maxBy<LogItem.Row, DateTime option> (fun li -> li.Date) logItems
  Seq.head logItems, (ends.Date.Value - begins.Date.Value).Milliseconds

let convert (arg: (LogItem.Row * int)) =
  let logItem, responseTime = arg
  let callingMethod = logItem.CallingMethod
  performanceItem logItem.Date logItem.Level callingMethod responseTime 0 // date logLevel resource responseTime responseLength

let getBy pred src = Seq.filter<LogItem.Row> pred src
                      |> Seq.groupBy  (fun li       -> li.CorrelationId)
                      |> Seq.map      (fun li       -> snd li
                                                        |> Seq.toList
                                                        |> timespan
                                                        |> convert)
                      |> Seq.filter   (fun perfItem -> perfItem.ResponseTime > 100) // Only those taking longer than 1 second

let getByCallingMethod logFile (callingMethod: string) =
  getBy (fun li -> li.CallingMethod = callingMethod) (load logFile)

// Function "get" will draw a chart for each CallingMethod. So 1st group by CallingMethod,
// and then group by correlation ID (almost like a nested "getByCallingMethod")
// TODO: Test this!!!
let get logFile =
  load logFile
  |> Seq.groupBy  (fun li -> li.CallingMethod)
  |> Seq.map      (fun li -> fst li, (snd li
                                       |> Seq.groupBy  (fun li       -> li.CorrelationId)
                                       |> Seq.map      (fun (_, logItms) ->  logItms
                                                                             |> Seq.toList
                                                                             |> timespan
                                                                             |> convert)))
  |> Seq.map      (fun (cm, perfItems) -> cm,  perfItems
                                               |> Seq.filter (fun pi -> pi.ResponseTime > 100))
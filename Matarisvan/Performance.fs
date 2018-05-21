module Matarisvan.Performance

open System
open FSharp.Data

type PerformanceItem = CsvProvider<"C:\Projects\Matarisvan\Matarisvan\PerformanceItem.csv", " | ", Schema="date option,,,,,,,">

let performanceItem date logLevel resource responseTime responseLength : PerformanceItem.Row = 
    PerformanceItem.Row(date, null, null, null, null, null, logLevel, null, null, null, null, null, resource, null, null, responseTime, null, null, responseLength)

module Logs =
  
  [<Literal>]
  let modelTemplate = "C:\Projects\Matarisvan\Matarisvan\AppLogs.csv"
  
  type LogItem = CsvProvider<modelTemplate, Schema=",date option,,,,,,,,,,,">
  
  let fromFile (file: string) =
    let f () : LogItem.Row seq =
      let log = LogItem.Load(file)
      log.Rows
    f
  
  let timespan (logItems : LogItem.Row seq) =
    let begins = Seq.minBy<LogItem.Row, DateTime option> (fun li -> li.Date) logItems
    let ends = Seq.maxBy<LogItem.Row, DateTime option> (fun li -> li.Date) logItems
    Seq.head logItems, (ends.Date.Value - begins.Date.Value).TotalMilliseconds |> int
  
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
  
  let getByCallingMethod (fromSource: unit -> seq<LogItem.Row>) (callingMethod: string) =
    getBy (fun li -> li.CallingMethod = callingMethod) (fromSource())
  
  // Function "get" will draw a chart for each CallingMethod. So 1st group by CallingMethod,
  // and then group by correlation ID (almost like a nested "getByCallingMethod")
  // TODO: Test drawing graphs with this.
  let get (fromSource: unit -> seq<LogItem.Row>) =
    fromSource()
    |> Seq.groupBy  (fun li -> li.CallingMethod)
    |> Seq.map      (fun li -> fst li, (snd li
                                         |> Seq.groupBy  (fun li       -> li.CorrelationId)
                                         |> Seq.map      (fun (_, logItms) ->  logItms
                                                                               |> Seq.toList
                                                                               |> timespan
                                                                               |> convert)))
    |> Seq.map      (fun (cm, perfItems) -> cm,  perfItems
                                                 |> Seq.filter (fun pi -> pi.ResponseTime > 100)
                                                 |> Seq.toList)
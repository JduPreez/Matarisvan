

open System
open FSharp.Data
open Matarisvan.Domain



type PerformanceItems = CsvProvider<"C:\Projects\Matarisvan\Matarisvan\PerformanceItem.csv", " | ", Schema="date option,,,,,,,">

let performanceItem time logLevel resource responseTime responseLength = {  time = time
                                                                            logLevel = logLevel
                                                                            resource = resource
                                                                            responseTime = responseTime
                                                                            responseLength = responseLength }

    //PerformanceItems.Row(date, null, null, null, null, null, logLevel, null, null, null, null, null, resource, null, null, responseTime, null, null, responseLength)

module Logs =
  
  [<Literal>]
  let modelTemplate = "C:\Projects\Matarisvan\Matarisvan\AppLogs.csv"
  
  type LogItems = CsvProvider<modelTemplate, Schema=",date option,,,,,,,,,,,">
  
  let fromFile (file: string) =
    let f () =
      let log = LogItems.Load(file)
      seq { for i in log.Rows -> {  time = i.Date
                                    callingMethod = i.CallingMethod
                                    correlationId = i.CorrelationId
                                    logLevel = i.Level } }
    f
  
  let timespan (logItems : LogItem seq) =
    let begins = Seq.minBy<LogItem, DateTime option> (fun li -> li.time) logItems
    let ends = Seq.maxBy<LogItem, DateTime option> (fun li -> li.time) logItems
    Seq.head logItems, (ends.time.Value - begins.time.Value).TotalMilliseconds |> int
  
  let convert (arg: (LogItem * int)) =
    let logItem, responseTime = arg
    let callingMethod = logItem.callingMethod
    performanceItem logItem.time logItem.logLevel callingMethod responseTime 0 // date logLevel resource responseTime responseLength
  
  let getBy pred src = Seq.filter<LogItem> pred src
                        |> Seq.groupBy  (fun li       -> li.correlationId)
                        |> Seq.map      (fun li       -> snd li
                                                          |> Seq.toList
                                                          |> timespan
                                                          |> convert)
                        |> Seq.filter   (fun perfItem -> perfItem.responseTime > 100) // Only those taking longer than 1 second
  
  let getByCallingMethod (fromSource: unit -> LogItem seq) (callingMethod: string) =
    getBy (fun li -> li.callingMethod = callingMethod) (fromSource())
  
  // Function "get" will draw a chart for each CallingMethod. So 1st group by CallingMethod,
  // and then group by correlation ID (almost like a nested "getByCallingMethod")
  // TODO: Test drawing graphs with this.
  let get (fromSource: unit -> LogItem seq) =
    fromSource()
    |> Seq.groupBy  (fun li -> li.callingMethod)
    |> Seq.map      (fun li -> fst li, (snd li
                                         |> Seq.groupBy  (fun li       -> li.correlationId)
                                         |> Seq.map      (fun (_, logItms) ->  logItms
                                                                               |> Seq.toList
                                                                               |> timespan
                                                                               |> convert)))
    |> Seq.map      (fun (cm, perfItems) -> cm,  perfItems
                                                 |> Seq.filter (fun pi -> pi.responseTime > 100)
                                                 |> Seq.toList)
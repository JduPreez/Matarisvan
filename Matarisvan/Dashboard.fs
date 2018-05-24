module Matarisvan.Dashboard

open System
open FSharp.Charting
open FSharp.Data
open FSharp.Charting.ChartTypes
open Matarisvan.Performance
open Matarisvan.Performance.Logs
open Matarisvan.Domain

let view (performanceItems: PerformanceItem seq) =
     lazy (performanceItems
            |> Seq.filter (fun x -> not (String.IsNullOrEmpty(x.resource)))
            |> Seq.toList
            |> List.groupBy<PerformanceItem, string> (fun i -> i.resource)            
            |> List.map<string * PerformanceItem list, ChartTypes.GenericChart> (fun i -> (List.map<PerformanceItem, string * int> (fun x -> 
                                                                                                                                                printfn "%O: %i" x.time.Value x.responseTime
                                                                                                                                                x.time.Value.ToString(), x.responseTime/100) (snd i))
                                                                                                |> Seq.toList
                                                                                                |> Chart.Column)
            |> Chart.Combine
            |> Chart.WithXAxis (LabelStyle = ChartTypes.LabelStyle(Angle = -45, Interval = 1.0))).ShowChart()

// TODO: Changes were made to getByCallingMethod, make sure it & everything else is still working
let logFile = "C:\Projects\Matarisvan\Matarisvan\AppLogs.csv"

let fromLogs =
    getByCallingMethod(fromFile logFile)
    >> view

let fromPerformance = fromLogs "blah"
module Matarisvan.Dashboard

open System
open FSharp.Charting
open FSharp.Data
open FSharp.Charting.ChartTypes
open Matarisvan.Performance
open Matarisvan.Performance.Logs

let view (performanceItems: PerformanceItem.Row seq) =
     lazy (performanceItems
            |> Seq.filter (fun x -> not (String.IsNullOrEmpty(x.Resource)))
            |> Seq.toList
            |> List.groupBy<PerformanceItem.Row, string> (fun i -> i.Resource)            
            |> List.map<string * PerformanceItem.Row list, ChartTypes.GenericChart> (fun i -> (List.map<PerformanceItem.Row, string * int> (fun x -> 
                                                                                                                                                printfn "%O: %i" x.Time.Value x.ResponseTime
                                                                                                                                                x.Time.Value.ToString(), x.ResponseTime/100) (snd i))
                                                                                                |> Seq.toList
                                                                                                |> Chart.Column)
            |> Chart.Combine
            |> Chart.WithXAxis (LabelStyle = ChartTypes.LabelStyle(Angle = -45, Interval = 1.0))).ShowChart()

// TODO: Changes were made to getByCallingMethod, make sure it & everything else is still working
let logFile = "C:\Projects\Matarisvan\Matarisvan\AppLogs.csv"

let fromLogs =
    (getByCallingMethod logFile)
    >> view

let fromPerformance = fromLogs "blah"
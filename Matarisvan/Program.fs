// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System
open FSharp.Charting
open FSharp.Data
open FSharp.Charting.ChartTypes

open Matarisvan.Performance
open Matarisvan

[<EntryPoint>]
let main argv = 
    (Dashboard.fromLogs "Matarisvan.Application.Services.WebApi.NonGlobalCompaniesController.GetByClient").Force()
    |> Windows.Forms.Application.Run

    (* Working code, can be uncommented
    let performanceItems = PerformanceItem.Load("C:\Projects\Matarisvan\performance_stats0.csv")

    performanceItems.Rows
    |> Seq.toList
    |> List.groupBy<PerformanceItem.Row, string> (fun i -> i.Resource)
    |> List.map<string * PerformanceItem.Row list, ChartTypes.GenericChart> (fun i -> (List.map<PerformanceItem.Row, string * int> (fun x -> x.Time.Value.ToString(), x.ResponseTime/100 ) (snd i)) 
                                                                                        |> Seq.toList
                                                                                        |> Chart.Column)
    |> Chart.Combine
    |> Chart.Show *)

    //Windows.Forms.Application.Run()
    //System.Console.ReadLine() |> ignore
    0 // return an integer exit code

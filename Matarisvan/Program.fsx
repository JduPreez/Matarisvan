#r "packages/FSharp.Data.2.3.3/lib/net40/FSharp.Data.dll"
#load "FSharp.Charting.fsx"

open FSharp.Data
open FSharp.Charting
open System

type PerformanceStats = CsvProvider<"PerformanceStats.csv">


let performanceStats = PerformanceStats.Load("http://localhost:8089/stats/requests/csv")

for performanceStat in performanceStats.Rows do    
    printfn "Row: %s" (performanceStat.ToString())
    printfn "Name: %s" performanceStat.Name
    printfn "Median response time: %O" performanceStat.AverageContentSize

#r "packages/FSharp.Data.2.3.3/lib/net40/FSharp.Data.dll"
#load "FSharp.Charting.fsx"

open FSharp.Data
open FSharp.Charting
open FSharp.Charting.ChartTypes
open System

type PerformanceItem = CsvProvider<"PerformanceItem.csv", " | ", Schema="date option,,,,,,,">

let performanceItems = PerformanceItem.Load("C:\Projects\Matarisvan\performance_stats0.csv")

performanceItems.Rows
|> Seq.toList
|> List.groupBy<PerformanceItem.Row, string> (fun i -> i.Resource)
|> List.map<string * PerformanceItem.Row list, ChartTypes.GenericChart> (fun i -> (List.map<PerformanceItem.Row, string * int> (fun x -> x.Time.Value.ToString(), x.ResponseTime/100 ) (snd i)) 
                                                                                    |> Seq.toList
                                                                                    |> Chart.Column)
|> Chart.Combine
//|> ignore

// Print the prices in the HLOC format
//let chart = [ for row in msft.Rows -> row.Date, row.Open ]
//            |> Chart.FastLine

//let chart = Chart.Line [ for x in 1.0 .. 100.0 -> (x, x ** 2.0) ]
//Windows.Forms.Application.Run()

//

//#r "packages/FSharp.Charting.0.90.14/lib/net40/Fsharp.Charting.dll"

//

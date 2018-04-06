module Matarisvan.Performance

open System
open FSharp.Data

type PerformanceItem = CsvProvider<"C:\Projects\Matarisvan\Matarisvan\PerformanceItem.csv", " | ", Schema="date option,,,,,,,">

let performanceItem date logLevel resource responseTime responseLength : PerformanceItem.Row = 
    PerformanceItem.Row(date, null, null, null, null, null, logLevel, null, null, null, null, null, resource, null, null, responseTime, null, null, responseLength)
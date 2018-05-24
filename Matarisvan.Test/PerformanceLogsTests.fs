module Matarisvan.Test.PerformanceLogsTests

open Matarisvan.Performance.Logs
open Matarisvan.Domain
open Xunit
open System

let random = new Random()

let callingMethods = 10
let correlationIDs = 10

let spawn callingMethod item = {  time = Some (DateTime.Now.AddSeconds(float(random.Next(3, 100))))
                                  correlationId = sprintf "%i-%i" callingMethod item
                                  callingMethod = sprintf "%i" callingMethod
                                  logLevel = sprintf "%i" (random.Next()) }

let fromMocked () = seq {
                      for a in 0 .. callingMethods-1 do
                        for b in 0 .. correlationIDs-1 -> spawn a b
                        for c in 0 .. correlationIDs-1 -> spawn a c
                        for d in 0 .. correlationIDs-1 -> spawn a d
                    }

[<Fact>]
let ``Logs.get should get log entries`` () =
  let logs = get <| fromFile modelTemplate
  Assert.NotEmpty(logs)

[<Fact>]
let ``Logs.get should group PerformanceItems by calling method`` () =
  let logs = get <| fromMocked
  Assert.NotEmpty(logs)
  Assert.Equal(callingMethods, Seq.length logs)
  Assert.Equal(correlationIDs, Seq.length (snd (Seq.head logs)))
module Matarisvan.Test.PerformanceLogsTests

open Matarisvan.Performance.Logs
open Xunit
open System

let random = new Random()

let callingMethods = 10
let correlationIDs = 10

let spawn callingMethod item = LogItem.Row(random.Next(),                                               // id
                                            Some (DateTime.Now.AddSeconds(float(random.Next(3, 100)))), // date
                                            sprintf "%i" (random.Next()),                               // thread
                                            sprintf "%i" (random.Next()),                               // level
                                            sprintf "%i" (random.Next()),                               // logger
                                            sprintf "%i" (random.Next()),                               // message
                                            sprintf "%i" (random.Next()),                               // exception
                                            sprintf "%i" (random.Next()),                               // application
                                            sprintf "%i-%i" callingMethod item,                         // correlationId
                                            sprintf "%i" callingMethod,                                 // callingMethod
                                            sprintf "%i" (random.Next()),                               // user
                                            decimal(random.NextDouble()),                               // percentProcessorTime
                                            random.Next())                                              // availableMemoryMb

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
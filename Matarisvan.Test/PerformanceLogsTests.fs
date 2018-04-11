module Matarisvan.Test.PerformanceLogsTests

open Matarisvan.Performance
open Xunit

[<Fact>]
let ``Logs.get should get log entries`` () =
  let logs = Logs.get Logs.modelTemplate
  Assert.NotEmpty(logs)
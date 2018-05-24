module Matarisvan.AppServices

open Matarisvan.Domain
                                   // isLog -> file-path
type IServer = { analysePerformance : bool -> string -> Async<Chart list> }
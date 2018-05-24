module Matarisvan.Domain

open System

type Chart = {
  title: string
  url: string
  fileType: string
}

type PerformanceItem = {
  time: DateTime option
  logLevel: string
  resource: string
  responseTime: int
  responseLength: int
}

type LogItem = {
  time: DateTime option
  callingMethod: string
  correlationId: string
  logLevel: string
}


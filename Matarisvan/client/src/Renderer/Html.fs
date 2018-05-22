module Html

open System
open Fable.Core
open Fable.Import

// In this module you can find most of the HTML generation used by the sample
// We added it in a separate file because it's not the important part of the sample
let replaceChildren (root: Browser.HTMLElement) children =
    while not (isNull root.firstChild) do
        root.removeChild(root.firstChild)
        |> ignore

    for child in children do
        root.appendChild(child)
        |> ignore

let createIcon faIcon =
    let root = Browser.document.createElement_span()
    let icon = Browser.document.createElement_i()

    root.className <- "icon"
    icon.className <- "fa " + faIcon
    root.appendChild(icon) |> ignore
    root

let createLink filename =
    let root = Browser.document.createElement_a()
    root.href <- "#"
    root.innerText <- filename
    root

let createImage (path: string) filename =
    // (path.Replace("/", "\\"))
    let imageUrl = path //sprintf "%s/%s" path filename
    let root = Browser.document.createElement_img()
    root.src <- imageUrl
    root
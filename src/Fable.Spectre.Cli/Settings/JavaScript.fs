module Fable.Spectre.Cli.Settings.JavaScript


open System
open System.ComponentModel
open Fable
open Fable.Spectre.Cli.Settings.CommonCompile
open Fable.Spectre.Cli.Settings.Spec
open Spectre.Console.Cli

type JavaScriptSettings() =
    inherit CommonCompileSettings(JavaScript)

    [<Description("[dim]Runs the generated script for last file with node.[/]\
[grey](Requires `\"type\": \"module\"` in package.json and at minimum Node.js 12.20, 14.14, or 16.0.0)[/]")>]
    [<CommandOption("--runScript")>]
    member val runScript = "" with get, set

    [<Description("[dim]Compile numeric arrays as JS typed arrays [/][grey](default is true for JS, false for TS)[/]")>]
    [<CommandOption("--typedArrays")>]
    member val typedArrays = false with get, set

    [<Description("[dim]Enable source maps.[/]")>]
    [<CommandOption("-s|--sourceMaps")>]
    member val sourceMaps = false with get, set

    [<Description("[dim]Set the value of the `sourceRoot` property in generated source map files.[/]")>]
    [<CommandOption("--sourceMapsRoot <VALUE>")>]
    member val sourceMapsRoot = "" with get, set

    interface IJavaScriptArgs with
        member this.runScript =
            if this.runScript |> String.IsNullOrWhiteSpace then
                None
            else
                Some this.runScript

        member this.sourceMap = this.sourceMaps

        member this.sourceMapRoot =
            if this.sourceMapsRoot |> String.IsNullOrWhiteSpace then
                None
            else
                Some this.sourceMapsRoot

        member this.typedArrays = this.typedArrays

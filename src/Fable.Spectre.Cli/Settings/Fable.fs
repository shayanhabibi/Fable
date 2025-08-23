module Fable.Spectre.Cli.Settings.Fable

open Fable.Spectre.Cli.Settings.JavaScript
open Fable.Spectre.Cli.Settings.Spec

type FableSettings() =
    inherit JavaScriptSettings()
    interface ICliArgs

module Fable.Spectre.Cli.Commands.Watch

open Fable.Spectre.Cli.Settings.Fable
open Spectre.Console.Cli

type WatchCommand() =
    inherit Command<FableSettings>()
    override this.Execute(context, settings) = 0

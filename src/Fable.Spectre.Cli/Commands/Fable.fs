module Fable.Spectre.Cli.Commands.Fable

open Fable.Spectre.Cli.Settings.Fable
open Spectre.Console.Cli

type FableCommand() =
    inherit Command<FableSettings>()
    override this.Execute(context, settings) = 0

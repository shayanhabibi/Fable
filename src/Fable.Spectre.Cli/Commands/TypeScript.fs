module Fable.Spectre.Cli.Commands.TypeScript

open Fable.Spectre.Cli.Settings.TypeScript
open Spectre.Console.Cli

type TypeScriptCommand() =
    inherit Command<TypeScriptSettings>()
    override this.Execute(context, settings) = 0

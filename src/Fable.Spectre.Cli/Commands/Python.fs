module Fable.Spectre.Cli.Commands.Python

open Fable.Spectre.Cli.Settings.Python
open Spectre.Console.Cli

type PythonCommand() =
    inherit Command<PythonSettings>()
    override this.Execute(context, settings) = 0

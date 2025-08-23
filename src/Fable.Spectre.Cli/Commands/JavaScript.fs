module Fable.Spectre.Cli.Commands.JavaScript

open Fable.Spectre.Cli.Settings.JavaScript
open Spectre.Console.Cli

type JavaScriptCommand() =
    inherit Command<JavaScriptSettings>()
    override this.Execute(context: CommandContext, settings: JavaScriptSettings) : int = 0

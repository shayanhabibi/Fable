module Fable.Spectre.Cli.Commands.Dart

open Fable.Spectre.Cli.Settings.Dart
open Spectre.Console.Cli

type DartCommand() =
    inherit Command<DartSettings>()
    override this.Execute(context, settings) = 0

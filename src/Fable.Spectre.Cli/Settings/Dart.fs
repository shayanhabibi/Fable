module Fable.Spectre.Cli.Settings.Dart

open Fable
open Fable.Spectre.Cli.Settings.CommonCompile
open Fable.Spectre.Cli.Settings.Spec

type DartSettings() =
    inherit CommonCompileSettings(Dart)
    interface IDartArgs

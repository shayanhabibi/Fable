module Fable.Spectre.Cli.Settings.Python

open Fable
open Fable.Spectre.Cli.Settings.CommonCompile
open Fable.Spectre.Cli.Settings.Spec

type PythonSettings() =
    inherit CommonCompileSettings(Python)
    interface IPythonArgs

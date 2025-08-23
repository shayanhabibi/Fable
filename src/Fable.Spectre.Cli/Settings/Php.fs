module Fable.Spectre.Cli.Settings.Php

open Fable
open Fable.Spectre.Cli.Settings.CommonCompile
open Fable.Spectre.Cli.Settings.Spec

type PhpSettings() =
    inherit CommonCompileSettings(Php)
    interface IPhpArgs

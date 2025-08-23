module Fable.Spectre.Cli.Settings.Rust

open Fable
open Fable.Spectre.Cli.Settings.CommonCompile
open Fable.Spectre.Cli.Settings.Spec

type RustSettings() =
    inherit CommonCompileSettings(Rust)
    interface IRustArgs

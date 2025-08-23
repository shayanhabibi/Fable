module Fable.Spectre.Cli.Commands.Rust

open Fable.Spectre.Cli.Settings.Rust
open Spectre.Console.Cli

type RustCommand() =
    inherit Command<RustSettings>()
    override this.Execute(context, settings) = 0

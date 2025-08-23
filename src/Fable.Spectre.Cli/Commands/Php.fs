module Fable.Spectre.Cli.Commands.Php

open Fable.Spectre.Cli.Settings.Php
open Spectre.Console.Cli

type PhpCommand() =
    inherit Command<PhpSettings>()
    override this.Execute(context, settings) = 0

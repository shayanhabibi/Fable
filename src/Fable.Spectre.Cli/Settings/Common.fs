module Fable.Spectre.Cli.Settings.Common

open System.ComponentModel
open Fable.Spectre.Cli.Settings.Spec
open Spectre.Console
open Spectre.Console.Cli
open System
open Fable.Compiler.Util
open Fable

type CommonSettings(?language: Language) =
    inherit CommandSettings()

    [<Description """[dim]Choose which language to compile to.[/]

[dim]Available options:[/]
[dim] - javascript | js (default)    [/][grey](default ext .fs.js)[/]
[dim] - typescript | ts              [/][grey](default ext .fs.ts)[/]
[dim] - python | py                  [/][grey](default ext .py)[/]
[dim] - rust | rs                    [/][grey](default ext .rs)[/]
[dim] - php                          [/][grey](default ext .php)[/]
[dim] - dart                         [/][grey](default ext .dart)[/]""">]
    [<CommandOption("-l|--language <LANGUAGE>")>]
    member val langString = "javascript" with get, set

    [<Description "[dim]Print the version.[/]">]
    [<CommandOption("--version")>]
    member val version = false with get, set

    [<Description "[dim]Set the working directory.[/]">]
    [<CommandOption("--cwd <PATH>")>]
    member val cwd = Environment.CurrentDirectory with get, set

    member val verbosity = Verbosity.Normal with get, set

    [<Description "[dim]Set the amount of information printed.[/] [grey][[v|verbose]] or [[n|normal]] or [[s|silent]][/]">]
    [<CommandOption("--verbosity <STRING>")>]
    member val verbosityString = "normal" with get, set

    // Backwards compatibility
    [<CommandOption("--verbose", IsHidden = true)>]
    member val verbose = false with get, set

    // Backwards compatibility
    [<CommandOption("--silent", IsHidden = true)>]
    member val silent = false with get, set

    [<Description "[dim]Extension for generated files.[/]">]
    [<CommandOption("-e|--extension")>]
    member val extension = ".fs.js" with get, set

    member val language = language |> Option.defaultValue JavaScript with get, set

    [<Description "[dim]Automatically respond 'yes' to prompts.[/]">]
    [<CommandOption("--yes")>]
    member val yes: bool = false with get, set

    interface ICommonArgs with
        member this.extension =
            match this.language with
            | JavaScript -> this.extension
            | _ when this.extension <> ".fs.js" -> this.extension
            | TypeScript -> ".fs.ts"
            | Python -> ".py"
            | Php -> ".php"
            | Dart -> ".dart"
            | Rust -> ".rs"

        member this.language = this.language
        member this.silent = this.silent
        member this.uncaughtArgs = []
        member this.verbosity = this.verbosity
        member this.version = this.version
        member this.workingDirectory = this.cwd
        member this.yes = this.yes

    member this.NormalizeAbsolutePath(path: string) =
        (if IO.Path.IsPathRooted(path) then
             path
         else
             IO.Path.Combine(this.cwd, path))
        // Use getExactFullPath to remove things like: myrepo/./build/
        // and get proper casing (see `getExactFullPath` comment)
        |> File.getExactFullPath
        |> Path.normalizePath

    override this.Validate() =
        let validateCwd acc =
            if System.IO.Path.Exists(this.cwd) then
                this.cwd <- this.NormalizeAbsolutePath this.cwd
                acc
            else
                $"[--cwd] Working directory does not exist: {this.cwd}" :: acc

        let validateVerbosity acc =
            match this.verbosityString.ToLower() with
            | "normal" when this.verbose ->
                this.verbosity <- Verbosity.Verbose
                acc
            | "normal" when this.silent ->
                this.verbosity <- Verbosity.Silent
                acc
            | "n"
            | "normal" ->
                this.verbosity <- Verbosity.Normal
                acc
            | "v"
            | "verbose" ->
                this.verbosity <- Verbosity.Verbose
                acc
            | "s"
            | "silent" ->
                this.verbosity <- Verbosity.Silent
                acc
            | value ->
                $"[--verbosity] Verbosity can be one of [verbose|silent|normal], but got '{value}'"
                :: acc

        let validateLanguage acc =
            match this.langString.ToLower() with
            | "js"
            | "javascript" ->
                this.language <- JavaScript
                acc
            | "py"
            | "python" ->
                this.language <- Python
                acc
            | "rs"
            | "rust" ->
                this.language <- Rust
                acc
            | "ts"
            | "typescript" ->
                this.language <- TypeScript
                acc
            | "php" ->
                this.language <- Php
                acc
            | "dart" ->
                this.language <- Dart
                acc
            | value ->
                $"[-lang|--language] Unknown language target: '{value}'

    Available Options:
        - js | javascript
        - ts | typescript
        - py | python
        - rs | rust
        - php
        - dart"
                :: acc

        let validation = [] |> validateCwd |> validateLanguage |> validateVerbosity

        if validation.IsEmpty then
            base.Validate()
        else
            ValidationResult.Error(validation |> String.concat "\n")

module Fable.Spectre.Cli.Commands.Clean

open System
open System.ComponentModel
open Fable
open Fable.Spectre.Cli.Commands.CommonRunner
open Fable.Spectre.Cli.Settings.Common
open Spectre.Console
open Spectre.Console.Cli
open SpectreCoff

type CleanSettings() =
    inherit CommonSettings(JavaScript)
    let mutable validatedLang = Language.JavaScript
    let mutable lang = "javascript"

    [<CommandArgument(0, "[Language]")>]
    member this.language
        with get () = lang
        and set (value: string) =
            lang <- value.ToLower()

            match value.ToLower() with
            | "js"
            | "javascript" -> validatedLang <- Language.JavaScript
            | "rs"
            | "rust" -> validatedLang <- Language.Rust
            | "py"
            | "python" -> validatedLang <- Language.Python
            | "ts"
            | "typescript" -> validatedLang <- Language.TypeScript
            | "php" -> validatedLang <- Language.Php
            | "dart" -> validatedLang <- Language.Dart
            | _ -> ()

    [<Description("Path to working directory.")>]
    [<CommandArgument(1, "[PATH]")>]
    member this.cwd
        with get () = base.cwd
        and set (value: string) = base.cwd <- value

    member this.validatedLanguage = validatedLang

    override this.Validate() =
        let maybeLanguageValidationError =
            match this.language.ToLower() with
            | "js"
            | "javascript"
            | "rs"
            | "rust"
            | "py"
            | "python"
            | "ts"
            | "typescript"
            | "php"
            | "dart" -> None
            | _ ->
                ValidationResult.Error(
                    $"""'{this.language}' is not a recognized language with a known extension.
    Either choose a known language, or use the --extension option.

    Known languages:
     - js | javascript (.fs.js)
     - ts | typescript (.fs.ts)
     - py | python (.py)
     - rs | rust (.rs)
     - dart (.dart)
     - php (.php)"""
                )
                |> Some

        if maybeLanguageValidationError.IsSome then
            maybeLanguageValidationError.Value
        else
            base.Validate()

type CleanCommand() =
    inherit Command<CleanSettings>()
    interface ICommandLimiter<CommonSettings>

    override this.Execute(_context, _settings) =
        let haveDefaultExtension = _settings.extension = ".fs.js"

        match _settings.validatedLanguage with
        | _ when not haveDefaultExtension -> clean _settings
        | JavaScript -> clean _settings
        | TypeScript ->
            _settings.extension <- ".fs.ts"
            clean _settings
        | Python ->
            _settings.extension <- ".py"
            clean _settings
        | Php ->
            _settings.extension <- ".php"
            clean _settings
        | Dart ->
            _settings.extension <- ".dart"
            clean _settings
        | Rust ->
            _settings.extension <- ".rust"
            clean _settings

        0

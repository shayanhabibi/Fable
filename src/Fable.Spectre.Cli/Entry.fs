open Fable
open Fable.Spectre.Cli.Commands.Clean
open Fable.Spectre.Cli.Commands.CommonRunner
open Fable.Spectre.Cli.Commands.Dart
open Fable.Spectre.Cli.Commands.Fable
open Fable.Spectre.Cli.Commands.JavaScript
open Fable.Spectre.Cli.Commands.Php
open Fable.Spectre.Cli.Commands.Python
open Fable.Spectre.Cli.Commands.Rust
open Fable.Spectre.Cli.Commands.TypeScript
open Fable.Spectre.Cli.Commands.Watch
open Fable.Spectre.Cli.Settings.CommonCompile
open Fable.Spectre.Cli.Settings.Common
open Fable.Spectre.Cli.Settings.Dart
open Fable.Spectre.Cli.Settings.JavaScript
open Fable.Spectre.Cli.Settings.Php
open Fable.Spectre.Cli.Settings.Python
open Fable.Spectre.Cli.Settings.Rust
open Fable.Spectre.Cli.Settings.TypeScript
open Fable.Spectre.Cli.SpectreOutput
open Spectre.Console
open Spectre.Console.Cli
open SpectreCoff

[<EntryPoint>]
let main argv =
    let app = CommandApp()

    app.Configure(fun config ->
        config.Settings.ShowOptionDefaultValues <- false
        config.Settings.HelpProviderStyles.Options.RequiredOption <- Style(foreground = Color.Blue)

        config.AddBranch(
            "python",
            (fun (branchConfig: IConfigurator<PythonSettings>) ->
                branchConfig.SetDefaultCommand<PythonCommand>()
                branchConfig.SetDescription(dim "Fable for python.")
            )
        )
        |> ignore

        config.AddBranch(
            "javascript",
            (fun (branchConfig: IConfigurator<JavaScriptSettings>) ->
                branchConfig.SetDefaultCommand<JavaScriptCommand>()
                branchConfig.SetDescription(dim "Fable for javascript.")
            )
        )
        |> ignore

        config.AddBranch(
            "rust",
            (fun (branchConfig: IConfigurator<RustSettings>) ->
                branchConfig.SetDefaultCommand<RustCommand>()
                branchConfig.SetDescription(dim "Fable for rust.")
            )
        )
        |> ignore

        config.AddBranch(
            "typescript",
            (fun (branchConfig: IConfigurator<TypeScriptSettings>) ->
                branchConfig.SetDefaultCommand<TypeScriptCommand>()
                branchConfig.SetDescription(dim "Fable for typescript.")
            )
        )
        |> ignore

        config.AddBranch(
            "php",
            (fun (branchConfig: IConfigurator<PhpSettings>) ->
                branchConfig.SetDefaultCommand<PhpCommand>()
                branchConfig.SetDescription(dim "Fable for php.")
            )
        )
        |> ignore

        config.AddBranch(
            "dart",
            (fun (branchConfig: IConfigurator<DartSettings>) ->
                branchConfig.SetDefaultCommand<DartCommand>()
                branchConfig.SetDescription(dim "Fable for dart.")
            )
        )
        |> ignore

        config
            .AddCommand<CleanCommand>("clean")
            .WithDescription(dim "Remove fable_modules folders and files with specified extension (default is .fs.js)")
        |> ignore

        config.AddCommand<WatchCommand>("watch").WithDescription(dim "Run fable in watch mode.")
        |> ignore
    )

    app
        .SetDefaultCommand<FableCommand>()
        .WithDescription(
            """[bold]Fable compiler[/].

All flags and options are available, with defaults set for [italic]JavaScript[/].
Language specific commands will set defaults for file extensions, and/or restrict/add the available flags for that language."""
        )
    |> ignore

    app.Run(argv)

module Fable.Spectre.Cli.Settings.CommonCompile

open System
open System.ComponentModel
open Fable.Spectre.Cli.Settings.Common
open Fable.Spectre.Cli.Settings.Spec
open Spectre.Console
open Spectre.Console.Cli

type CommonCompileSettings(defaultLanguage) =
    inherit CommonSettings(defaultLanguage)

    [<Description("[dim]Directory for generated files.[/]")>]
    [<CommandOption("-o|--output <DIRECTORY>")>]
    member val outputDir = "" with get, set

    [<Description("[dim]Defines a symbol for use in conditional compilation[/]")>]
    [<CommandOption("--define <SYMBOL>")>]
    member val symbols: string array = [||] with get, set

    [<Description("[dim]The configuration to use when parsing .fsproj with MsBuild, default is 'Debug' in watch mode, or 'Release' otherwise.[/]")>]
    [<CommandOption("-c|--configuration <MODE>")>]
    [<DefaultValue("Release")>]
    member val config = "Release" with get, set

    [<Description("[dim]Alias of watch command[/]")>]
    [<CommandOption("--watch")>]
    member val watch = false with get, set

    [<Description("[dim]Delay in ms before recompiling after a file changes[/] [grey](default 200)[/]")>]
    [<CommandOption("--watchDelay <INT>")>]
    [<DefaultValue 200>]
    member val watchDelay = 200 with get, set

    [<Description("[dim]A string containing a command after the argument that will be executed after compilation[/]")>]
    [<CommandOption("--run <COMMAND>")>]
    member val runCommand = "" with get, set

    [<Description("[dim]The command string after the argument will be executed BEFORE compilation.[/]")>]
    [<CommandOption("--runFast <COMMAND>")>]
    member val runCommandFast = "" with get, set

    [<Description("[dim]The command after the argument will be executed AFTER each watch compilation.[/]")>]
    [<CommandOption("--runWatch <COMMAND>")>]
    member val runCommandWatch = "" with get, set

    [<Description("[dim]Skip [italic]`dotnet restore`[/][/]")>]
    [<CommandOption("--noRestore")>]
    member val noRestore = false with get, set

    [<Description("[dim]Recompile all files, including sources from packages.[/]")>]
    [<CommandOption("--noCache")>]
    member val noCache = false with get, set

    [<Description("[dim]Don't merge sources of referenced projects with specified pattern (Intended for plugin development).[/]")>]
    [<CommandOption("--exclude <PATTERN>")>]
    member val excludePatterns: string array = [||] with get, set

    [<Description("[dim]Compile with optimized F# AST (experimental)[/]")>]
    [<CommandOption("--optimize")>]
    member val optimize = false with get, set

    [<Description("[dim]Use this if you have issues with the new MSBuild Cracker released in Fable 5.[/]")>]
    [<CommandOption("--legacyCracker")>]
    member val legacyCracker = false with get, set

    [<CommandOption("--precompiledLib <PATH>", IsHidden = true)>]
    member val precompiledLib: string = "" with get, set

    [<CommandOption("--printAst", IsHidden = true)>]
    member val printAst = false with get, set

    [<CommandOption("--noReflection", IsHidden = true)>]
    member val noReflection = false with get, set

    [<CommandOption("--noParallelTypeCheck", IsHidden = true)>]
    member val noParallelTypeCheck = false with get, set

    [<CommandOption("--trimRootModule", IsHidden = true)>]
    member val trimRootModule = false with get, set

    [<CommandOption("--fableLib <PATH>", IsHidden = true)>]
    member val fableLib = "" with get, set

    member val replace: Map<string, string> = Map([]) with get, set

    [<CommandOption("--replace <KEYVALUE>", IsHidden = true)>]
    member val replaceStrings: string[] = [||] with get, set

    interface ICompilingArgs with
        member this.configuration = this.config
        member this.exclude = this.excludePatterns

        member this.fableLib =
            if this.fableLib |> String.IsNullOrWhiteSpace then
                None
            else
                Some this.fableLib

        member this.legacyCracker = this.legacyCracker
        member this.noCache = this.noCache
        member this.noParallelTypeCheck = this.noParallelTypeCheck
        member this.noReflection = this.noReflection
        member this.noRestore = this.noRestore
        member this.optimize = this.optimize

        member this.precompiledLib =
            if this.precompiledLib |> String.IsNullOrWhiteSpace then
                None
            else
                Some this.precompiledLib

        member this.printAst = this.printAst
        member this.replace = this.replace

        member this.run =
            if this.runCommand |> String.IsNullOrWhiteSpace then
                None
            else
                Some this.runCommand

        member this.runFast =
            if this.runCommandFast |> String.IsNullOrWhiteSpace then
                None
            else
                Some this.runCommandFast

        member this.runWatch =
            if this.runCommandWatch |> String.IsNullOrWhiteSpace then
                None
            else
                Some this.runCommandWatch

        member this.definitions = this.symbols
        member this.trimRootModule = this.trimRootModule
        member this.watch = this.watch
        member this.watchDelay = this.watchDelay

        member this.outputDirectory =
            if this.outputDir |> String.IsNullOrWhiteSpace then
                None
            else
                Some this.outputDir

    override this.Validate() =
        let validateReplace acc =
            this.replaceStrings
            |> Array.filter (String.exists ((=) ':') >> not)
            |> function
                | [||] ->
                    this.replace <-
                        this.replaceStrings
                        |> Array.map (_.Split(':') >> fun arr -> arr[0], arr[1])
                        |> Map

                    acc
                | arr ->
                    arr
                    |> Array.map (sprintf "[--replace] Expected '<KEY>:<VALUE>' pair but got: %s")
                    |> Array.toList
                    |> (@) acc

        let validateConfiguration acc =
            match this.config with
            | "Debug"
            | "Release" -> acc
            | value -> $"[--configuration] Expected one of [ Debug | Release ] but got: {value}" :: acc

        let validation = [] |> validateReplace |> validateConfiguration

        if validation.IsEmpty then
            ValidationResult.Success()
        else
            ValidationResult.Error(validation |> String.concat "\n")

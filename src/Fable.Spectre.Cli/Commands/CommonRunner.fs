module Fable.Spectre.Cli.Commands.CommonRunner

open Fable.Spectre.Cli.Settings.Spec
open Fable.Spectre.Cli.SpectreOutput
open Spectre.Console
open SpectreCoff
open System
open Fable.Cli.Main
open Fable.Cli
open Fable
open Fable.Compiler.Util
open Fable.Cli.CustomLogging
open Fable.Spectre.Cli.Settings.Common
open Microsoft.Extensions.Logging
open Spectre.Console.Cli

// type Runner =
//     static member Run
//         (
//             args: CliArgs,
//             language: Language,
//             rootDir: string,
//             runProc: RunProcess option,
//             verbosity: Fable.Verbosity,
//             ?fsprojPath: string,
//             ?watch,
//             ?precompile
//         )
//         =
//         result {
//             let normalizeAbsolutePath (path: string) =
//                 (if IO.Path.IsPathRooted(path) then
//                      path
//                  else
//                      IO.Path.Combine(rootDir, path))
//                 // Use getExactFullPath to remove things like: myrepo/./build/
//                 // and get proper casing (see `getExactFullPath` comment)
//                 |> File.getExactFullPath
//                 |> Path.normalizePath
//
//             let watch = defaultArg watch false
//             let precompile = defaultArg precompile false
//
//             let fsprojPath =
//                 fsprojPath |> Option.map normalizeAbsolutePath |> Option.defaultValue rootDir
//
//             let! projFile =
//                 if IO.Directory.Exists(fsprojPath) then
//                     let files = IO.Directory.EnumerateFileSystemEntries(fsprojPath) |> Seq.toList
//
//                     files
//                     |> List.filter (fun file -> file.EndsWith(".fsproj", StringComparison.Ordinal))
//                     |> function
//                         | [] ->
//                             files
//                             |> List.filter (fun file -> file.EndsWith(".fsx", StringComparison.Ordinal))
//                         | candidates -> candidates
//                     |> function
//                         | [] -> Error("Cannot find .fsproj/.fsx in dir: " + fsprojPath)
//                         | [ fsproj ] -> Ok fsproj
//                         | _ -> Error("Found multiple .fsproj/.fsx in dir: " + fsprojPath)
//                 elif not (IO.File.Exists(fsprojPath)) then
//                     Error("File does not exist: " + fsprojPath)
//                 else
//                     Ok fsprojPath
//
//             let typedArrays = args.FlagOr("--typedArrays", not (language = TypeScript))
//
//             let outDir = args.Value("-o", "--outDir") |> Option.map normalizeAbsolutePath
//
//             let precompiledLib =
//                 args.Value("--precompiledLib") |> Option.map normalizeAbsolutePath
//
//             let fableLib = args.Value "--fableLib" |> Option.map Path.normalizePath
//             let useMSBuildForCracking = args.FlagOr("--legacyCracker", true)
//
//             do!
//                 match watch, outDir, fableLib with
//                 | true, _, _ when precompile -> Error("Cannot watch when precompiling")
//                 | _, None, _ when precompile -> Error("outDir must be specified when precompiling")
//                 | _, _, Some _ when Option.isSome precompiledLib ->
//                     Error("Cannot set fableLib when setting precompiledLib")
//                 | _ -> Ok()
//
//             do!
//                 let reservedDirs = [ Naming.fableModules; "obj" ]
//
//                 let outDirLast =
//                     outDir
//                     |> Option.bind (fun outDir -> outDir.TrimEnd('/').Split('/') |> Array.tryLast)
//                     |> Option.defaultValue ""
//
//                 if List.contains outDirLast reservedDirs then
//                     Error($"{outDirLast} is a reserved directory, please use another output directory")
//                 // TODO: Remove this check when typed arrays are compatible with typescript
//                 elif language = TypeScript && typedArrays then
//                     Error("Typescript output is currently not compatible with typed arrays, pass: --typedArrays false")
//                 else
//                     Ok()
//
//             let configuration =
//                 let defaultConfiguration =
//                     if watch then
//                         "Debug"
//                     else
//                         "Release"
//
//                 match args.Value("-c", "--configuration") with
//                 | None -> defaultConfiguration
//                 | Some c when String.IsNullOrWhiteSpace c -> defaultConfiguration
//                 | Some configurationArg -> configurationArg
//
//             let define =
//                 args.Values "--define"
//                 |> List.append
//                     [
//                         "FABLE_COMPILER"
//                         "FABLE_COMPILER_5"
//                         match language with
//                         | Php -> "FABLE_COMPILER_PHP"
//                         | Rust -> "FABLE_COMPILER_RUST"
//                         | Dart -> "FABLE_COMPILER_DART"
//                         | Python -> "FABLE_COMPILER_PYTHON"
//                         | TypeScript -> "FABLE_COMPILER_TYPESCRIPT"
//                         | JavaScript -> "FABLE_COMPILER_JAVASCRIPT"
//                     ]
//                 |> List.distinct
//
//             let fileExt =
//                 args.Value("-e", "--extension")
//                 |> Option.map (fun e ->
//                     if e.StartsWith('.') then
//                         e
//                     else
//                         "." + e
//                 )
//                 |> Option.defaultWith (fun () ->
//                     let usesOutDir = Option.isSome outDir
//                     File.defaultFileExt usesOutDir language
//                 )
//
//             let compilerOptions =
//                 CompilerOptionsHelper.Make(
//                     language = language,
//                     typedArrays = typedArrays,
//                     fileExtension = fileExt,
//                     define = define,
//                     debugMode = (configuration = "Debug"),
//                     optimizeFSharpAst = args.FlagEnabled "--optimize",
//                     noReflection = args.FlagEnabled "--noReflection",
//                     verbosity = verbosity
//                 )
//
//             let cliArgs =
//                 {
//                     ProjectFile = Path.normalizeFullPath projFile
//                     FableLibraryPath = fableLib
//                     RootDir = rootDir
//                     Configuration = configuration
//                     OutDir = outDir
//                     IsWatch = watch
//                     Precompile = precompile
//                     PrecompiledLib = precompiledLib
//                     PrintAst = args.FlagEnabled "--printAst"
//                     SourceMaps = args.FlagEnabled "-s" || args.FlagEnabled "--sourceMaps"
//                     SourceMapsRoot = args.Value "--sourceMapsRoot"
//                     NoRestore = args.FlagEnabled "--noRestore"
//                     NoCache = args.FlagEnabled "--noCache"
//                     // TODO: If we select optimize we cannot have F#/Fable parallelization
//                     NoParallelTypeCheck = args.FlagEnabled "--noParallelTypeCheck"
//                     Exclude = args.Values "--exclude"
//                     Replace =
//                         args.Values "--replace"
//                         |> List.map (fun v ->
//                             let v = v.Split(':')
//                             v.[0], normalizeAbsolutePath v.[1]
//                         )
//                         |> Map
//                     RunProcess = runProc
//                     CompilerOptions = compilerOptions
//                     Verbosity = verbosity
//                 }
//
//             let watchDelay =
//                 if watch then
//                     args.Value("--watchDelay") |> Option.map int |> Option.defaultValue 200 |> Some
//                 else
//                     None
//
//             let startCompilation () =
//                 State.Create(cliArgs, ?watchDelay = watchDelay, useMSBuildForCracking = useMSBuildForCracking)
//                 |> startCompilationAsync
//                 |> Async.RunSynchronously
//
//             return!
//                 // In CI builds, it may happen that two parallel Fable compilations try to precompile
//                 // the same library at the same time, use a lock file to prevent issues in that case.
//                 match outDir, precompile, watch with
//                 | Some outDir, true, false -> File.withLock outDir startCompilation
//                 | _ -> startCompilation ()
//                 |> Result.mapEither ignore fst
//         }

let clean (settings: ICommonArgs) =
    let logAlways content = toConsole content

    let logVerbose (content: Lazy<OutputPayload>) =
        if settings.verbosity.IsVerbose then
            logAlways content.Value

    let ignoreDirs = set [ "bin"; "obj"; "node_modules" ]

    let outDir =
        if settings.workingDirectory |> String.IsNullOrWhiteSpace then
            None
        else
            Some settings.workingDirectory

    let fileExt = settings.extension

    let cleanDir =
        outDir |> Option.defaultValue settings.workingDirectory |> IO.Path.GetFullPath

    // clean is a potentially destructive operation, we need a permission before proceeding
    let payload = [ V "all"; E $"*{fileExt}[.map]"; V "files in"; E cleanDir ]

    if not settings.yes then
        Many
            [
                V "This will recursively"
                MarkupCD(Color.DarkOrange, [ Decoration.Bold ], "delete")
                yield! payload
            ]
        |> toConsole

        if confirm "Continue?" |> not then
            V "Clean was cancelled." |> toConsole
            exit 0
    else
        V "Deleting" :: payload |> Many |> toConsole

    let mutable fileCount = 0
    let mutable fableModulesDeleted = false

    let asyncProcess (context: StatusContext) =
        async {
            let rec recClean dir =
                seq {
                    yield! IO.Directory.GetFiles(dir, "*" + fileExt)
                    yield! IO.Directory.GetFiles(dir, "*" + fileExt + ".map")
                }
                |> Seq.iter (fun file ->
                    async {
                        IO.File.Delete(file)
                        fileCount <- fileCount + 1
                        logVerbose (lazy ("Deleted " + file |> V))
                    }
                    |> Async.RunSynchronously
                )

                IO.Directory.GetDirectories(dir)
                |> Array.filter (fun subdir -> ignoreDirs.Contains(IO.Path.GetFileName(subdir)) |> not)
                |> Array.iter (fun subdir ->
                    if IO.Path.GetFileName(subdir) = Naming.fableModules then
                        IO.Directory.Delete(subdir, true)
                        fableModulesDeleted <- true

                        V $"Deleted {IO.Path.GetRelativePath(settings.workingDirectory, subdir)}"
                        |> logAlways
                    else
                        recClean subdir
                )

            recClean cleanDir
        }

    Status.start "Cleaning directories" asyncProcess |> Async.RunSynchronously


    if fileCount = 0 && not fableModulesDeleted then
        V
            ":orange_circle:  No files have been deleted. If Fable output is in another directory, pass it as an argument."
        |> logAlways
    else
        ":check_mark: Clean completed! Files deleted: " + string<int> fileCount
        |> V
        |> logAlways

let getStatus =
    function
    | JavaScript
    | TypeScript -> "stable"
    | Python -> "beta"
    | Rust -> "alpha"
    | Dart -> "beta"
    | Php -> "experimental"

let getLibPkgVersion =
    function
    | JavaScript -> Some("npm", "@fable-org/fable-library-js", Literals.JS_LIBRARY_VERSION)
    | TypeScript -> Some("npm", "@fable-org/fable-library-ts", Literals.JS_LIBRARY_VERSION)
    | Python
    | Rust
    | Dart
    | Php -> None

let private logPrelude (settings: ICliArgs) =
    if not settings.version then
        Many
            [
                MarkupCD(Color.DodgerBlue1, [ Decoration.Bold ], "Fable")
                E $"{Literals.VERSION}:"
                Dim $"F# to {settings.language} compiler"
                match getStatus settings.language with
                | "stable"
                | "" -> ()
                | status ->
                    MarkupD([ Decoration.Dim ], $"(status: {markupString None [ Decoration.Italic ] status})")
                    |> padLeft 2
                BL
                V "Thanks to the contributor!"
                E $"@{Contributors.getRandom ()}"
                BL
                E "Stand with"
                MarkupCD(Color.Aqua, [ Decoration.Bold ], "Ukraine!")
                Link "https://standwithukraine.com.ua/"
            ]
        |> customPanel HeaderPanel ""
        |> toConsoleInline

        match getLibPkgVersion settings.language with
        | Some(repository, pkgName, version) ->
            MarkupD([ Decoration.Dim ], $"Minimum {pkgName} version (when installed from {repository}): {version}")
            |> toConsole
        | None -> ()

type Processes =
    {
        run: RunProcess option
        runFast: RunProcess option
        runWatch: RunProcess option
        runScript: RunProcess option
    }

let private runWithSettings (settings: ICliArgs) (processes: Processes) =

    0

let runPreludeWithSettings (settings: ICliArgs) =
    let createLogger level =
        use factory =
            LoggerFactory.Create(fun builder ->
                builder.SetMinimumLevel(level).AddCustomConsole(fun options -> options.UseNoPrefixMsgStyle <- true)
                |> ignore
            )

        factory.CreateLogger("")

    // Set an initial logger in case, we fail to parse the CLI args
    Log.setLogger Verbosity.Normal (createLogger LogLevel.Information)
    Compiler.SetLanguageUnsafe settings.language

    let createProcess fast watch (input: string option) =
        if input.IsSome && input.Value |> String.IsNullOrWhiteSpace |> not then
            match input.Value.Split() |> Array.toList with
            | exeFile :: args when fast -> Some(RunProcess(exeFile, args, fast = true))
            | exeFile :: args when watch -> Some(RunProcess(exeFile, args, watch = true))
            | exeFile :: args -> Some(RunProcess(exeFile, args))
            | [] -> None
        else
            None

    let run = settings.run |> createProcess false false
    let runFast = settings.runFast |> createProcess true false
    let runWatch = settings.runWatch |> createProcess false true

    let runScript =
        settings.runScript
        |> function
            | Some str when str |> String.IsNullOrWhiteSpace -> None
            | Some str -> Some $"{Naming.placeholder} {str}" |> createProcess false true
            | _ -> None

    let level, verbosity =
        if settings.verbosity.IsVerbose then
            LogLevel.Debug, Verbosity.Verbose
        else
            LogLevel.Information, Verbosity.Normal

    Log.setLogger verbosity (createLogger level)
    logPrelude settings

    match settings.version with
    | true -> 0
    | false ->
        {
            run = run
            runFast = runFast
            runWatch = runWatch
            runScript = runScript
        }
        |> runWithSettings settings

﻿using System.IO.Enumeration;
using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Core.PatternMatching;
using ModdingTools.Templates.Extensions;

namespace ModdingTools.Templates.Writers;

public class InitialStructure : IDisposable
{
    // public static void ExecuteCommand(string command)

    private readonly GameOptions _gameOptions;
    private readonly TemplateOptions _templateOptions;

    public InitialStructure(GameOptions gameOptions, TemplateOptions templateOptions)
    {
        _gameOptions = gameOptions;
        _templateOptions = templateOptions;
    }

    // public async Task<int> InvokeAsync()

    public void Run()
    {
        Console.WriteLine("[GAME OPTIONS]");
        Console.WriteLine(_gameOptions);
        Console.WriteLine("[TEMPLATE OPTIONS]");
        Console.WriteLine(_templateOptions);

        var (Location, ProjectName, AssemblyName, SolutionName, RootNamespace) = _templateOptions.AsTuple();
        var (GamePath, GameName, UnityPlayerVersion, ManagedFrameworkVersion) = _gameOptions.AsTuple();

        var projectDescription = $"A BepInEx plugin for the {GameName} game.";
        // var dotnetTemplate = "bepinex5plugin";
        var dotnetTemplate = "bep6plugin_unity_mono";
        var forceOption = "";

        if (!Location.Exists) Location.Create();

        var shell = new CommandShell() { WorkingDirectory = Location }
            .Write(".gitignore")
            .Write(".editorconfig")
            .Go("src", true)
            .Exec($"dotnet new sln -n {SolutionName} {forceOption}")
            // TODO: Install/Update dotnet BepInEx.Templates
            .Exec(
                $"dotnet new {dotnetTemplate} -n {ProjectName} -o {ProjectName} {forceOption} -T net471 -U {UnityPlayerVersion} -D \"{projectDescription}\"")
            .Exec($"dotnet sln add {ProjectName}/{ProjectName}.csproj")
            .GoBack()
            .Go("lib", true)
            .Go("References", true);

        var asmTool = new AssemblyTool() { Destination = shell.WorkingDirectory };
        
        // TODO: Public accessor to well known path needed (ManagedPath)
        var managedPath = new DirectoryInfo(Path.Combine(Path.Combine(GamePath.FullName, GameName + "_Data"), "Managed"));

        var assemblyIgnorePatterns = new[]
        {
            "UnityEngine*dll", "System.*ll", "Photo*-DotNet.dll", "Newtonsoft.Json.dll", 
            "netstandard*dll", "mscorlib.dll", "LZ4.dll", "*.Zip.Unity.dll", "*SharpZipLib.dll",
            "I18N*dll", "CommandLine.dll", "clipper_library.dll", "Castle.Core.dll",
            "modio.*.dll", "ZBrowser.dll", "Rogo.Digital*dll", "Logitech.dll", "LeanTouch*dll",
            "LeanCommon*dll", "Backtrace.Unity.dll", "Mono.*.dll", "Unity.*.dll"
        };
        var ignoreCase = true;

        var ignoreMatches = new GlobMatcher(assemblyIgnorePatterns);



        managedPath
            .GetFiles("*.dll")
            .Where(dll => Negate(ignoreMatches.IsMatch(dll))).Select(asmTool.TryWriteAsPublic);
        
        
        
        shell.GoBack().GoBack();

        // private string[] GetAssemblyFilesInFolder(string externalDllsPath) => Directory.GetFiles(externalDllsPath, "*.dll");
        
        /*dotnet new sln -n mysolution
        dotnet new console -o myapp
        dotnet new classlib -o mylib1
        dotnet new classlib -o mylib2
        dotnet sln mysolution.sln add myapp\myapp.csproj
        dotnet sln mysolution.sln add mylib1\mylib1.csproj --solution-folder mylibs
        dotnet sln mysolution.sln add mylib2\mylib2.csproj --solution-folder mylibs*/

        Console.WriteLine("-- ALL DONE --");
    }

    public static bool Negate(bool flag) => !flag;

    public void Dispose()
    {
    }
}

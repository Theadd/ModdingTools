using System.IO.Enumeration;
using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Core.PatternMatching;
using ModdingTools.Templates.Extensions;

namespace ModdingTools.Templates.Tasks;

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

    public void Run(CommandShell Shell)
    {
        Console.WriteLine("[GAME OPTIONS]");
        Console.WriteLine(_gameOptions);
        Console.WriteLine("[TEMPLATE OPTIONS]");
        Console.WriteLine(_templateOptions);

        var (Location, ProjectName, AssemblyName, SolutionName, RootNamespace) = _templateOptions.AsTuple();
        var (GamePath, GameName, UnityPlayerVersion, ManagedFrameworkVersion) = _gameOptions.AsTuple();

        var projectDescription = $"A BepInEx plugin for the {GameName} game.";
        var dotnetTemplate = "bepinex5plugin";
        // var dotnetTemplate = "bep6plugin_unity_mono";

        // if (!Location.Exists) Location.Create();

        Shell
            .Write(".gitignore")
            .Write(".editorconfig")
            .Go("src", true)
            .Exec($"dotnet new sln -n {SolutionName}")
            // TODO: Install/Update dotnet BepInEx.Templates
            .Exec(
                $"dotnet new {dotnetTemplate} -n {ProjectName} -o {ProjectName} --force -T {ManagedFrameworkVersion} -U {UnityPlayerVersion} -D \"{projectDescription}\"")
            .WriteTemplate("Directory.Build.props.template", "Directory.Build.props")
            .Exec($"dotnet sln add {ProjectName}/{ProjectName}.csproj")
            .GoBack()
            .Go("lib", true)
            .Go("References", true);

        var asmTool = new AssemblyTool() { Destination = Shell.WorkingDirectory };

        Console.WriteLine($"AssemblyTool.Destination = {Shell.WorkingDirectory.FullName}");
        // TODO: Public accessor to well known path needed (ManagedPath)
        var managedPath =
            new DirectoryInfo(Path.Combine(Path.Combine(GamePath.FullName, GameName + "_Data"), "Managed"));

        var assemblyIgnorePatterns = new List<string>()
        {
            "UnityEngine*dll", "System.*ll", "Photo*-DotNet.dll", "Newtonsoft.Json.dll",
            "netstandard*dll", "mscorlib.dll", "LZ4.dll", "*.Zip.Unity.dll", "*SharpZipLib.dll",
            "I18N*dll", "CommandLine.dll", "clipper_library.dll", "Castle.Core.dll",
            "modio.*.dll", "ZBrowser.dll", "Rogo.Digital*dll", "Logitech.dll", "LeanTouch*dll",
            "LeanCommon*dll", "Backtrace.Unity.dll", "Mono.*.dll", "Unity.*.dll"
        };

        var ignoreMatches = new GlobMatcher(assemblyIgnorePatterns);

        Console.WriteLine($"Managed PATH = {managedPath.FullName}");

        foreach (var dll in managedPath.GetFiles("*.dll"))
            if (!ignoreMatches.IsMatch(dll))
                asmTool.TryWriteAsPublic(dll);

        Shell.GoBack().GoBack();
    }

    public void Dispose()
    {
    }
}

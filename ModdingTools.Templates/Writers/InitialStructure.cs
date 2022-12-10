using ModdingTools.Core;
using ModdingTools.Core.Options;
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
        var (GamePath, GameName, UnityPlayerVersion) = _gameOptions.AsTuple();

        var projectDescription = $"A BepInEx plugin for the {GameName} game.";
        // var dotnetTemplate = "bepinex5plugin";
        var dotnetTemplate = "bep6plugin_unity_mono";
        var forceOption = "";

        if (!Location.Exists) Location.Create();

        var shell = new CommandShell() { WorkingDirectory = Location }
            .Go("lib", true)
            .Go("References", true)
            .GoBack()
            .GoBack()
            .Write(".gitignore")
            .Write(".editorconfig")
            .Go("src", true)
            .Exec($"dotnet new sln -n {SolutionName} {forceOption}")
            // TODO: Install/Update dotnet BepInEx.Templates
            .Exec(
                $"dotnet new {dotnetTemplate} -n {ProjectName} -o {ProjectName} {forceOption} -T net471 -U {UnityPlayerVersion} -D \"{projectDescription}\"")
            .Exec($"dotnet sln add {ProjectName}/{ProjectName}.csproj")
            .GoBack();

        /*dotnet new sln -n mysolution
        dotnet new console -o myapp
        dotnet new classlib -o mylib1
        dotnet new classlib -o mylib2
        dotnet sln mysolution.sln add myapp\myapp.csproj
        dotnet sln mysolution.sln add mylib1\mylib1.csproj --solution-folder mylibs
        dotnet sln mysolution.sln add mylib2\mylib2.csproj --solution-folder mylibs*/

        Console.WriteLine("-- ALL DONE --");
    }

    public void Dispose()
    {
    }
}

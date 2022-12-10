using ModdingTools.Core;
using ModdingTools.Core.Options;

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

        if (!_templateOptions.Location!.Exists)
            _templateOptions.Location!.Create();
        /*
        _templateOptions.Location!.Create();

        _templateOptions.Location
            .CreateSubdirectory("lib")
            .CreateSubdirectory("src");
        */

        new CommandShell() { WorkingDirectory = _templateOptions.Location }
            .Go("lib", true)
            .Go("References", true)
            .GoBack()
            .GoBack()
            .Go("src", true)
            .GoBack();
    }

    public void Dispose()
    {
    }
}

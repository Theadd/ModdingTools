using System.CommandLine;
using ModdingTools.App.Binding;
using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Tasks;
using Newtonsoft.Json;

namespace ModdingTools.App.CommandLine;

public static class CommandNew
{
    public static Command Create()
    {
        var cmd = new Command("new", "Create a new mod project for your game.")
        {
            CommandLineArgs.GameOption,
            CommandLineArgs.InitialProjectNameOption,
            CommandLineArgs.AssemblyNameOption,
            CommandLineArgs.SolutionNameOption,
            CommandLineArgs.RootNamespaceOption,
            CommandLineArgs.DotnetNewTemplateShortNameOption
        };

        cmd.AddArgument(CommandLineArgs.EmptyDirectoryArgument);
        cmd.SetHandler(
            async (gameOptions, templateOptions, dryRun, quiet) =>
            {
                await Run(gameOptions, templateOptions, dryRun, quiet);
            },
            new GameOptionsBinder(CommandLineArgs.GameOption),
            new TemplateOptionsBinder(
                CommandLineArgs.EmptyDirectoryArgument!,
                CommandLineArgs.InitialProjectNameOption!,
                CommandLineArgs.AssemblyNameOption!,
                CommandLineArgs.SolutionNameOption!,
                CommandLineArgs.RootNamespaceOption!,
                CommandLineArgs.DotnetNewTemplateShortNameOption!),
            CommandLineArgs.DryRunOption,
            CommandLineArgs.QuietOption
        );

        return cmd;
    }

    private static IEnumerable<ConsoleColor> ColorPaletteA = new[]
        { ConsoleColor.DarkGray, ConsoleColor.Green, ConsoleColor.White };
    private static IEnumerable<ConsoleColor> ColorPaletteB = new[]
        { ConsoleColor.DarkGray, ConsoleColor.Green, ConsoleColor.Yellow };

    private static void DisplayCommandShellEntry(CommandShell.CommandShellEntry entry)
    {
        if (entry.QuietMode) return;

        var palette = (entry.CommandName is "Write" or "dotnet" ? ColorPaletteB : ColorPaletteA).ToArray();

        Console.ForegroundColor = palette.ElementAt(0);
        Console.Write("$ ");
        Console.ForegroundColor = palette.ElementAt(1);
        Console.Write($"{entry.CommandName}");
        Console.ForegroundColor = palette.ElementAt(2);
        Console.WriteLine($" {entry.CommandArgs}");
        Console.ResetColor();
    }

    private static async Task Run(GameOptions gameOptions, TemplateOptions templateOptions, bool dryRun,
        bool quiet)
    {
        var allOptions = new AllOptions(gameOptions.AsRecord(), templateOptions.AsRecord(), dryRun, quiet);
        var shell = CommandShellBuilder.Create(allOptions);

        shell.OnAction += DisplayCommandShellEntry;

        var success =
            SafeInvoke.All(quiet,

                // Create basic directory structure and common files
                () => RootDirectoryStructure
                    .Create(allOptions)
                    .InvokeAsync(shell)
                    .Result,

                // Generate publicized version of game assemblies
                () => ManagedAssembliesPublicizer
                    .Create(allOptions)
                    .InvokeAsync(shell.SetWorkingDirectory("{{Location}}/lib/references"))
                    .Result,

                // Create VS Solution file (*.sln)
                () => DotnetSolution
                    .Create(allOptions)
                    .InvokeAsync(shell.SetWorkingDirectory("{{Location}}/src"))
                    .Result,

                // Install/Update dotnet BepInEx.Templates
                () => DotnetBepInExTemplates
                    .Create(allOptions)
                    .InvokeAsync(shell)
                    .Result,

                // Initialize first project and add to solution
                () => DotnetProjectTemplate
                    .Create(allOptions)
                    .InvokeAsync(shell.SetWorkingDirectory("{{Location}}/src"))
                    .Result
                
                // TODO: [Preloader.Entrypoint]
                // TODO: Type = MonoBehaviour
            );

        string json = JsonConvert.SerializeObject(shell.Tree, Formatting.Indented);
        Console.WriteLine(json);
        
        shell.OnAction -= DisplayCommandShellEntry;
    }
}

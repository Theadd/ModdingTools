using System.CommandLine;
using ModdingTools.App.Binding;
using ModdingTools.Core;
using ModdingTools.Core.Common;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Tasks;
using Newtonsoft.Json;

namespace ModdingTools.App.CommandLine;

public static class CommandNew
{
    private static Command? Instance { get; set; } = default;
    
    public static Command Create()
    {
        if (Instance != null) return Instance;
        
        var cmd = new Command("new", "Create a new mod project for your game.")
        {
            CommandLineArgs.GameOption,
            CommandLineArgs.InitialProjectNameOption,
            CommandLineArgs.AssemblyNameOption,
            CommandLineArgs.SolutionNameOption,
            CommandLineArgs.RootNamespaceOption,
            CommandLineArgs.DotnetNewTemplateShortNameOption,
            CommandLineArgs.JsonOption,
            CommandLineArgs.ShowConfigOption,
            CommandLineArgs.ShowFsOption
        };

        cmd.AddArgument(CommandLineArgs.EmptyDirectoryArgument);
        cmd.SetHandler(
            async (gameOptions, templateOptions, inJson, showConfig, showFs, dryRun, quiet) =>
            {
                await Run(gameOptions, templateOptions, inJson, showConfig, showFs, dryRun, quiet);
            },
            new GameOptionsBinder(
                CommandLineArgs.GameOption,
                CommandLineArgs.EmptyDirectoryArgument),
            new TemplateOptionsBinder(
                CommandLineArgs.EmptyDirectoryArgument,
                CommandLineArgs.InitialProjectNameOption,
                CommandLineArgs.AssemblyNameOption,
                CommandLineArgs.SolutionNameOption,
                CommandLineArgs.RootNamespaceOption,
                CommandLineArgs.DotnetNewTemplateShortNameOption),
            CommandLineArgs.JsonOption,
            CommandLineArgs.ShowConfigOption,
            CommandLineArgs.ShowFsOption,
            CommandLineArgs.DryRunOption,
            CommandLineArgs.QuietOption
        );

        return (Instance = cmd);
    }

    private static async Task Run(GameOptions gameOptions, TemplateOptions templateOptions, bool inJson, 
        bool showConfig, bool showFs, bool dryRun, bool quiet)
    {
        var allOptions = new AllOptions(gameOptions.AsRecord(), templateOptions.AsRecord(), dryRun, quiet);

        if (dryRun && quiet && !showFs && showConfig)
        {
            JsonLib.Print(new ShowOptions() { Config = allOptions });
            return;
        }
        
        var shell = CommandShellBuilder.Create(allOptions);

        shell.OnAction += CommandLineHelper.DisplayCommandShellEntry;

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

        if (showFs || showConfig)
        {
            if (inJson)
            {
                JsonLib.Print(new ShowOptions()
                {
                    Config = showConfig ? allOptions : null,
                    FileSystem = showFs ? shell.Tree : null
                });
            }
            else
            {
                // TODO
            }
        }

        shell.OnAction -= CommandLineHelper.DisplayCommandShellEntry;
    }
}

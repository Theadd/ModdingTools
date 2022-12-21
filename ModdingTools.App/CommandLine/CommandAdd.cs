using System.CommandLine;
using ModdingTools.App.Binding;
using ModdingTools.Core;
using ModdingTools.Core.Common;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Tasks;
using Newtonsoft.Json;

namespace ModdingTools.App.CommandLine;

public static class CommandAdd
{
    private static Command? Instance { get; set; } = default;

    public static Command Create()
    {
        if (Instance != null) return Instance;
        
        var cmd = new Command("add", "Add another project from our starters kit.")
        {
            // CommandLineArgs.ProjectNameArgument,
            CommandLineArgs.AssemblyNameOption,
            CommandLineArgs.RootNamespaceOption,
            CommandLineArgs.DotnetNewTemplateShortNameOption.Hide()
        };

        cmd.AddArgument(CommandLineArgs.ProjectNameArgument);
        cmd.AddArgument(CommandLineArgs.KickstartTemplateArgument.FromAmong(new []{ "AssetBundles", "StyledGUI" }));
        cmd.AddArgument(CommandLineArgs.TargetArgument);
        cmd.SetHandler(
            async (gameOptions, templateOptions, dryRun, quiet) =>
            {
                await Run(gameOptions, templateOptions, dryRun, quiet);
            },
            new GameOptionsBinder(
                CommandLineArgs.GameOption,
                CommandLineArgs.TargetArgument),
            new TemplateOptionsBinder(
                CommandLineArgs.EmptyDirectoryArgument,
                CommandLineArgs.ProjectNameArgument,
                CommandLineArgs.AssemblyNameOption,
                CommandLineArgs.SolutionNameOption,
                CommandLineArgs.RootNamespaceOption,
                CommandLineArgs.DotnetNewTemplateShortNameOption),
            CommandLineArgs.DryRunOption,
            CommandLineArgs.QuietOption
        );

        return (Instance = cmd);
    }

    private static async Task Run(GameOptions gameOptions, TemplateOptions templateOptions, bool dryRun,
        bool quiet)
    {
        var allOptions = new AllOptions(gameOptions.AsRecord(), templateOptions.AsRecord(), dryRun, quiet);
        var shell = CommandShellBuilder.Create(allOptions);

        shell.OnAction += CommandLineHelper.DisplayCommandShellEntry;

        /*
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
        */
        
        JsonLib.Print(shell.Tree);
        JsonLib.Print(allOptions);
        
        shell.OnAction -= CommandLineHelper.DisplayCommandShellEntry;
    }
}

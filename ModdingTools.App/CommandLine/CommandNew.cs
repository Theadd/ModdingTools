using System.CommandLine;
using ModdingTools.App.Binding;
using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Tasks;

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
            CommandLineArgs.RootNamespaceOption
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
                CommandLineArgs.RootNamespaceOption!),
            CommandLineArgs.DryRunOption,
            CommandLineArgs.QuietOption
        );

        return cmd;
    }

    private static async Task Run(GameOptions gameOptions, TemplateOptions templateOptions, bool dryRun,
        bool quiet)
    {
        var allOptions = new AllOptions(gameOptions.AsRecord(), templateOptions.AsRecord(), dryRun, quiet);
        var shell = CreateCommandShell.Create(allOptions);
        
        await Task.Run(() => SafeInvoke.All(false,
            async () => await CreateDirectoryStructure.Create(allOptions).InvokeAsync(shell),
            async () => await PublicizeManagedAssemblies.Create(allOptions).InvokeAsync(shell)));
    }
}

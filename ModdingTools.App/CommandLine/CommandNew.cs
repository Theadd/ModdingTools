using System.CommandLine;
using ModdingTools.App.Binding;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Writers;

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
        cmd.AddAlias("init");
        cmd.SetHandler(
            async (gameOptions, templateOptions, dryRun) =>
            {
                // await ExecuteCommand(directory!, game, projectName, assemblyName, dryRun);
                await Run(gameOptions, templateOptions, dryRun);
            },
            new GameOptionsBinder(CommandLineArgs.GameOption),
            new TemplateOptionsBinder(
                CommandLineArgs.EmptyDirectoryArgument!, 
                CommandLineArgs.InitialProjectNameOption!, 
                CommandLineArgs.AssemblyNameOption!, 
                CommandLineArgs.SolutionNameOption!, 
                CommandLineArgs.RootNamespaceOption!),
            
            
            
            CommandLineArgs.DryRunOption
        );

        return cmd;
    }

    private static async Task Run(GameOptions gameOptions, TemplateOptions templateOptions, bool dryRun)
    {
        if (dryRun) return;
        
        var genInitialStructure = new InitialStructure(gameOptions, templateOptions);
        genInitialStructure.Run();
    }

}

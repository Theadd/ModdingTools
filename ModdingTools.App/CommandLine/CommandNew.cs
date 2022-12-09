using System.CommandLine;
using System.CommandLine.Parsing;
using ModdingTools.Core.PluginTemplates.BepInEx5;

namespace ModdingTools.App.CommandLine;

public static class CommandNew
{
    public static Command Create()
    {
        var cmd = new Command("new", "Create a new mod project for your game.")
        {
            CommandLineArgs.GameOption,
            CommandLineArgs.InitialProjectNameOption, 
            CommandLineArgs.AssemblyNameOption
        };

        cmd.AddArgument(CommandLineArgs.EmptyDirectoryArgument);
        cmd.AddAlias("init");
        cmd.SetHandler(
            async (directory, game, projectName, assemblyName, dryRun) => { await ExecuteCommand(directory!, game!, projectName, assemblyName, dryRun); },
            CommandLineArgs.EmptyDirectoryArgument, 
            CommandLineArgs.GameOption,
            CommandLineArgs.InitialProjectNameOption, 
            CommandLineArgs.AssemblyNameOption, 
            CommandLineArgs.DryRunOption
        );

        return cmd;
    }

    private static async Task ExecuteCommand(DirectoryInfo directory, DirectoryInfo game, string? projectName, string? assemblyName, bool dryRun)
    {
        if (dryRun) return;

        var gameName = CommandLineUtils.GetGameName(game);
        var solutionName = gameName + "Hacks";
        
        projectName ??= directory.Name.Replace(' ', '.');
        assemblyName ??= projectName;

        Console.WriteLine($@"
The new project files will be created using the following settings:

    TARGET_GAME = {gameName}
    GAME_PATH = {game.FullName}
    TARGET_LOCATION = {directory.FullName}
    PROJECT_NAME = {projectName}
    ASSEMBLY_NAME = {assemblyName}
    UNITY_ENGINE = {CommandLineUtils.GetUnityPlayerVersion(game)}
");

        var config = new CreateBepInExPluginSettings()
        {
            AssemblyName = assemblyName,
            SolutionName = solutionName,
            ProjectName = projectName,
            RootNamespace = projectName,
            Location = directory.FullName,
            IsMultiProjectSolution = true
        };
        
        var invalidNames = new List<string>();
        var proceed = config.Validate(ref invalidNames);
        
        // if proceed is false but invalidNames is empty is due to target directory is not empty.

        foreach (var invalidName in invalidNames)
        {
            Console.WriteLine("INVALID PLUGIN SETTINGS VALUE FOR: " + invalidName);
        }

        if (!proceed) return;
        
        Console.WriteLine("BEGIN GENERATING PLUGIN");
        /*
        var generator = new BepInEx5PluginGenerator()
        {
            Configuration = config,
            Paths = Paths.Export(game.FullName)
        };

        generator.Create();

        fileSystemUtil.ShowFolder(generator.TargetPath);
        */
        Console.WriteLine("END GENERATING PLUGIN");

        await Task.Delay(100);
    }
}

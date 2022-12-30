using System.CommandLine;
using ModdingTools.App.Binding;
using ModdingTools.Core;
using ModdingTools.Core.Common;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Tasks;
using Newtonsoft.Json;

namespace ModdingTools.App.CommandLine;

public static class CommandGameInfo
{
    private static Command? Instance { get; set; } = default;

    public static Command Create()
    {
        if (Instance != null) return Instance;
        
        var cmd = new Command("gameinfo", "Retrieve and display any useful information from the unity game in the current directory.")
        {
            CommandLineArgs.JsonOption,
            CommandLineArgs.GameOption
        };
        
        cmd.AddArgument(CommandLineArgs.TargetArgument);
        cmd.SetHandler(
            async (gameOptions, inJson, dryRun, quiet) =>
            {
                await Run(gameOptions, inJson, dryRun, quiet);
            },
            new GameOptionsBinder(
                CommandLineArgs.GameOption,
                CommandLineArgs.TargetArgument),
            CommandLineArgs.JsonOption,
            CommandLineArgs.DryRunOption,
            CommandLineArgs.QuietOption
        );

        return (Instance = cmd);
    }

    private static async Task Run(GameOptions gameOptions, bool inJson, bool dryRun, bool quiet)
    {
        if (inJson)
        {
            JsonLib.Print(gameOptions);
        }
        else
        {
            Console.WriteLine(gameOptions.ToString());
        }
    }
}

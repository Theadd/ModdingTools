using System.CommandLine;
using ModdingTools.App.Binding;
using ModdingTools.Core;
using ModdingTools.Core.Common;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Tasks;
using Newtonsoft.Json;

namespace ModdingTools.App.CommandLine;

public static class CommandBepInEx
{
    private static Command? Instance { get; set; } = default;

    public static Command Create()
    {
        if (Instance != null) return Instance;
        
        var cmd = new Command("bepinex", "Manage installed BepInEx within the unity game in the current directory or in the provided path, if any.");
        
        var subCommandInfo = new Command("info", "Retrieve information about the installed BepInEx version.")
        {
            CommandLineArgs.JsonOption,
            CommandLineArgs.GameOption
        };
        // subCommandInfo.AddArgument(CommandLineArgs.TargetArgument);
        subCommandInfo.SetHandler(
            async (bepInExOptions, inJson, dryRun, quiet) =>
            {
                await RunInfo(bepInExOptions, inJson, dryRun, quiet);
            },
            new BepInExOptionsBinder(
                CommandLineArgs.GameOption,
                CommandLineArgs.TargetArgument),
            CommandLineArgs.JsonOption,
            CommandLineArgs.DryRunOption,
            CommandLineArgs.QuietOption
        );

        cmd.AddCommand(subCommandInfo);
        
        return (Instance = cmd);
    }

    private static async Task RunInfo(BepInExOptions bepInExOptions, bool inJson, bool dryRun, bool quiet)
    {
        if (inJson)
        {
            JsonLib.Print(bepInExOptions);
        }
        else
        {
            Console.WriteLine(bepInExOptions.ToString());
        }
    }
}

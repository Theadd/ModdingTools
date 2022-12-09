using System.CommandLine;
using System.Runtime.InteropServices;
using ModdingTools.App.CommandLine;

namespace ModdingTools.App;

class Program
{
    // S:\Program Files\Steam\steamapps\common\Humankind
    static async Task<int> Main(string[] args)
    {

        var rootCommand = new RootCommand("ModdingTools CLI");
        
        rootCommand.AddGlobalOption(CommandLineArgs.DryRunOption);
        rootCommand.AddCommand(CommandNew.Create());
        
        return await rootCommand.InvokeAsync(args);
    }

}

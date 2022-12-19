using System.CommandLine;
using ModdingTools.App.Binding;
using ModdingTools.Core;
using ModdingTools.Core.Extensions;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Tasks;

namespace ModdingTools.App.CommandLine;

public static class CommandInfo
{
    public static Command Create()
    {
        var cmd = new Command("info", "Retrieve and display any useful information from the current directory.");

        cmd.AddArgument(CommandLineArgs.TargetArgument);
        cmd.SetHandler(
            async (target, dryRun, quiet) =>
            {
                await Run(target!, dryRun, quiet);
            },
            CommandLineArgs.TargetArgument,
            CommandLineArgs.DryRunOption,
            CommandLineArgs.QuietOption
        );

        return cmd;
    }

    private static async Task Run(DirectoryInfo gamePath, bool dryRun, bool quiet)
    {
        var gameName = BindingOptionsHelper.GetGameName(gamePath);

        var csharpAssembly = gamePath
            .GetDirectory(gameName + "_Data")
            ?.GetDirectory("Managed")
            ?.GetFile("Assembly-CSharp.dll");

        if (csharpAssembly != null)
        {
            Console.WriteLine("Assemblies referenced by Assembly-CSharp.dll:");
            
            foreach (var asm in csharpAssembly.GetAssemblyReferences())
            {
                Console.WriteLine($"   {asm.Name} (Version={asm.Version})");
            }
        }

    }
}

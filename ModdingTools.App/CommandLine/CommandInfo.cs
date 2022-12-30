using System.CommandLine;
using System.Xml.Linq;
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

    public static int FirstRunner(DirectoryInfo targetPath, bool dryRun, bool quiet)
    {
        FileInfo? dbp = (targetPath.GetDirectory("src") ?? targetPath).GetFile("Directory.Build.props");

        if (dbp == null) return -1;
        
        XElement root = XElement.Load(dbp.FullName);
        
        XElement? gamePathX = root.Elements("PropertyGroup")
            .Where(el => el.HasElements && el.Elements("GAME_PATH").Any())
            .FirstOrDefault(default(XElement))
            ?.Element("GAME_PATH");

        var gamePath = gamePathX?.Value ?? "NOT FO_UND";
        
        Console.WriteLine($">>>>> FOUND GAME_PATH = \"{gamePath}\"");
        return 0;
    }

    private static async Task Run(DirectoryInfo gamePath, bool dryRun, bool quiet)
    {

        FirstRunner(gamePath, dryRun, quiet);
        
        return;
        
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

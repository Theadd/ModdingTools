using System;
using System.IO;
using System.Linq;
using ModdingTools.Core.PluginGenerator;
using ModdingTools.Core.Utils;

namespace ModdingTools.Core.PluginTemplates.BepInEx5;

public partial class BepInEx5PluginGenerator : AbstractPluginGenerator
{
    public ICreateBepInExPluginSettings Config => (ICreateBepInExPluginSettings) Configuration;

    // private static string[] IgnoreDlls = { "System", "Unity", "Mono", "netstandard", "mscorlib" };
    private static string[] AllowedDlls = { "Amplitude", "Assembly", "Sirenix", "modio" };

    public static bool LibraryFilter(string name) =>
        AllowedDlls.Any(bad => name.StartsWith(bad, StringComparison.Ordinal));

    public override bool OnCreate()
    {
        var pathToDlls = Path.Combine(TargetPath, "./lib/References");
        var vsSolutionFilePath = Path.Combine(TargetPath, RelativePathToSolutionFile);
        
        var result = SafeInvoke.All(false,
            CreateBasicDirectoryStructure,
            CreateTemplateResources,
            () => this.WriteAllReferencesAsPublic(pathToDlls, LibraryFilter),
            () => CreateCSharpProjectFile(pathToDlls, LibraryFilter),
            () => CreateVisualStudioSolutionFile(vsSolutionFilePath));

        return result;
    }
    
}

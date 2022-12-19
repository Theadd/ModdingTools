using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Core.PatternMatching;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Copy publicized game assemblies (*.dll) into CommandShell's WorkingDirectory.
/// </summary>
public class ManagedAssembliesPublicizer : RunnableTask
{
    public static List<string> AssemblyIgnorePatterns { get; } = new()
    {
        "UnityEngine*dll", "System.*ll", "Photo*-DotNet.dll", "Newtonsoft.Json.dll",
        "netstandard*dll", "mscorlib.dll", "LZ4.dll", "*.Zip.Unity.dll", "*SharpZipLib.dll",
        "I18N*dll", "CommandLine.dll", "clipper_library.dll", "Castle.Core.dll",
        "modio.*.dll", "ZBrowser.dll", "Rogo.Digital*dll", "Logitech.dll", "LeanTouch*dll",
        "LeanCommon*dll", "Backtrace.Unity.dll", "Mono.*.dll", "Unity.*.dll"
    };

    protected ManagedAssembliesPublicizer(AllOptions allOptions) : base(allOptions) { }

    public static ManagedAssembliesPublicizer Create(AllOptions allOptions) => new(allOptions);
    
    protected override bool Invoke()
    {
        var asmTool = new AssemblyTool() { Destination = Shell.WorkingDirectory };
        var ignoreMatches = new GlobMatcher(AssemblyIgnorePatterns);
        
        foreach (var dll in ManagedAssembliesPath.GetFiles("*.dll"))
            if (!ignoreMatches.IsMatch(dll))
            {
                Shell.Tree?.Write(GetAssemblyDestinationFile(dll));
                Shell.Trigger("Write", dll.Name, DryRun, () => asmTool.TryWriteAsPublic(dll));
            }
        return true;
    }

    private FileInfo GetAssemblyDestinationFile(FileInfo dll) =>
        new FileInfo(Path.Combine(Shell.WorkingDirectory.FullName, dll.Name));
}

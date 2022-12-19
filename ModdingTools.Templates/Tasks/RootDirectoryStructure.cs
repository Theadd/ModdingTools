using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Extensions;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Copy publicized game assemblies (*.dll) into CommandShell's WorkingDirectory.
/// </summary>
public class RootDirectoryStructure : RunnableTask
{
    protected RootDirectoryStructure(AllOptions allOptions) : base(allOptions) { }

    public static RootDirectoryStructure Create(AllOptions allOptions) => new(allOptions);
    
    protected override bool Invoke()
    {
        Shell
            .Write(".gitignore")
            .Write(".editorconfig")
            .Go("src", true)
            .WriteTemplate("Directory.Build.props.template", "Directory.Build.props")
            .GoBack()
            .Go("lib", true)
            .Go("references", true)
            .GoBack()
            .Go("test", true)
            .Unzip("GameAssets.zip");
        
        return true;
    }
}

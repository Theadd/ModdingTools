using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Extensions;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Copy publicized game assemblies (*.dll) into CommandShell's WorkingDirectory.
/// </summary>
public class CreateDirectoryStructure : RunnableTask
{
    public CommandShell Shell { get; set; } = default!;

    protected CreateDirectoryStructure(AllOptions allOptions) : base(allOptions) { }

    public static CreateDirectoryStructure Create(AllOptions allOptions) => new(allOptions);

    public async Task<bool> InvokeAsync(CommandShell shell) =>
        await Task.Run(() => 
            SafeInvoke.All(false, () => SafeInvoke.TryInvoke(() => Shell = shell), Invoke));

    protected override bool Invoke()
    {
        Shell
            .Write(".gitignore")
            .Write(".editorconfig")
            .Go("src", true)
            .WriteTemplate("Directory.Build.props.template", "Directory.Build.props")
            .GoBack()
            .Go("lib", true)
            .Go("References", true);
        
        return true;
    }
}

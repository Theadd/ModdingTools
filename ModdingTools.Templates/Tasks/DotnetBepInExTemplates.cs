using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Extensions;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Set CommandShell's WorkingDirectory to `{{Location}}/src` and generate
/// msbuild solution and other related files.
/// </summary>
public class DotnetBepInExTemplates : RunnableTask
{
    public CommandShell Shell { get; set; } = default!;

    protected DotnetBepInExTemplates(AllOptions allOptions) : base(allOptions) { }

    public static DotnetBepInExTemplates Create(AllOptions allOptions) => new(allOptions);

    public async Task<bool> InvokeAsync(CommandShell shell) =>
        await Task.Run(() => 
            SafeInvoke.All(false, () => SafeInvoke.TryInvoke(() => Shell = shell), Invoke));

    protected override bool Invoke()
    {
        Shell
            .Exec("dotnet new install BepInEx.Templates::2.0.0-be.1 --nuget-source https://nuget.bepinex.dev/v3/index.json");

        return true;
    }
}

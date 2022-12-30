using ModdingTools.Core;
using ModdingTools.Core.Options;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Set CommandShell's WorkingDirectory to `{{Location}}/src` and generate
/// msbuild solution and other related files.
/// </summary>
public class DotnetBepInExTemplates : RunnableTask
{
    protected DotnetBepInExTemplates(AllOptions allOptions) : base(allOptions) { }

    public static DotnetBepInExTemplates Create(AllOptions allOptions) => new(allOptions);
    
    protected override bool Invoke()
    {
        Shell
            .Exec("dotnet new install BepInEx.Templates::2.0.0-be.1 --nuget-source https://nuget.bepinex.dev/v3/index.json");

        return true;
    }
}

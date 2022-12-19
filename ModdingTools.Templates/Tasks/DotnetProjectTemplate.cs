using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Extensions;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Set CommandShell's WorkingDirectory to `{{Location}}/src` and generate
/// msbuild solution and other related files.
/// </summary>
public class DotnetProjectTemplate : RunnableTask
{
    public CommandShell Shell { get; set; } = default!;

    protected DotnetProjectTemplate(AllOptions allOptions) : base(allOptions) { }

    public static DotnetProjectTemplate Create(AllOptions allOptions) => new(allOptions);

    public async Task<bool> InvokeAsync(CommandShell shell) =>
        await Task.Run(() => 
            SafeInvoke.All(false, () => SafeInvoke.TryInvoke(() => Shell = shell), Invoke));

    protected override bool Invoke()
    {
        var pName = CommandShell.Quote(ProjectName);
        
        Shell
            .Exec("dotnet new {{TemplateShortName}} -n " + pName + " -o " + pName + " --force -T {{ManagedFrameworkVersion}} -U {{UnityPlayerVersion}} -D \"A BepInEx plugin for the {{GameName}} game.\"")
            .Exec($"dotnet sln add {pName}/{pName}.csproj");

        UpdateFileTree();

        return true;
    }
    
    private void UpdateFileTree()
    {
        Shell.Tree?.Write(new FileInfo(Path.Combine(Shell.WorkingDirectory.FullName, $"{ProjectName}/{ProjectName}.csproj")));
        Shell.Tree?.Write(new FileInfo(Path.Combine(Shell.WorkingDirectory.FullName, $"{ProjectName}/Plugin.cs")));
    }
}

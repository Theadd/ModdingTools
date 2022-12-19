using ModdingTools.Core;
using ModdingTools.Core.Options;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Generate msbuild solution and other related files.
/// </summary>
public class DotnetProjectTemplate : RunnableTask
{
    protected DotnetProjectTemplate(AllOptions allOptions) : base(allOptions) { }

    public static DotnetProjectTemplate Create(AllOptions allOptions) => new(allOptions);
    
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

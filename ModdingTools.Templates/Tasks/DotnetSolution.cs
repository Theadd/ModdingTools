using ModdingTools.Core;
using ModdingTools.Core.Options;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Generate msbuild solution file (*.sln).
/// </summary>
public class DotnetSolution : RunnableTask
{
    protected DotnetSolution(AllOptions allOptions) : base(allOptions) { }

    public static DotnetSolution Create(AllOptions allOptions) => new(allOptions);

    protected override bool Invoke()
    {
        Shell
            .Exec("dotnet new sln -n " + CommandShell.Quote(SolutionName));

        UpdateFileTree();
        
        return true;
    }

    private void UpdateFileTree()
    {
        Shell.Tree?.Write(new FileInfo(Path.Combine(Shell.WorkingDirectory.FullName, SolutionName + ".sln")));
    }
}

using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Templates.Extensions;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Set CommandShell's WorkingDirectory to `{{Location}}/src` and generate
/// msbuild solution and other related files.
/// </summary>
public class DotnetSolution : RunnableTask
{
    public CommandShell Shell { get; set; } = default!;

    protected DotnetSolution(AllOptions allOptions) : base(allOptions) { }

    public static DotnetSolution Create(AllOptions allOptions) => new(allOptions);

    public async Task<bool> InvokeAsync(CommandShell shell) =>
        await Task.Run(() => 
            SafeInvoke.All(false, () => SafeInvoke.TryInvoke(() => Shell = shell), Invoke));

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

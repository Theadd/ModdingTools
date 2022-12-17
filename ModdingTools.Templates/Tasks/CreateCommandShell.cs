using ModdingTools.Core;
using ModdingTools.Core.Options;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Creates an instance of CommandShell to be used in other RunnableTasks.
/// </summary>
public class CreateCommandShell : RunnableTask
{
    public CommandShell Shell { get; set; } = default!;
    
    protected CreateCommandShell(AllOptions allOptions) : base(allOptions) { }

    public static CommandShell Create(AllOptions allOptions)
    {
        var instance = new CreateCommandShell(allOptions);
        instance.Invoke();
        return instance.Shell;
    }

    protected override bool Invoke()
    {
        Shell = new CommandShell()
        {
            DryRun = DryRun,
            QuietMode = QuietMode,
            SubstitutionVars = new Dictionary<string, string>()
            {
                { $"{{{{{nameof(GamePath)}}}}}", GamePath.FullName },
                { $"{{{{{nameof(GameName)}}}}}", GameName },
                { $"{{{{{nameof(UnityPlayerVersion)}}}}}", UnityPlayerVersion },
                { $"{{{{{nameof(ManagedFrameworkVersion)}}}}}", ManagedFrameworkVersion },
                { $"{{{{{nameof(Location)}}}}}", Location.FullName },
                { $"{{{{{nameof(ProjectName)}}}}}", ProjectName },
                { $"{{{{{nameof(AssemblyName)}}}}}", AssemblyName },
                { $"{{{{{nameof(SolutionName)}}}}}", SolutionName },
                { $"{{{{{nameof(RootNamespace)}}}}}", RootNamespace },
                { $"{{{{{nameof(DryRun)}}}}}", DryRun ? "--dry-run" : "" },
                { $"{{{{{nameof(QuietMode)}}}}}", QuietMode ? "--quiet" : "" },
            }
        }.SetWorkingDirectory(Location);

        return true;
    }
}

﻿using ModdingTools.Core;
using ModdingTools.Core.Options;
using ModdingTools.Core.PatternMatching;

namespace ModdingTools.Templates.Tasks;

/// <summary>
/// Creates an instance of CommandShell to be used in other RunnableTasks.
/// </summary>
public class CommandShellBuilder : RunnableTask
{
    public CommandShell Shell { get; set; } = default!;
    
    protected CommandShellBuilder(AllOptions allOptions) : base(allOptions) { }

    public static CommandShell Create(AllOptions allOptions)
    {
        var instance = new CommandShellBuilder(allOptions);
        instance.Invoke();
        return instance.Shell;
    }

    protected override bool Invoke()
    {
        Shell = new CommandShell()
        {
            DryRun = DryRun,
            QuietMode = QuietMode,
            Tree = new FileTree() { Node = Location },
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
                { $"{{{{{nameof(TemplateShortName)}}}}}", TemplateShortName },
                { $"{{{{{nameof(DryRun)}}}}}", DryRun ? "--dry-run" : "" },
                { $"{{{{{nameof(QuietMode)}}}}}", QuietMode ? "--quiet" : "" },
            }
        }.SetWorkingDirectory(Location);

        return true;
    }
}

﻿using ModdingTools.Core.Options;

namespace ModdingTools.Core;

public abstract class RunnableTask
{
    public DirectoryInfo GamePath { get; }
    public string GameName { get; }
    public string UnityPlayerVersion { get; }
    public string ManagedFrameworkVersion { get; }
    public DirectoryInfo Location { get; }
    public string ProjectName { get; }
    public string AssemblyName { get; }
    public string SolutionName { get; }
    public string RootNamespace { get; }
    public bool DryRun { get; }
    public bool QuietMode { get; }
    
    public DirectoryInfo ManagedAssembliesPath => 
        new (Path.Combine(Path.Combine(GamePath.FullName, GameName + "_Data"), "Managed"));

    protected RunnableTask(AllOptions allOptions)
    {
        (GamePath, GameName, UnityPlayerVersion, ManagedFrameworkVersion) = allOptions.GameOptions;
        (Location, ProjectName, AssemblyName, SolutionName, RootNamespace) = allOptions.TemplateOptions;
        DryRun = allOptions.DryRun;
        QuietMode = allOptions.QuietMode;
    }

    protected abstract bool Invoke();
}

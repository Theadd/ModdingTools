using System.Diagnostics;
using ModdingTools.Core.Extensions;
using ModdingTools.Core.PatternMatching;

namespace ModdingTools.Core;

public class CommandShell
{
    private Dictionary<string, string> WorkingSubstitutionVars { get; } = new();
    
    public static bool DebugMode { get; set; } = false;

    public event Action<CommandShellEntry> OnAction;
    public bool DryRun { get; init; } = false;
    public bool QuietMode { get; init; } = false;
    public DirectoryInfo WorkingDirectory { get; private set; } = new(".");
    public FileTree? Tree { get; init; }
    public int CommandExecutionTimeout { get; set; } = 60000; // In milliseconds
    public Dictionary<string, string> SubstitutionVars
    {
        init => value.ToList().ForEach(pair => AddSubstitutionVar(pair.Key, pair.Value));
    }
    
    public CommandShell SetWorkingDirectory(string nextWorkingDirectory)
    {
        var path = Path.GetFullPath(Substitute(nextWorkingDirectory));
        
        return SetWorkingDirectory(new DirectoryInfo(
            Path.IsPathRooted((string?) path) ? path : Path.Combine(WorkingDirectory.FullName, path)));
    }

    public CommandShell SetWorkingDirectory(DirectoryInfo nextWorkingDirectory)
    {
        var relativePath = Path.GetRelativePath(WorkingDirectory.FullName, nextWorkingDirectory.FullName);
        
        if (WorkingDirectory.FullName.Equals(nextWorkingDirectory.FullName, StringComparison.InvariantCultureIgnoreCase))
            return this;

        if (!nextWorkingDirectory.Exists && !(DryRun && (Tree?.Exists(nextWorkingDirectory) ?? false)))
            Trigger("mkdir", Quote(relativePath), DryRun, nextWorkingDirectory.Create);

        Trigger("cd", Quote(relativePath));
        WorkingDirectory = nextWorkingDirectory;

        return this;
    }

    public void Trigger(string cmdName, string cmdArgs, bool skipExecution, Action actionToExecute)
    {
        Trigger(cmdName, cmdArgs, skipExecution);

        if (!skipExecution)
            actionToExecute.Invoke();
    }

    public void Trigger(string cmdName, string cmdArgs = "", bool skipExecution = false)
    {
        var entry = new CommandShellEntry(cmdName, cmdArgs, skipExecution, DryRun, QuietMode);
        OnAction += NoopShellAction;
        OnAction(entry);
        OnAction -= NoopShellAction;
    }

    public CommandShell GoBack() => Ignore(TryGo("..", true));

    public CommandShell Go(string directoryName, bool createIfNotExists = false) =>
        Ignore(TryGo(directoryName, createIfNotExists));

    public bool TryGo(string directoryName, bool createIfNotExists = false)
    {
        directoryName = Substitute(directoryName);
        var nextWorkingDir = new DirectoryInfo(Path.Combine(WorkingDirectory.FullName, directoryName));

        if (!nextWorkingDir.Exists && !createIfNotExists)
            return false;

        return SetWorkingDirectory(nextWorkingDir) is { } _;
    }

    public CommandShell Exec(string command, bool executeInDryRun = false) => Ignore(TryExec(command, executeInDryRun));

    public bool TryExec(string command, bool executeInDryRun = false)
    {
        var (cmdName, cmdArgs, _) = Substitute(command).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = cmdName,
                Arguments = cmdArgs,
                RedirectStandardOutput = QuietMode,
                RedirectStandardError = QuietMode,
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = WorkingDirectory.FullName
            }
        };
        
        Trigger(cmdName, cmdArgs, DryRun && !executeInDryRun, () =>
        {
            process.Start();
            // process.BeginOutputReadLine();
            process.WaitForExit(CommandExecutionTimeout);
        });
        
        return true; // TODO
    }

    public CommandShell AddSubstitutionVar(string key, string value)
    {
        WorkingSubstitutionVars.Add(key, value);
        return this;
    }

    public string Substitute(string source) =>
        WorkingSubstitutionVars.Aggregate(source, (content, pair) =>
            content.Replace(pair.Key, pair.Value, StringComparison.InvariantCulture));

    private CommandShell Ignore(bool _) => this;

    public static void NoopShellAction(CommandShellEntry _) { }

    public static string Quote(string value, bool quoteAlways = false) =>
        quoteAlways || value.Contains(' ') ? $"\"{value}\"" : value;
    
    public record CommandShellEntry(
        string CommandName, 
        string CommandArgs, 
        bool SkipExecution, 
        bool DryRun,
        bool QuietMode);
}

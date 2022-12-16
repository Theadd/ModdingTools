using System.Diagnostics;
using System.Globalization;
using ModdingTools.Core.Extensions;

namespace ModdingTools.Core;

public class CommandShell
{
    private Dictionary<string, string> WorkingTemplateVars { get; } = new();
    
    public DirectoryInfo WorkingDirectory { get; set; } = new(".");
    public Dictionary<string, string> TemplateVars
    {
        init => value.ToList().ForEach(pair => AddTemplateVar(pair.Key, pair.Value));
    }
    public int CommandExecutionTimeout { get; set; } = 60000;   // In milliseconds

    public CommandShell GoBack() => Ignore(TryGo(".."));
    
    public CommandShell Go(string directoryName, bool createIfNotExists = false) => Ignore(TryGo(directoryName, createIfNotExists));

    public bool TryGo(string directoryName, bool createIfNotExists = false)
    {
        var nextWorkingDir = new DirectoryInfo(Path.Combine(WorkingDirectory.FullName, directoryName));

        if (!nextWorkingDir.Exists && !createIfNotExists)
            return false;
        
        if (!nextWorkingDir.Exists)
            nextWorkingDir.Create();

        WorkingDirectory = nextWorkingDir;
        
        return true;
    }
    
    public CommandShell Exec(string command) => Ignore(TryExec(command));
    
    public bool TryExec(string command)
    {
        var (cmdName, cmdArgs, _) = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = cmdName,
                Arguments = cmdArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = WorkingDirectory.FullName
            }
        };
        
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit(CommandExecutionTimeout);
        
        return true;    // TODO
    }

    public CommandShell AddTemplateVar(string key, string value)
    {
        WorkingTemplateVars.Add("{{" + key + "}}", value);

        return this;
    }

    public string SubstituteVars(string source) => 
        WorkingTemplateVars.Aggregate(source, (content, pair) => 
            content.Replace(pair.Key, pair.Value, StringComparison.InvariantCulture));

    private CommandShell Ignore(bool _) => this;
}

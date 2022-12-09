using System.Diagnostics;
using ModdingTools.Core.Extensions;

namespace ModdingTools.Core;

public class CommandShell
{
    public DirectoryInfo WorkingDirectory { get; set; } = new(".");
    public int CommandExecutionTimeout { get; set; } = 60000;   // In milliseconds

    public bool Exec(string command)
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
}

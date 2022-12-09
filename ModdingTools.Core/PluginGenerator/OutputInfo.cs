namespace ModdingTools.Core.PluginGenerator;

public interface IOutputInfo
{
    public bool IsGenerationComplete { get; set; }
    public string OutputDirectory { get; set; }
}

public class OutputInfo : IOutputInfo
{
    public bool IsGenerationComplete { get; set; } = false;
    public string OutputDirectory { get; set; }
    public string VisualStudioSolutionPath { get; set; }
}

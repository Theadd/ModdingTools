using System.IO;

namespace ModdingTools.Core.PluginGenerator;

public interface IPluginGenerator : ICreateOutput
{
    public ICreatePluginSettings Configuration { get; set; }
    public FileSystemPaths Paths { get; set; }
    DirectoryInfo TempDirectory { get; }
    string TargetPath { get; }
    string SolutionFilePath { get; }
}

public interface ICreateOutput
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>True on success</returns>
    public bool Create();
    
    public IOutputInfo Info { get; set; }
}

public interface ILegacyPluginGenerator
{
}

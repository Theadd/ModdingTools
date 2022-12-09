namespace ModdingTools.Core;

public interface ICSharpProject
{
    string TargetFramework { get; set; }
    string OutputPath { get; set; }
    void AddReference(string dllPath, string priv = "false", bool relative = true);
    void Save(string location);
    void AddResource(string path);
}

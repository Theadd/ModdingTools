namespace ModdingTools.Core.Options;

public class GameOptions
{
    public DirectoryInfo? GamePath { get; set; }
    public string? GameName { get; set; }
    public string? UnityPlayerVersion { get; set; }
    public string? ManagedFrameworkVersion { get; set; }
    
    public (DirectoryInfo, string, string, string) AsTuple() => (GamePath!, GameName!, UnityPlayerVersion!, ManagedFrameworkVersion!);

    public override string ToString() => $"\tGamePath = {GamePath!.FullName}\n\tGameName = {GameName}\n\tUnityPlayerVersion = {UnityPlayerVersion}\n\tManagedFrameworkVersion = {ManagedFrameworkVersion}";
}



namespace ModdingTools.Core.Options;

public class GameOptions
{
    public DirectoryInfo? GamePath { get; set; }
    public string? GameName { get; set; }
    public string? UnityPlayerVersion { get; set; }

    public override string ToString() => $"\tGamePath = {GamePath!.FullName}\n\tGameName = {GameName}\n\tUnityPlayerVersion = {UnityPlayerVersion}";
}



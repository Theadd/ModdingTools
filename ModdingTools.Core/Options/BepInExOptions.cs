namespace ModdingTools.Core.Options;

public record BepInExOptionsRecord(
    DirectoryInfo BepInExPath,
    string BepInExVersion);

public class BepInExOptions
{
    public DirectoryInfo? BepInExPath { get; set; }
    public string? BepInExVersion { get; set; }
    
    public (DirectoryInfo, string) AsTuple() => 
        (BepInExPath!, BepInExVersion!);

    public BepInExOptionsRecord AsRecord() =>
        new (BepInExPath!, BepInExVersion!);

    public override string ToString() => $"\tBepInExPath = {BepInExPath?.FullName}\n\tBepInExVersion = {BepInExVersion}";
}

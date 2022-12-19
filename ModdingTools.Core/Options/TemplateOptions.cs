namespace ModdingTools.Core.Options;

public record TemplateOptionsRecord(
    DirectoryInfo Location, 
    string ProjectName, 
    string AssemblyName,
    string SolutionName,
    string RootNamespace,
    string TemplateShortName);

public class TemplateOptions
{
    public DirectoryInfo? Location { get; set; }
    public string? ProjectName { get; set; }
    public string? AssemblyName { get; set; }
    public string? SolutionName { get; set; }
    public string? RootNamespace { get; set; }
    public string? TemplateShortName { get; set; }

    public (DirectoryInfo, string, string, string, string, string) AsTuple() =>
        (Location!, ProjectName!, AssemblyName!, SolutionName!, RootNamespace!, TemplateShortName!);
    
    public TemplateOptionsRecord AsRecord() =>
        new (Location!, ProjectName!, AssemblyName!, SolutionName!, RootNamespace!, TemplateShortName!);

    public override string ToString() =>
        $"\tLocation = {Location!.FullName}\n\tProjectName = {ProjectName}\n\tAssemblyName = {AssemblyName}\n\tSolutionName = {SolutionName}\n\tRootNamespace = {RootNamespace}\n\tTemplateShortName = {TemplateShortName}";
}

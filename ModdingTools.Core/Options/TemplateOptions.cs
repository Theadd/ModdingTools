namespace ModdingTools.Core.Options;

public class TemplateOptions
{
    public DirectoryInfo? Location { get; set; }
    public string? ProjectName { get; set; }
    public string? AssemblyName { get; set; }
    public string? SolutionName { get; set; }
    public string? RootNamespace { get; set; }

    public override string ToString() =>
        $"\tLocation = {Location!.FullName}\n\tProjectName = {ProjectName}\n\tAssemblyName = {AssemblyName}\n\tSolutionName = {SolutionName}\n\tRootNamespace = {RootNamespace}";
}

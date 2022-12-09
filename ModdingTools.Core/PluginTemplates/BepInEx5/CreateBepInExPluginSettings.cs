using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ModdingTools.Core.PluginGenerator;

namespace ModdingTools.Core.PluginTemplates.BepInEx5;

public class CreateBepInExPluginSettings : ICreateBepInExPluginSettings
{
    public static CreateBepInExPluginSettings Empty = new CreateBepInExPluginSettings();

    public string SolutionName { get; set; }
    public string ProjectName { get; set; }
    public string AssemblyName { get; set; }
    public string RootNamespace { get; set; }
    public string Location { get; set; }
    public bool IsMultiProjectSolution { get; set; }

    public ICreatePluginSettings AutoComplete() =>
        new CreateBepInExPluginSettings()
        {
            SolutionName = SolutionName,
            ProjectName = ProjectName,
            AssemblyName = AssemblyName,
            RootNamespace = RootNamespace,
            IsMultiProjectSolution = IsMultiProjectSolution,
            Location = ((Location?.Length ?? 0) > 0
                ? Location : GetDefaultProjectsLocation())!
        };

    private bool HasAnyInvalidCharOrEmpty(string name) =>
        name.Length == 0 || !char.IsLetter(name[0]) || name.IndexOfAny(AbstractPluginGenerator.InvalidChars) != -1;

    /// <summary>
    /// Check if provided values are valid for they type and gives back a list of invalid property names.
    /// Also returns false with an empty list of invalid property names when the target directory already
    /// exists in order to prompt the user for overwriting it.
    /// </summary>
    /// <param name="invalids"></param>
    /// <returns></returns>
    public bool Validate(ref List<string> invalids)
    {
        if (HasAnyInvalidCharOrEmpty(SolutionName)) invalids.Add(nameof(SolutionName));
        if (HasAnyInvalidCharOrEmpty(ProjectName)) invalids.Add(nameof(ProjectName));
        if (HasAnyInvalidCharOrEmpty(AssemblyName)) invalids.Add(nameof(AssemblyName));
        if (HasAnyInvalidCharOrEmpty(RootNamespace)) invalids.Add(nameof(RootNamespace));

        if (!LocalDevice.IsValid(Location)) invalids.Add(nameof(Location));

        if (invalids.Count == 0 && new DirectoryInfo(GetTargetPath()).Exists)
            return false;

        return invalids.Count == 0;
    }

    public string GetTargetPath() => Path.GetFullPath(Path.Combine(Location, SolutionName));

    private static string GetDefaultProjectsLocation() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? Path.Combine(new string[]
                { Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Documents" })
            : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
}

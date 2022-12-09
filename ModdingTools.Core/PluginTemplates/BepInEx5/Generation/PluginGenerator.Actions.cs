using System;
using System.IO;
using System.Linq;
using ModdingTools.Core.Utils;

namespace ModdingTools.Core.PluginTemplates.BepInEx5;

public partial class BepInEx5PluginGenerator
{
    private bool WriteToFile(string relativePath, string content)
    {
        using var writer = new StreamWriter(Path.Combine(TargetPath, relativePath));
        writer.Write(content);

        return true;
    }

    private bool CreateTemplateResources()
    {
        if (!GetListOfResources.All(res =>
                SafeInvoke.TryInvoke(() => WriteToFile(res.RelativePath, res.Content))))
            throw new Exception("Unexpected exception while writing the template files to disk.");

        return true;
    }

    private bool CreateBasicDirectoryStructure()
    {
        var targetDirectory = new DirectoryInfo(TargetPath);

        if (!GetListOfDirectoriesToCreate.All(dir =>
                SafeInvoke.TryInvoke(() => targetDirectory.CreateSubdirectory(dir))))
            throw new Exception("Unexpected exception while creating the structure of directories.");

        return true;
    }

    private bool CreateCSharpProjectFile(string pathToDlls, Func<string, bool> addReferenceFilter)
    {
        var projFile = new BepInEx5CSharpProjectFile() { Generator = this };
        var csprojPath = Path.Combine(TargetPath,
            $"{RelativePathToProjectDirectory}{Slash}{Config.ProjectName}.csproj");

        var libraries = new DirectoryInfo(pathToDlls)
            .EnumerateFiles("*.dll")
            .Where(lib => addReferenceFilter(lib.Name));

        foreach (var lib in libraries)
            projFile.AddReference(lib.FullName);

        projFile.Save(csprojPath);

        return true;
    }

    private bool CreateVisualStudioSolutionFile(string slnAbsPath)
    {
        // VisualStudioSolutionFileAutoCreator.Create(Path.GetDirectoryName(slnAbsPath));
        //return true;


        var slnCreator = new SolutionCreator(Config.SolutionName);
        var slnFileAbsPath = Path.GetFullPath(slnAbsPath);
        var slnDirAbsPath = Path.GetDirectoryName(slnAbsPath);
        var csprojAbsPath = Path.GetFullPath(
            Path.Combine(TargetPath, RelativePathToProjectDirectory, $"{Config.ProjectName}.csproj"));
        var csprojRelPath = Path.GetRelativePath(slnDirAbsPath, csprojAbsPath);

        // slnCreator.AddProject(csprojRelPath);
        slnCreator.AddProject(Config.ProjectName);
        slnCreator.Save(slnFileAbsPath);

        return true;
    }
}

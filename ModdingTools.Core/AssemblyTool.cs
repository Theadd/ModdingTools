using AsmResolver;
using AsmResolver.DotNet;
using AsmResolver.DotNet.Serialized;
using AsmResolver.PE;
using BepInEx.AssemblyPublicizer;

namespace ModdingTools.Core;

public class AssemblyTool
{
    public DirectoryInfo Destination { get; set; } = new DirectoryInfo(".");

    public bool TryWriteAsPublic(FileInfo assemblyFile)
    {
        WriteAsPublic(assemblyFile);

        return true;
    }

    public AssemblyTool WriteAsPublic(FileInfo assemblyFile)
    {
        AssemblyPublicizer.Publicize(
            assemblyFile.FullName, 
            Path.Combine(Destination.FullName, assemblyFile.Name));

        return this;
    }
}

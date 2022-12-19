using AsmResolver.DotNet;
using AsmResolver.PE;

namespace ModdingTools.Core.Extensions;

public static class FileInfoEx
{
    public static AssemblyDefinition GetAssemblyDefinition(this FileInfo file) => 
        AssemblyDefinition.FromImage(PEImage.FromFile(file.FullName));

    public static IEnumerable<AssemblyReference> GetAssemblyReferences(this FileInfo file) =>
        SafeInvoke.Invoke<IEnumerable<AssemblyReference>>(() =>
            file.GetAssemblyDefinition().Modules.SelectMany(m => m.AssemblyReferences)) 
        ?? Array.Empty<AssemblyReference>();

    public static FileInfo? GetFile(this DirectoryInfo self, string filename) =>
        self.GetFiles(filename).FirstOrDefault(default(FileInfo));
    
    public static DirectoryInfo? GetDirectory(this DirectoryInfo self, string directoryName) =>
        self.GetDirectories(directoryName).FirstOrDefault(default(DirectoryInfo));
}

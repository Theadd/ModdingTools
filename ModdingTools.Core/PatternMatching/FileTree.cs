using ModdingTools.Core.Extensions;
using Newtonsoft.Json;

namespace ModdingTools.Core.PatternMatching;

public interface IFileTree<T>
{
    public IEnumerable<T> Children { get; set; }
    public T? Parent { get; init; }
}

[JsonObject(MemberSerialization.OptIn)]
public class FileTree : IFileTree<FileTree>
{
    [JsonProperty]
    public string Name => Node.Name;
    
    [JsonProperty]
    public IEnumerable<FileTree> Children { get; set; } = Enumerable.Empty<FileTree>();
    public FileTree? Parent { get; init; } = default;
    public FileSystemInfo Node { get; init; } = null!;
    

    public bool ShouldSerializeChildren() => Children.Any();
    public FileTree Write(FileInfo fileInfo) => Write(fileInfo, 0);
    
    protected FileTree Write(FileInfo fileInfo, int depth)
    {
        var (nextNode, rest) = GetNextRelativeNode(fileInfo);

        return string.IsNullOrWhiteSpace(rest) 
            ? Add(new FileTree() { Node = fileInfo, Parent = this }) 
            : (Children.FirstOrDefault(c => c.Name == nextNode) 
               ?? Add(
                   new FileTree()
                   {
                       Node = new DirectoryInfo(Path.Combine(Node.FullName, nextNode)), 
                       Parent = this
                   })
               ).Write(fileInfo, depth + 1);
    }

    public bool Exists(FileSystemInfo fileInfo)
    {
        var (nextNode, rest) = GetNextRelativeNode(fileInfo);

        return string.IsNullOrWhiteSpace(rest)
            ? fileInfo.FullName.Equals(Node.FullName, StringComparison.InvariantCultureIgnoreCase) 
              || Children.Any(c => c.Name == nextNode)
            : Children.FirstOrDefault(c => c.Name == nextNode)?.Exists(fileInfo) ?? false;
    }

    protected (string NextNode, string Other) GetNextRelativeNode(FileSystemInfo fileInfo)
    {
        var relativePath = Path.GetRelativePath(Node.FullName, fileInfo.FullName);
        var (nextNode, rest, _) = relativePath.Split(Path.DirectorySeparatorChar, 2, StringSplitOptions.RemoveEmptyEntries);
        
        return (nextNode, rest);
    }

    protected FileTree Add(FileTree child)
    {
        Children = Children.Add(child);
        return child;
    }

    public override string ToString() =>
        Children.Any()
            ? $"\"{Name}\": {{ {string.Join(", ", Children.Select(child => child.ToString()))} }}"
            : $"\"{Name}\"";
}

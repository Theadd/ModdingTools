using System.Reflection;

namespace ModdingTools.Templates;

public class ResourceManager
{
    private string[] resNames;
    public static ResourceManager Instance = new ResourceManager();

    private ResourceManager()
    {
        Assembly = GetType().Assembly;
        resNames = Assembly.GetManifestResourceNames();
    }

    public Assembly Assembly { get; }

    public Stream GetResource(string filename) => Assembly.GetManifestResourceStream(
        resNames.First(
            x => x.EndsWith(filename)))!;
}

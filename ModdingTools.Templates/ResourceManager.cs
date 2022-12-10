using System.Reflection;

namespace ModdingTools.Templates;

public class ResourceManager
{
    private string[] resNames;
    public static ResourceManager Instance = new ResourceManager();

    private ResourceManager()
    {
        this.Assembly = this.GetType().Assembly;
        this.resNames = this.Assembly.GetManifestResourceNames();
    }

    public Assembly Assembly { get; }

    public Stream GetResource(string filename) => this.Assembly.GetManifestResourceStream(
        ((IEnumerable<string>) this.resNames).First<string>(
            (Func<string, bool>) (x => x.EndsWith(filename))));
}

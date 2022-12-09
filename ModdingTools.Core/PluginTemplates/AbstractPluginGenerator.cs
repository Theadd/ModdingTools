using System.IO;
using System.Linq;
using ModdingTools.Core.PluginGenerator;

namespace ModdingTools.Core.PluginTemplates;

public abstract class AbstractPluginGenerator : IPluginGenerator
{
    public virtual ICreatePluginSettings Configuration { get; set; }

    public virtual FileSystemPaths Paths { get; set; }

    public virtual DirectoryInfo TempDirectory { get; protected set; }

    public virtual string TargetPath { get; protected set; }
    
    public virtual string SolutionFilePath { get; protected set; }


    private static char[] _invalidChars;

    public static char[] InvalidChars =>
        _invalidChars != null
            ? _invalidChars
            : (_invalidChars = Path.GetInvalidFileNameChars()
                .Union(Path.GetInvalidPathChars())
                .Union(new char[] { '\\', '/', '(', ')', '[', ']', '{', '}', '-', '+' })
                .ToArray());

    public AbstractPluginGenerator()
    {
        Info = new OutputInfo();
    }

    public abstract bool OnCreate();

    public virtual bool Create()
    {
        var target = new DirectoryInfo(Configuration.GetTargetPath());

        if (target.Exists)
            LocalDevice.TryDeleteDirectoryAndAllContents(target, 50);

        target.Create();
        TargetPath = target.FullName;

        TempDirectory = this.GetExclusiveTempDirectory();

        return OnCreate();
    }

    public IOutputInfo Info { get; set; }
}

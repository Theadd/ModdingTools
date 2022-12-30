using ModdingTools.Core;

namespace ModdingTools.Templates.Extensions;

public static class CommandShellEx
{
    public static CommandShell Write(this CommandShell self, string resourceFile) => 
        self.Write(resourceFile, resourceFile);

    public static CommandShell Write(this CommandShell self, string resourceFile, string targetFilename)
    {
        var targetFile = new FileInfo(Path.Combine(self.WorkingDirectory.FullName, targetFilename));
        self.Tree?.Write(targetFile);
        self.Trigger("Write", CommandShell.Quote(targetFilename), self.DryRun, () =>
        {
            TemplateResource
                .From(resourceFile)
                .Write(targetFile);
        });
        
        return self;
    }

    public static CommandShell WriteTemplate(this CommandShell self, string resourceFile) => self.WriteTemplate(resourceFile, resourceFile);

    public static CommandShell WriteTemplate(this CommandShell self, string resourceFile, string targetFilename)
    {
        var targetFile = new FileInfo(Path.Combine(self.WorkingDirectory.FullName, targetFilename));
        self.Tree?.Write(targetFile);
        self.Trigger("Write", CommandShell.Quote(targetFilename), self.DryRun, () =>
        {
            TemplateResource
                .From(resourceFile)
                .Write(targetFile, self.Substitute);
        });
        
        return self;
    }
    
    /// <summary>
    /// Unzip contents from a zip file in the Resources folder to CommandShell's current WorkingDirectory.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="resourceFile"></param>
    /// <returns></returns>
    public static CommandShell Unzip(this CommandShell self, string resourceFile)
    {
        self.Trigger("Unzip", CommandShell.Quote(resourceFile), self.DryRun, () =>
        {
            TemplateResource
                .From(resourceFile)
                .UnZip(self.WorkingDirectory, self.Substitute);
        });
        
        return self;
    }
}

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
}

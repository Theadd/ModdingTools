using ModdingTools.Core;

namespace ModdingTools.Templates.Extensions;

public static class CommandShellEx
{
    public static CommandShell Write(this CommandShell self, string resourceFile) => 
        self.Write(resourceFile, resourceFile);

    public static CommandShell Write(this CommandShell self, string resourceFile, string targetFilename)
    {
        TemplateResource
            .From(resourceFile)
            .Write(new FileInfo(Path.Combine(self.WorkingDirectory.FullName, targetFilename)));

        return self;
    }

    public static CommandShell WriteTemplate(this CommandShell self, string resourceFile) => self.WriteTemplate(resourceFile, resourceFile);

    public static CommandShell WriteTemplate(this CommandShell self, string resourceFile, string targetFilename)
    {
        TemplateResource
            .From(resourceFile)
            .Write(
                new FileInfo(Path.Combine(self.WorkingDirectory.FullName, targetFilename)),
                self.Substitute);

        return self;
    }
}

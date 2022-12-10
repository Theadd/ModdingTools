using ModdingTools.Core;

namespace ModdingTools.Templates.Extensions;

public static class CommandShellEx
{
    public static CommandShell Write(this CommandShell self, string resourceFile)
    {
        TemplateResource
            .From(resourceFile)
            .Write(new FileInfo(Path.Combine(self.WorkingDirectory.FullName, resourceFile)));

        return self;
    }
}

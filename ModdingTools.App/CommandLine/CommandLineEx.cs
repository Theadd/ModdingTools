using System.CommandLine;

namespace ModdingTools.App.CommandLine;

/// <summary>
/// System.CommandLine related extension methods.
/// </summary>
public static class CommandLineEx
{
    public static Argument<DirectoryInfo?> Hide(this Argument<DirectoryInfo?> self)
    {
        self.IsHidden = true;
        return self;
    }
    
    public static Argument<string?> Hide(this Argument<string?> self)
    {
        self.IsHidden = true;
        return self;
    }
    
    public static Option<DirectoryInfo?> Hide(this Option<DirectoryInfo?> self)
    {
        self.IsHidden = true;
        return self;
    }
    
    public static Option<string?> Hide(this Option<string?> self)
    {
        self.IsHidden = true;
        return self;
    }
}

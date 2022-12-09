using System.CommandLine;

namespace ModdingTools.App.CommandLine;

public static class CommandLineArgs
{
    public static Option<DirectoryInfo?> GameOption { get; } = new(
        name: "--game",
        description: "Path to local directory where the game files are located.",
        parseArgument: result =>
        {
            var gamePath = result.Tokens.Single().Value;

            if (!Directory.Exists(gamePath))
                result.ErrorMessage = "Game path does not exist";
            else if (CommandLineUtils.GetGameName(new DirectoryInfo(gamePath)).Length == 0)
                result.ErrorMessage =
                    "Game path does not follow the expected directory structure for a unity game.";
            else
                return new DirectoryInfo(gamePath);

            return null;
        }
    ) {
        Arity = ArgumentArity.ExactlyOne,
        IsRequired = true
    };
    
    public static Option<bool> DryRunOption { get; } = new(
        name: "--dry-run",
        description: "Don't actually perform the operation, just check for valid arguments and provide descriptive error messages if any.",
        getDefaultValue: () => false
    );
    
    public static Argument<DirectoryInfo?> EmptyDirectoryArgument { get; } = new(
        name: "directory",
        description:
        "If you provide a directory, the command is run inside it. If this directory already exists, it must be an empty directory.",
        isDefault: true,
        parse: result =>
        {
            DirectoryInfo directory =
                new DirectoryInfo(result.Tokens.Count == 0 ? "." : result.Tokens.Single().Value);

            if (directory.Exists &&
                (directory.GetDirectories().Length != 0 || directory.GetFiles().Length != 0))
            {
                result.ErrorMessage = "Target directory must be an empty directory.";
                return null;
            }

            return directory;
        }
    );
    
    public static Option<string?> InitialProjectNameOption { get; } = new(
        name: "--name",
        description:
        "Just a name for an initial C# project to begin working with. [default: Inherits from <directory>]"
    )
    {
        Arity = ArgumentArity.ExactlyOne
    };
    
    public static Option<string?> AssemblyNameOption { get; } = new(
        name: "--assembly-name",
        description: "Project assembly name."
    );
}

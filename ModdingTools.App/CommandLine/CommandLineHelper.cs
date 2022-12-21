using ModdingTools.Core;

namespace ModdingTools.App.CommandLine;

public class CommandLineHelper
{
    private static IEnumerable<ConsoleColor> ColorPaletteA = new[]
        { ConsoleColor.DarkGray, ConsoleColor.Green, ConsoleColor.White };
    private static IEnumerable<ConsoleColor> ColorPaletteB = new[]
        { ConsoleColor.DarkGray, ConsoleColor.Green, ConsoleColor.Yellow };

    public static void DisplayCommandShellEntry(CommandShell.CommandShellEntry entry)
    {
        if (entry.QuietMode) return;

        var palette = (entry.CommandName is "Write" or "dotnet" or "Unzip" ? ColorPaletteB : ColorPaletteA).ToArray();

        Console.ForegroundColor = palette.ElementAt(0);
        Console.Write("$ ");
        Console.ForegroundColor = palette.ElementAt(1);
        Console.Write($"{entry.CommandName}");
        Console.ForegroundColor = palette.ElementAt(2);
        Console.WriteLine($" {entry.CommandArgs}");
        Console.ResetColor();
    }
}

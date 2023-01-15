using System.CommandLine;
using System.CommandLine.Binding;
using ModdingTools.Core.Extensions;
using ModdingTools.Core.Options;

namespace ModdingTools.App.Binding;

public class BepInExOptionsBinder : BinderBase<BepInExOptions>
{
    private readonly Option<DirectoryInfo?> _gamePath;
    private readonly Argument<DirectoryInfo?> _targetPath;

    public BepInExOptionsBinder(Option<DirectoryInfo?> gamePath, Argument<DirectoryInfo?> targetPath)
    {
        _gamePath = gamePath;
        _targetPath = targetPath;
    }

    protected override BepInExOptions GetBoundValue(BindingContext bindingContext)
    {
        DirectoryInfo? bepInExDirectory;
        var isValid = false;
        var isValidBepInEx = false;
        var bepInExVersion = "";
        var gameName = "";

        var dir = bindingContext.ParseResult.GetValueForOption(_gamePath);

        if (dir != null)
        {
            gameName = BindingOptionsHelper.GetGameName(dir);
            if (!string.IsNullOrWhiteSpace(gameName))
                isValid = true;
        }
        
        if (!isValid)
        {
            var workingPath = bindingContext.ParseResult.GetValueForArgument(_targetPath);
            
            if (workingPath != null)
            {
                var auxGamePath = BindingOptionsHelper.GetGamePathFromConfigurationFilesFrom(workingPath);
                if (auxGamePath != null)
                {
                    dir = auxGamePath;
                    isValid = true;
                }
            }
        }

        bepInExDirectory = dir?.GetDirectory("BepInEx") ?? null;
        
        if (isValid)
        {
            if (bepInExDirectory != null)
            {
                bepInExVersion =
                    BindingOptionsHelper.GetBepInExVersion(bepInExDirectory.GetDirectory("core"));
                isValidBepInEx = !string.IsNullOrWhiteSpace(bepInExVersion);
            }
        }

        if (isValidBepInEx)
        {
            return new BepInExOptions
            {
                BepInExPath = bepInExDirectory,
                BepInExVersion = bepInExVersion
            };
        }
        else
        {
            return new BepInExOptions
            {
                BepInExPath = bepInExDirectory,
                BepInExVersion = bepInExVersion
            };
        }
        
    }
}

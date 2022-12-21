using System.CommandLine;
using System.CommandLine.Binding;
using ModdingTools.Core.Options;

namespace ModdingTools.App.Binding;

public class GameOptionsBinder : BinderBase<GameOptions>
{
    private readonly Option<DirectoryInfo?> _gamePath;
    private readonly Argument<DirectoryInfo?> _targetPath;

    public GameOptionsBinder(Option<DirectoryInfo?> gamePath, Argument<DirectoryInfo?> targetPath)
    {
        _gamePath = gamePath;
        _targetPath = targetPath;
    }

    protected override GameOptions GetBoundValue(BindingContext bindingContext)
    {
        var isValid = false;
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
                    gameName = BindingOptionsHelper.GetGameName(dir);
                }
            }
        }

        return new GameOptions
        {
            GamePath = dir,
            GameName = gameName,
            UnityPlayerVersion = BindingOptionsHelper.GetUnityPlayerVersion(dir),
            ManagedFrameworkVersion = BindingOptionsHelper.GetManagedFrameworkVersion(dir, gameName)
        };
    }
}

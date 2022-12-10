﻿using System.CommandLine;
using System.CommandLine.Binding;
using ModdingTools.Core.Options;

namespace ModdingTools.App.Binding;

public class GameOptionsBinder : BinderBase<GameOptions>
{
    private readonly Option<DirectoryInfo> _gamePath;

    public GameOptionsBinder(Option<DirectoryInfo?> gamePath)
    {
        _gamePath = gamePath!;
    }

    protected override GameOptions GetBoundValue(BindingContext bindingContext)
    {
        var dir = bindingContext.ParseResult.GetValueForOption(_gamePath)!;

        return new GameOptions
        {
            GamePath = dir,
            GameName = BindingOptionsHelper.GetGameName(dir),
            UnityPlayerVersion = BindingOptionsHelper.GetUnityPlayerVersion(dir)
        };
    }
}
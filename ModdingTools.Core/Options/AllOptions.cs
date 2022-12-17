namespace ModdingTools.Core.Options;

public record AllOptions(GameOptionsRecord GameOptions, TemplateOptionsRecord TemplateOptions, bool DryRun, bool QuietMode);

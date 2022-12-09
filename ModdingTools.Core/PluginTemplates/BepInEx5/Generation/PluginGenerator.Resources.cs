using System;
using System.IO;

namespace ModdingTools.Core.PluginTemplates.BepInEx5;

public partial class BepInEx5PluginGenerator
{
    private char Slash => Path.DirectorySeparatorChar;

    private string[] GetListOfDirectoriesToCreate => Config.IsMultiProjectSolution
        ? new[]
        {
            "lib",
            $"lib{Slash}References",
            "src",
            $"src{Slash}{Config.ProjectName}",
        }
        : new []
        {
            "lib",
            $"lib{Slash}References",
            $"{Config.ProjectName}",
            $"{Config.ProjectName}{Slash}src",
        };

    private string RelativePathToProjectDirectory => Config.IsMultiProjectSolution
        ? $"src/{Config.ProjectName}"
        : $"{Config.ProjectName}";
    
    private string RelativePathToSolutionFile => Config.IsMultiProjectSolution
        ? $"src/{Config.SolutionName}.sln"
        : $"{RelativePathToProjectDirectory}{Slash}{Config.SolutionName}.sln";

    private TemplateResource[] GetListOfResources => new TemplateResource[]
    {
        new($"{RelativePathToProjectDirectory}{Slash}NuGet.Config", GetNugetConfig),
        new($"{RelativePathToProjectDirectory}{Slash}Plugin.cs", GetMinimalPluginCS),
        new($"{RelativePathToProjectDirectory}{Slash}Directory.Build.props", GetDirectoryBuildProps),
        new(".gitignore", GitIgnoreResource.Content),
    };

    private string GetNugetConfig => @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <config>
    <add key=""dependencyVersion"" value=""Lowest"" />
  </config>

  <packageRestore>
    <!-- Allow NuGet to download missing packages -->
    <add key=""enabled"" value=""True"" />

    <!-- Automatically check for missing packages during build in Visual Studio -->
    <add key=""automatic"" value=""True"" />
  </packageRestore>

  <packageSources>
    <add key=""nuget.org"" value=""https://api.nuget.org/v3/index.json"" protocolVersion=""3"" />
    <add key=""BepInEx"" value=""https://nuget.bepinex.dev/v3/index.json"" />
  </packageSources>
</configuration>
";

    private string GetMinimalPluginCS => $@"using BepInEx;

namespace {Config.RootNamespace}
{{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {{
        private void Awake()
        {{
            // Plugin startup logic here
        }}

        private void OnDestroy()
        {{
            Destroy(gameObject);
        }}
    }}
}}
";

    private string GetDirectoryBuildProps => @$"<Project>
  <PropertyGroup>
    <!-- 
      Full path to Humankind installation directory, ex: C:\Program Files\Steam\steamapps\common\Humankind
    -->
    <HUMANKIND_GAME_PATH>{Paths.GamePath.TrimEnd(new[] { '/', '\\' })}</HUMANKIND_GAME_PATH>
  </PropertyGroup>
</Project>
";
}

public struct TemplateResource
{
    public string RelativePath { get; set; }
    public string Content { get; set; }

    public TemplateResource(string relativePath, string content)
    {
        RelativePath = relativePath;
        Content = content;
    }
}

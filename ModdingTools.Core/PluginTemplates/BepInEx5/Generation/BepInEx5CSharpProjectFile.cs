using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModdingTools.Core.PluginGenerator.CSharpProjectFile;

namespace ModdingTools.Core.PluginTemplates.BepInEx5;

public class BepInEx5CSharpProjectFile : AbstractCSharpProjectFile
{
    public BepInEx5PluginGenerator Generator { get; set; }

    public override string TargetFramework { get; set; }
    public override string OutputPath { get; set; }

    // public Func<string, bool> AddReferenceFilter { get; set; } = (_) => true;

    public bool IncludeCopyPluginToBepInEx { get; set; } = true;
    public bool IsCompatibleWithScriptEngine { get; set; } = true;

    public List<CSProjReference> References { get; set; } = new List<CSProjReference>();

    public List<CSProjResource> Resources { get; set; } = new List<CSProjResource>();

    // Shortcuts

    private char Slash => Path.DirectorySeparatorChar;

    //
    private XmlAttribute SdkAttribute => xA("Sdk", "Microsoft.NET.Sdk");

    public override void Save(string location)
    {
        var csproj =
            XmlDocument("Project", new object[]
            {
                SdkAttribute,
                GetMainProperties,
                (XmlComment) "Disable annoying compile time warnings.",
                GetNoWarnProperties,
                GetReleaseConfiguration,
                GetDebugConfiguration,
                (XmlComment) "NuGet referenced packages.",
                GetNuGetPackageReferences,
                GetNetFrameworkPackageReference,
                IncludeCopyPluginToBepInEx
                    ? new object[]
                    {
                        GetPostBuildTarget,
                        (XmlComment)
                        "PostBuild task to copy output binaries to BepInEx plugins or scripts directory.",
                        GetPostBuildCopyPluginTarget
                    }
                    : null,
                (XmlComment) "References to other game libraries not provided by nuget packages above.",
                GetReferences(Path.GetDirectoryName(location)),
            });
        
        csproj.Save(location);
    }

    private XmlElement GetMainProperties => xE("PropertyGroup",
        xE("TargetFramework", "net471"),
        xE("Configuration", xA("Condition", " '$(Configuration)' == '' "), "Release"),
        xE("Platform", xA("Condition", " '$(Platform)' == '' "), "AnyCPU"),
        xE("AssemblyName", Generator.Config.AssemblyName),
        xE("Product", Generator.Config.ProjectName),
        xE("Description", "Humankind Game Mod - BepInEx Plugin"),
        xE("Version", "1.0.0"),
        xE("AllowUnsafeBlocks", "true"),
        xE("LangVersion", "latest"),
        xE("OutputType", "Library"),
        xE("Deterministic", "true"),
        xE("DefineConstants", ""),
        xE("IsCpp", "false"),
        xE("DebugSymbols", "false"),
        xE("DebugType", "portable"),
        xE("Optimize", "true"),
        xE("ErrorReport", "none"),
        xE("WarningLevel", "4"),
        xE("PlatformTarget", "x64"),
        xE("Prefer32Bit", "false"),
        xE("RootNamespace", Generator.Config.RootNamespace),
        xE("Nullable", "disable"),
        xE("ImplicitUsings", "disable"),
        xE("PackageLicenseExpression", "Unlicense"),
        xE("ProduceReferenceAssembly", "False"),
        xE("Platforms", "AnyCPU"),
        xE("Configurations", "Debug;Release"),
        xE("AssemblyVersion", "1.0.0"),
        xE("FileVersion", "1.0.0"));

    private XmlElement GetNoWarnProperties => xE("PropertyGroup", xE("NoWarn", "$(NoWarn);CS1591"));

    private XmlElement GetReleaseConfiguration => xE("PropertyGroup",
        xA("Condition", " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "),
        xE("OutputPath", $"..{Slash}..{Slash}Release{Slash}{Generator.Config.ProjectName}{Slash}"),
        xE("Prefer32Bits", "false"),
        xE("IsCpp", "false"),
        xE("DocumentationFile",
            $"..{Slash}..{Slash}Release{Slash}{Generator.Config.ProjectName}{Slash}{Generator.Config.AssemblyName}.xml"),
        xE("DebugSymbols", "false"),
        xE("DebugType", "none"),
        xE("Optimize", "true"));

    private XmlElement GetDebugConfiguration => xE("PropertyGroup",
        xA("Condition", " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "),
        xE("DebugSymbols", "true"),
        xE("DebugType", "full"),
        xE("Optimize", "false"));

    private XmlElement GetNuGetPackageReferences => xE("ItemGroup",
        xE("PackageReference", xA("Include", "BepInEx.Analyzers"), xA("Version", "1.0.8"),
            xA("PrivateAssets", "all")),
        xE("PackageReference", xA("Include", "BepInEx.Core"), xA("Version", "5.*")),
        xE("PackageReference", xA("Include", "BepInEx.PluginInfoProps"), xA("Version", "1.1.0")),
        xE("PackageReference", xA("Include", "UnityEngine.Modules"), xA("Version", "2020.3.24"),
            xA("IncludeAssets", "compile")));

    private XmlElement GetNetFrameworkPackageReference => xE("ItemGroup",
        xA("Condition", "'$(TargetFramework.TrimEnd(`0123456789`))' == 'net'"),
        xE("PackageReference", xA("Include", "Microsoft.NETFramework.ReferenceAssemblies"),
            xA("Version", "1.0.2"), xA("PrivateAssets", "all")));

    private XmlElement GetPostBuildTarget => xE("Target", xA("Name", "PostBuild"),
        xA("AfterTargets", "PostBuildEvent"),
        xE("Message", xA("Text", $"GAME PATH = {Generator.Paths.GamePath}")));

    private XmlElement GetPostBuildCopyPluginTarget =>
        xE("Target",
            xA("Name", "PostBuildCopyPlugin"),
            xA("AfterTargets", "PostBuild"),
            xA("Condition", "'$(HUMANKIND_GAME_PATH)' != ''"),
            xE("ItemGroup",
                xE("FilesToCopy", xA("Include", "$(TargetPath)"))),
            xE("Copy", xA("SourceFiles", "@(FilesToCopy)"),
                IsCompatibleWithScriptEngine
                    ? xA("DestinationFolder", $"$(HUMANKIND_GAME_PATH){Slash}BepInEx{Slash}scripts{Slash}")
                    : xA("DestinationFolder", $"$(HUMANKIND_GAME_PATH){Slash}BepInEx{Slash}plugins{Slash}")));

    private XmlElement GetReferences(string relativeTo)
    {
        var xNodeReferences = References.Select(r =>
            xE("Reference", xA("Include", r.Name),
                xE("HintPath", r.Relative ? Path.GetRelativePath(relativeTo, r.HintPath) : r.HintPath),
                xE("Private", "false")));
        return xE("ItemGroup", xNodeReferences);
    }

    public override void AddReference(string dllPath, string priv = "false", bool relative = true)
    {
        Console.WriteLine($"    +REF#={dllPath}");
        References.Add(new CSProjReference
        {
            Name = Path.GetFileNameWithoutExtension(dllPath),
            HintPath = dllPath,
            Private = priv,
            Relative = relative
        });
    }

    public override void AddResource(string path)
    {
        throw new System.NotImplementedException();
    }
}

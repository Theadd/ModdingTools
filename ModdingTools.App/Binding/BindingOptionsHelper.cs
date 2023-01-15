using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ModdingTools.Core.Extensions;

namespace ModdingTools.App.Binding;

public static class BindingOptionsHelper
{
    public static string GetGameName(DirectoryInfo location)
    {
        var isValidDirectory = false;
        var gameName = "";
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            DirectoryInfo[] dataDirs = location.GetDirectories("*_Data");

            foreach (DirectoryInfo d in dataDirs)
            {
                gameName = d.Name[..^5];
                
                if (d.GetDirectories("Managed").Length == 0)
                    continue;

                if (GetGameExecutable(location, gameName) == default(FileInfo))
                    continue;
                
                isValidDirectory = true;
                break;
            }
        }
        else
        {
            // TODO: OSPlatform.OSX
        }

        return isValidDirectory ? gameName : "";
    }

    private static FileInfo? GetGameExecutable(DirectoryInfo location, string gameName) =>
        location
            .GetFiles(gameName + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""))
            .FirstOrDefault(default(FileInfo));

    public static string GetUnityPlayerVersion(DirectoryInfo? location)
    {
        // UnityEngine.Modules v5.6.0 will be used if no version is found.
        var version = "5.6.0";
        if (location == null) return version;
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var unityPlayer = location.GetFiles("UnityPlayer*").FirstOrDefault(
                file => file != null && FileVersionInfo.GetVersionInfo(file.FullName).FileVersion?[..2] == "20",
                null);
            
            if (unityPlayer != null)
            {
                version = FileVersionInfo.GetVersionInfo(unityPlayer.FullName).FileVersion;
                version = version![..version!.LastIndexOf('.')];
            }
            else
            {
                var binaryFile = GetGameExecutable(location, GetGameName(location));
                if (binaryFile != default(FileInfo))
                {
                    version = FileVersionInfo.GetVersionInfo(binaryFile.FullName).FileVersion;
                    version = version![..version!.LastIndexOf('.')];
                }
            }
        }
        else
        {
            // TODO: OSPlatform.OSX
        }

        return version;
    }

    public static string GetManagedFrameworkVersion(DirectoryInfo? gamePath, string gameName)
    {
        // The available values for TargetFrameworkVersion are v2.0, v3.0, v3.5, v4.5.2, v4.6, v4.6.1, v4.6.2, v4.7, v4.7.1, v4.7.2, and v4.8.
        var version = "net35";
        if (gamePath == null) return version;

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var managedDirectory = new DirectoryInfo(
                Path.Combine(Path.Combine(gamePath.FullName, gameName + "_Data"), "Managed"));

            var isUsingNetstandard = managedDirectory
                .GetFile("Assembly-CSharp.dll")
                ?.GetAssemblyReferences()
                .Any(asm => asm.Name?.ToString().Contains("netstandard") ?? false) ?? false;

            var matchingAssemblies = GetDllWithValidDotnetVersion(managedDirectory, isUsingNetstandard ? "netstandard*" : "mscorlib*") 
                                     ?? GetDllWithValidDotnetVersion(managedDirectory, "mscorlib*")
                                     ?? GetDllWithValidDotnetVersion(managedDirectory, "System.Core*");
            
            if (matchingAssemblies != null)
            {
                version = FileVersionInfo.GetVersionInfo(matchingAssemblies.FullName).FileVersion;
                var (major, minor, _) = version!.Split('.').Select(int.Parse);
                var isNetstandard = matchingAssemblies.Name.Contains("netstandard",
                    StringComparison.InvariantCultureIgnoreCase);

                version = major switch
                {
                    4 when minor >= 6 => "net472",
                    4 => "net452",
                    3 => "net35",
                    (< 3) when isNetstandard && minor >= 1 => "netstandard2.1",
                    (< 3) when isNetstandard && minor == 0 => "netstandard2.0",
                    (< 3) when !isNetstandard => "net35",
                    5 => "net5.0",
                    6 => "net6.0",
                    7 => "net7.0",
                    _ => "net35"
                };
            }
        }
        else
        {
            // TODO: OSPlatform.OSX
        }

        return version;
    }
    
    private static FileInfo? GetDllWithValidDotnetVersion(DirectoryInfo managedDirectory, string searchText) =>
        managedDirectory.GetFiles(searchText).FirstOrDefault(
            file =>
            {
                if (file != null)
                {
                    var fileVersion = FileVersionInfo.GetVersionInfo(file.FullName).FileVersion ?? "";
                    if (fileVersion.LastIndexOf('.') > 0 && int.Parse(fileVersion[0].ToString()) > 1)
                        return true;
                }

                return false;
            },
            null);
    
    public static DirectoryInfo? GetGamePathFromConfigurationFilesFrom(DirectoryInfo targetPath)
    {
        FileInfo? dbp = (targetPath.GetDirectory("src") ?? targetPath).GetFile("Directory.Build.props");
        if (dbp == null) return null;
        
        XElement root = XElement.Load(dbp.FullName);
        XElement? gamePathX = root.Elements("PropertyGroup")
            .Where(el => el.HasElements && el.Elements("GAME_PATH").Any())
            .FirstOrDefault(default(XElement))
            ?.Element("GAME_PATH");

        return gamePathX == null ? null : new DirectoryInfo(gamePathX.Value);
    }
    
    public static string GetBepInExVersion(DirectoryInfo? coreDirectoryLocation)
    {
        var version = "";
        if (coreDirectoryLocation == null) return version;
        
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var targetDll = coreDirectoryLocation.GetFile("BepInEx.dll") ??
                              coreDirectoryLocation.GetFile("BepInEx.Core.dll");
            
            if (targetDll != null)
            {
                version = FileVersionInfo.GetVersionInfo(targetDll.FullName).FileVersion;
                // version = version![..version!.LastIndexOf('.')];
            }
        }
        else
        {
            // TODO: OSPlatform.OSX
        }

        return version;
    }
}

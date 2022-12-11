using System.Diagnostics;
using System.Runtime.InteropServices;
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

                if (location.GetFiles(gameName + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")).Length == 0)
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

    public static string GetUnityPlayerVersion(DirectoryInfo location)
    {
        // UnityEngine.Modules v5.6.0 will be used when there's no UnityPlayer found
        // or when it's version is older than 20XX.X.X
        var version = "5.6.0";

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
        }
        else
        {
            // TODO: OSPlatform.OSX
        }

        return version;
    }
    
    public static string GetManagedFrameworkVersion(DirectoryInfo location, string gameName)
    {
        // The available values for TargetFrameworkVersion are v2.0, v3.0, v3.5, v4.5.2, v4.6, v4.6.1, v4.6.2, v4.7, v4.7.1, v4.7.2, and v4.8.
        var version = "net35";

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var managedDirectory = new DirectoryInfo(
                Path.Combine(Path.Combine(location.FullName, gameName + "_Data"), "Managed"));
            
            var matchingAssemblies = GetDllWithValidDotnetVersion(managedDirectory, "mscorlib*") 
                                     ?? GetDllWithValidDotnetVersion(managedDirectory, "System.Core*");
            
            if (matchingAssemblies != null)
            {
                version = FileVersionInfo.GetVersionInfo(matchingAssemblies.FullName).FileVersion;
                var (major, minor, _) = version!.Split('.').Select(int.Parse);

                version = major switch
                {
                    4 when minor >= 6 => "net472",
                    4 => "net452",
                    3 => "net35",
                    (< 3) => "netstandard2.1",
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
}

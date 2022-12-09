using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ModdingTools.App.CommandLine;

public static class CommandLineUtils
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
                gameName = d.Name.Substring(0, d.Name.Length - 5);
                
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
        var version = "";

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
}

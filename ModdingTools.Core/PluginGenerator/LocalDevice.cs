using System;
using System.IO;
using System.Threading;
using ModdingTools.Core.Utils;

namespace ModdingTools.Core.PluginGenerator;

public static class LocalDevice
{
    public static DirectoryInfo CreateRandomSubdirectory(DirectoryInfo directory) =>
        directory.CreateSubdirectory(Guid.NewGuid().ToString());

    public static bool TryGetOrCreateDirectory(string path, out DirectoryInfo directory)
    {
        directory = null;

        try
        {
            directory = new DirectoryInfo(path);
            if (!directory.Exists)
                directory.Create();

            return IsADirectory(directory);
        }
        catch (Exception)
        {
            /* ignored */
        }

        return false;
    }

    public static bool CanDelete(DirectoryInfo directory)
    {
        if (!directory.Exists) return false;

        var path = WithoutTrailingSlash(directory.FullName);
        var auxPath = path + "~aux";

        try
        {
            directory.MoveTo(auxPath);
            directory.MoveTo(path);

            return true;
        }
        catch (Exception)
        {
            /* ignored */
        }

        return false;
    }

    public static bool CanDelete(FileInfo file)
    {
        if (!file.Exists) return false;

        var path = WithoutTrailingSlash(file.FullName);
        var auxPath = path + "~aux";

        try
        {
            file.MoveTo(auxPath);
            file.MoveTo(path);

            return true;
        }
        catch (Exception)
        {
            /* ignored */
        }

        return false;
    }

    public static bool IsADirectory(FileSystemInfo fileSystemInfo) =>
        (fileSystemInfo.Attributes & FileAttributes.Directory) != 0;

    public static string WithoutTrailingSlash(string path) => path.TrimEnd('/', '\\');

    public static bool IsValid(string path, out string normalizedPath)
    {
        var aux = "";
        normalizedPath = "";
        var valid = SafeInvoke.TryInvoke(() => { aux = Path.GetFullPath(path); });
        normalizedPath = aux;

        return valid;
    }

    public static bool IsValid(string path) => SafeInvoke.TryInvoke(() => Path.GetFullPath(path));

    public static bool TryDeleteDirectoryAndAllContents(DirectoryInfo directory,
        int timeoutMs)
    {
        while (timeoutMs >= 0f)
        {
            timeoutMs -= 100;

            try
            {
                directory.Refresh();
                if (!directory.Exists)
                    return true;

                if (CanDelete(directory))
                {
                    directory.Delete(true);
                    directory.Refresh();
                    if (!directory.Exists)
                        return true;

                    Console.Error.WriteLine($"Directory still exists after deletion. ({directory.FullName})");
                }
            }
            catch (Exception)
            {
                /* ignored */
            }

            if (directory == null || !directory.Exists)
                return true;

            if (!IsADirectory(directory))
            {
                Console.Error.WriteLine(
                    new Exception("Trying to delete something that's not a directory."));
                return false;
            }

            Thread.Sleep(100);
        }

        return false;
    }
    
    public static string GetCustomTempPath() => Path.Combine(Path.GetTempPath(), "ModdingTools-TMP");

    public static DirectoryInfo GetExclusiveTempDirectory()
    {
        var flag = false;
        DirectoryInfo directory;
        
        if (!TryGetOrCreateDirectory(GetCustomTempPath(), out directory))
            throw new Exception("Invalid system's Path.GetTempPath() directory.");

        if (CanDelete(directory))
        {
            try
            {
                TryDeleteDirectoryAndAllContents(directory, 2000);
                directory.Refresh();
            }
            catch (Exception)
            {
                flag = true;
            }
            
            if (!TryGetOrCreateDirectory(GetCustomTempPath(), out directory))
                throw new Exception("Invalid system's Path.GetTempPath() directory.");
            
        }
        
        if (flag || !CanDelete(directory))
        {
            directory = CreateRandomSubdirectory(directory);

            if (!CanDelete(directory))
                throw new Exception(
                    "Unable to get an exclusive directory under system's Path.GetTempPath() directory.");
        }

        return directory;
    }
}

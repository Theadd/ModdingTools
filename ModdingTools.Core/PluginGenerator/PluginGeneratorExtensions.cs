using System;
using System.IO;

namespace ModdingTools.Core.PluginGenerator;

public static class PluginGeneratorExtensions
{
    private static bool NoFilter(string _) => true;

    public static bool WriteAllReferencesAsPublic(this IPluginGenerator self, string targetPath,
        Func<string, bool> filter = null)
    {
        var result = false;
        CILUtility cilUtility = new CILUtility();
        try
        {
            var cachedLibraries = self.CacheGameLibraries();
            using (DLLPatchContext dllPatchContext = new DLLPatchContext(cachedLibraries))
            {
                foreach (string listDll in cachedLibraries.ListDlls())
                    cilUtility.MakePublic(dllPatchContext.LoadOriginal(listDll));
                dllPatchContext.SaveAll(targetPath, filter);
            }

            result = true;
        }
        catch (Exception ex)
        {
            Console.Out.WriteLine(ex);
        }

        return result;
    }

    // private static Version FakeVersion => new Version(0, 3, DateTime.Now.DayOfYear, DateTime.Now.Month);

    private static DLLCache CacheGameLibraries(this IPluginGenerator self)
    {
        return new DLLCache(self.TempDirectory.FullName, self.Paths.ManagedPath,
            GameUtility.GetCurrentGameVersion(self.Paths.ManagedPath));
    }

    /// <summary>
    /// Attempts to delete all contents in this app's temp directory and returns it, if it can't be deleted,
    /// returns a new subfolder under it.
    /// </summary>
    /// <param name="self"></param>
    /// <returns>Temp directory</returns>
    public static DirectoryInfo GetExclusiveTempDirectory(this IPluginGenerator self) =>
        LocalDevice.GetExclusiveTempDirectory();

    public static bool TryDeleteDirectory(this IPluginGenerator self, DirectoryInfo directory,
        float timeoutInSeconds = 0.5f) =>
        LocalDevice.TryDeleteDirectoryAndAllContents(directory, (int) (timeoutInSeconds * 1000f));
}

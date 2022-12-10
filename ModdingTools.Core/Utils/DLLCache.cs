using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModdingTools.Core.Utils;

public class DLLCache
{
    private readonly string backupPath;
    private readonly string baseDllPath;
    private const string dllExt = ".dll";
    private const string dllWildCard = "*.dll";
    private List<Version> versionList;

    public Version CurrentVersion { get; }

    public DirectoryInfo BackupDirectory { get; }

    public DLLCache(string backupPath, string baseDllPath, Version version)
    {
        this.backupPath = backupPath;
        this.baseDllPath = baseDllPath;
        this.CurrentVersion = version;
        this.InitVersionList();
        this.BackupDirectory =
            new DirectoryInfo(Path.Combine(this.backupPath, this.CurrentVersion.ToString()));
        if (this.BackupDirectory.Exists)
            return;
        this.BackupDirectory.Create();
        this.InitBackups();
        this.versionList.Add(this.CurrentVersion);
    }

    private void InitVersionList()
    {
        this.versionList = new List<Version>();
        foreach (string enumerateDirectory in Directory.EnumerateDirectories(this.backupPath))
        {
            Version result;
            if (Version.TryParse(Path.GetFileName(enumerateDirectory), out result))
                this.versionList.Add(result);
        }
    }

    public void BackupOriginalWithoutLoading(string asmName, string lastBackup)
    {
        FileInfo fileInfo1 = new FileInfo(Path.Combine(this.baseDllPath, asmName + ".dll"));
        FileInfo fileInfo2 = new FileInfo(this.GetBackupPath(asmName));
        if (fileInfo2.Exists)
            return;
        fileInfo1.CopyTo(fileInfo2.FullName);
    }

    private string GetBackupPath(string asmName) =>
        Path.Combine(this.BackupDirectory.FullName, asmName + ".dll");

    public string GetDllPath(string asmName)
    {
        string backupPath = this.GetBackupPath(asmName);
        if (!File.Exists(backupPath))
            File.Copy(Path.Combine(this.baseDllPath, asmName + ".dll"), backupPath);
        return backupPath;
    }

    private void InitBackups()
    {
        string lastBackup = this.versionList.Any<Version>()
            ? Path.Combine(this.backupPath, this.versionList.Max<Version>().ToString())
            : (string) null;
        foreach (string listDll in this.ListDlls())
            this.BackupOriginalWithoutLoading(listDll, lastBackup);
    }

    public IEnumerable<string> ListDlls()
    {
        return Directory.EnumerateFiles(this.baseDllPath, "*.dll")
            .Select<string, string>((Func<string, string>) (x => Path.GetFileNameWithoutExtension(x)));
    }
}

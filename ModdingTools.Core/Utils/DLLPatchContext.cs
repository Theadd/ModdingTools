using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ModdingTools.Core.Utils;

public class DLLPatchContext : IDisposable
{
    private const string dllExt = ".dll";
    private readonly Dictionary<string, AssemblyDefinition> cache;
    private readonly DefaultAssemblyResolver resolver;
    private readonly ReaderParameters readerParams;
    private readonly DLLCache dllCache;

    public DLLPatchContext(DLLCache dllCache)
    {
        this.cache = new Dictionary<string, AssemblyDefinition>();
        this.resolver = new DefaultAssemblyResolver();
        MetadataResolver metadataResolver = new MetadataResolver((IAssemblyResolver) this.resolver);
        this.resolver.AddSearchDirectory(dllCache.BackupDirectory.FullName);
        this.readerParams = new ReaderParameters()
        {
            MetadataResolver = (IMetadataResolver) metadataResolver
        };
        AppDomain.CurrentDomain.AssemblyResolve +=
            new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
        this.dllCache = dllCache;
    }

    public AssemblyDefinition LoadMod(string modPath) =>
        AssemblyDefinition.ReadAssembly(modPath, this.readerParams);

    public AssemblyDefinition LoadOriginal(string asmName)
    {
        AssemblyDefinition assemblyDefinition;
        if (!this.cache.TryGetValue(asmName, out assemblyDefinition))
        {
            assemblyDefinition =
                AssemblyDefinition.ReadAssembly(this.dllCache.GetDllPath(asmName), this.readerParams);
            this.cache[asmName] = assemblyDefinition;
        }

        return assemblyDefinition;
    }

    private static Func<string, bool> NoFilter { get; } = (_) => true;

    public void SaveAll(string outputDir, Func<string, bool> filter = null)
    {
        foreach (KeyValuePair<string, AssemblyDefinition> keyValuePair in this.cache)
            if (filter == null || filter(keyValuePair.Key))
                keyValuePair.Value.Write(Path.Combine(outputDir, keyValuePair.Key + ".dll"));
    }

    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        string str = Path.Combine(this.dllCache.BackupDirectory.FullName,
            new AssemblyName(args.Name).Name + ".dll");
        return !File.Exists(str) ? (Assembly) null : Assembly.LoadFrom(str);
    }

    public void Dispose()
    {
        foreach (KeyValuePair<string, AssemblyDefinition> keyValuePair in this.cache)
            keyValuePair.Value.Dispose();
        this.cache.Clear();
        AppDomain.CurrentDomain.AssemblyResolve -=
            new ResolveEventHandler(this.CurrentDomain_AssemblyResolve);
    }

    public void RemoveAllMods(string managedDir)
    {
        foreach (string listDll in this.dllCache.ListDlls())
        {
            FileInfo fileInfo =
                new FileInfo(Path.Combine(this.dllCache.BackupDirectory.FullName, listDll + ".dll"));
            int num = new FileInfo(Path.Combine(managedDir, listDll + ".dll")).LastWriteTime >
                      fileInfo.LastWriteTime
                ? 1
                : 0;
        }
    }
}

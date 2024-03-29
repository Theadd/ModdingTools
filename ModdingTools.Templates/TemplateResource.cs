﻿using ModdingTools.Core.Common;

namespace ModdingTools.Templates;

public class TemplateResource
{
    public string ResourceFile { get; private set; } = String.Empty;
    
    private TemplateResource() { }

    public static TemplateResource From(string resourceFile)
    {
        return new TemplateResource() { ResourceFile = resourceFile };
    }

    public TemplateResource Load(string resourceFile)
    {
        ResourceFile = resourceFile;

        return this;
    }

    public TemplateResource Write(FileInfo targetFile) => Write(targetFile, null);

    public TemplateResource Write(FileInfo targetFile, Func<string, string>? action)
    {
        using (var streamReader = new StreamReader(ResourceManager.Instance.GetResource(ResourceFile)))
        {
            using (var streamWriter = new StreamWriter(targetFile.FullName))
            {
                // streamWriter.Write(action == null ? streamReader.ReadToEnd() : action.Invoke(streamReader.ReadToEnd()));
                streamWriter.Write(action?.Invoke(streamReader.ReadToEnd()) ?? streamReader.ReadToEnd());
            }
        }

        return this;
    }
    
    /// <summary>
    /// Unzips the content of the ResourceFile into destination directory with
    /// the filename provided by the rename action function.
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public TemplateResource UnZip(DirectoryInfo destination, Func<string, string>? action)
    {
        var zipLib = new ZipLib() { Destination = destination };
        
        // using (var streamReader = new StreamReader(ResourceManager.Instance.GetResource(ResourceFile)))
        using (var streamReader = ResourceManager.Instance.GetResource(ResourceFile))
        {
            zipLib.UnzipFromStream(streamReader, action ?? NoopStringFn);
        }

        return this;
    }

    private string NoopStringFn(string content) => content;
}

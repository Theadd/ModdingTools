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

    public TemplateResource Write(FileInfo targetFile)
    {
        using (var streamReader = new StreamReader(ResourceManager.Instance.GetResource(ResourceFile)))
        {
            using (var streamWriter = new StreamWriter(targetFile.FullName))
            {
                streamWriter.Write(streamReader.ReadToEnd());
            }
        }

        return this;
    }
}

namespace ModdingTools.Templates;

public class TemplateResource
{
    public string ResourceFile { get; set; }

    public static TemplateResource Load(string resourceFile)
    {
        return new TemplateResource() { ResourceFile = resourceFile };
    }

    public TemplateResource Write(FileInfo targetFile)
    {
        using (StreamReader streamReader =
               new StreamReader(ResourceManager.Instance.GetResource(ResourceFile)))
        {
            using (StreamWriter streamWriter =
                   new StreamWriter(targetFile.FullName))
            {
                streamWriter.Write(streamReader.ReadToEnd());
            }
        }

        return this;
    }
}

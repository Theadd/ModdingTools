using System.Linq;
using System.Xml.Linq;

namespace ModdingTools.Core.PluginGenerator.CSharpProjectFile;

public interface IXmlObject
{
}

public interface IXmlElement : IXmlObject
{
}

public interface IXmlComment : IXmlObject
{
}

public interface IXmlAttribute : IXmlObject
{
}

public interface IXmlText : IXmlObject
{
}

public class XmlComment : XComment, IXmlComment
{
    public XmlComment(string value) : base(value)
    {
    }

    public XmlComment(XComment other) : base(other)
    {
    }

    public static implicit operator XmlComment[](XmlComment one) => new[] { one };
    public static implicit operator XmlComment(string message) => new XmlComment(" " + message + " ");
}

public class XmlText : XText, IXmlText
{
    public XmlText(string value) : base(value)
    {
    }

    public XmlText(XmlText other) : base(other)
    {
    }

    public static implicit operator XmlText[](XmlText one) => new[] { one };
}

public class XmlAttribute : XAttribute, IXmlAttribute
{
    public XmlAttribute(XAttribute other) : base(other)
    {
    }

    public XmlAttribute(XName name, object value) : base(name, value)
    {
    }

    public static implicit operator XmlAttribute[](XmlAttribute one) => new[] { one };
}

public class XmlElement : XElement, IXmlElement
{
    public XmlElement(XElement other) : base(other)
    {
    }

    public XmlElement(XName name) : base(name)
    {
    }

    public XmlElement(XName name, object content) : base(name, content)
    {
    }

    public XmlElement(XName name, params object[] content) : base(name, content)
    {
    }

    public XmlElement(XStreamingElement other) : base(other)
    {
    }

    public static implicit operator XmlElement[](XmlElement one) => new[] { one };
}

public abstract class AbstractCSharpProjectFile : ICSharpProject
{
    protected static XDocument XmlDocument(XName name, params object[][] elements)
    {
        return new XDocument(new XElement(name, elements.SelectMany(x => x).Where(x => x != null)));
    }

    protected XName xN(string name) => XName.Get(name);
    protected XmlElement xE(string name, object content) => new XmlElement(xN(name), content);
    protected XmlElement xE(string name, params object[] content) => new XmlElement(xN(name), content);
    private XmlElement xE(XName name, object content) => new XmlElement(name, content);
    private XmlElement xE(XName name, params object[] content) => new XmlElement(name, content);
    private XmlAttribute xA(XName name, object value) => new XmlAttribute(name, value);
    protected XmlAttribute xA(string name, object value) => new XmlAttribute(xN(name), value);
    protected XDocument xD(params object[] content) => new XDocument(content);
    protected XmlComment _____(string content) => new XmlComment(" " + content + " ");
    protected XmlText TextNode(string content) => new XmlText(content);


    public abstract string TargetFramework { get; set; }
    public abstract string OutputPath { get; set; }
    public abstract void AddReference(string dllPath, string priv = "false", bool relative = true);
    public abstract void Save(string location);
    public abstract void AddResource(string path);
}

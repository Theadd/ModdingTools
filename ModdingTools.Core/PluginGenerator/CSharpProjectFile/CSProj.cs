using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ModdingTools.Core
{
    public class CSProj : ICSharpProject
    {
        public string Sdk { get; set; } = "Microsoft.NET.Sdk";

        public string TargetFramework { get; set; } = "netstandard2.1";

        public string Platforms { get; set; } = "AnyCPU";

        public string PreserveCompilationContext { get; set; } = "false";

        public string AppendTargetFrameworkToOutputPath { get; set; } = "false";

        public string OutputPath { get; set; }

        public string DebugType { get; set; } = "embedded";

        public List<CSProjReference> References { get; set; } = new List<CSProjReference>();

        public List<CSProjResource> Resources { get; set; } = new List<CSProjResource>();

        public void AddReference(string dllPath, string priv = "false", bool relative = true) =>
            References.Add(new CSProjReference
            {
                Name = Path.GetFileNameWithoutExtension(dllPath),
                HintPath = dllPath,
                Private = priv
            });

        public void Save(string location)
        {
            string dir = Path.GetDirectoryName(location);

            var csproj = new XDocument(
                new XElement(XName.Get("Project"), new XAttribute(XName.Get("Sdk"), Sdk),
                    new XElement(XName.Get("PropertyGroup"),
                        new XElement(XName.Get("TargetFramework"), TargetFramework),
                        new XElement(XName.Get("Platforms"), Platforms),
                        new XElement(XName.Get("PreserveCompilationContext"), PreserveCompilationContext),
                        new XElement(XName.Get("AppendTargetFrameworkToOutputPath"),
                            AppendTargetFrameworkToOutputPath),
                        new XElement(XName.Get("OutputPath"), Path.GetRelativePath(dir, OutputPath)),
                        new XElement(XName.Get("DebugType"), DebugType)),
                    new XElement(XName.Get("ItemGroup"), References.Select(r =>
                        new XElement(XName.Get("Reference"),
                            new XAttribute(XName.Get("Include"), r.Name),
                            new XElement(XName.Get("HintPath"),
                                r.Relative ? Path.GetRelativePath(dir, r.HintPath) : (object) r.HintPath),
                            new XElement(XName.Get("Private"), r.Private)))),
                    new XElement(XName.Get("ItemGroup"), Resources.Select<CSProjResource, XElement>((r =>
                        new XElement(XName.Get("None"),
                            new XAttribute(XName.Get("Remove"), (object) r.ResourcePath))))),
                    new XElement(XName.Get("ItemGroup"), Resources.Select<CSProjResource, XElement>((r =>
                        new XElement(XName.Get("EmbeddedResource"),
                            new XAttribute(XName.Get("Include"), (object) r.ResourcePath)))))));

            csproj.Save(location);
        }

        public void AddResource(string path) => Resources.Add(new CSProjResource
        {
            ResourcePath = path
        });
    }
}

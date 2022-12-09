using System;
using System.Collections.Generic;
using System.IO;

namespace ModdingTools.Core.Utils;

public class SolutionCreator
{
    public SolutionCreator(string name) => this.Name = name;

    public List<SolutionCreator.Proj> Projects { get; } = new List<SolutionCreator.Proj>();

    public string Name { get; }

    public void AddProject(string relativePath) => this.Projects.Add(new SolutionCreator.Proj()
    {
        Name = relativePath
    });

    public Guid Guid { get; private set; } = Guid.NewGuid();

    public List<string> Configs { get; set; } = new List<string>()
    {
        "Debug|Any CPU",
        "Release|Any CPU"
    };

    public void Save(string path)
    {
        using (StreamWriter text = File.CreateText(path))
        {
            text.WriteLine();
            text.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            text.WriteLine("# Visual Studio Version 16");
            text.WriteLine("VisualStudioVersion = 16.0.810.0");
            text.WriteLine("MinimumVisualStudioVersion = 10.0.40219.1");
            foreach (SolutionCreator.Proj project in this.Projects)
            {
                text.WriteLine(string.Format(
                    "Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{0}\", \"{1}\\{2}.csproj\", \"{{{3}}}\"",
                    (object) project.Name, (object) project.Name, (object) project.Name,
                    (object) project.Guid));
                text.WriteLine("EndProject");
            }

            text.WriteLine("Global");
            text.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            foreach (string config in this.Configs)
                text.WriteLine("\t\t" + config + " = " + config);
            text.WriteLine("\tEndGlobalSection");
            text.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            foreach (SolutionCreator.Proj project in this.Projects)
            {
                foreach (string config in this.Configs)
                {
                    text.WriteLine(string.Format("\t\t{{{0}}}.{1}.ActiveCfg = {2}", (object) project.Guid,
                        (object) config, (object) config));
                    text.WriteLine(string.Format("\t\t{{{0}}}.{1}.Build.0 = {2}", (object) project.Guid,
                        (object) config, (object) config));
                }
            }

            text.WriteLine("\tEndGlobalSection");
            text.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
            text.WriteLine("\t\tHideSolutionNode = FALSE");
            text.WriteLine("\tEndGlobalSection");
            text.WriteLine("\tGlobalSection(ExtensibilityGlobals) = postSolution");
            text.WriteLine(string.Format("\t\tSolutionGuid = {{{0}}}", (object) this.Guid));
            text.WriteLine("\tEndGlobalSection");
            text.WriteLine("EndGlobal");
        }
    }

    public class Proj
    {
        public string Name { get; set; }

        public Guid Guid { get; } = Guid.NewGuid();
    }
}

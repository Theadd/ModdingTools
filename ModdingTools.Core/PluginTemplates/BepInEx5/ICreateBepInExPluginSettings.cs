using ModdingTools.Core.PluginGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModdingTools.Core.PluginTemplates.BepInEx5
{
    public interface ICreateBepInExPluginSettings : ICreatePluginSettings
    {
        public string SolutionName { get; set; }
        public string ProjectName { get; set; }
        public string AssemblyName { get; set; }
        public string RootNamespace { get; set; }
        public string Location { get; set; }
        public bool IsMultiProjectSolution { get; set; }
    }
}

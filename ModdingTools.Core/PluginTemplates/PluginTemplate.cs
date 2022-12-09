using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ModdingTools.Core.PluginTemplates.BepInEx5;

namespace ModdingTools.Core.PluginTemplates
{
    public class PluginTemplate
    {
        public ICreatePluginSettings Configuration { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString() => Name;

        public static List<PluginTemplate> AllTemplates = new List<PluginTemplate>()
        {
            new PluginTemplate()
            {
                Name = "BepInEx 5 Template",
                Description = "This template is based on ...",
                Configuration = new CreateBepInExPluginSettings()
                {
                    Location = "I:/temp/GeneratedStuff",
                    SolutionName = "GameHacksCollection",
                    ProjectName = "BullTimeHack",
                    AssemblyName = "Ghole.GameHacksCollection.BullTimeHack",
                    RootNamespace = "Ghole.BullTimeHack",
                    IsMultiProjectSolution = true
                }
            },
        };
    }
}

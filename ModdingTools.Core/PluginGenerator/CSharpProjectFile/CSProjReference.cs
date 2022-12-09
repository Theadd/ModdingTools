namespace ModdingTools.Core
{
    public class CSProjReference
    {
        public bool Relative { get; set; } = true;

        public string Name { get; set; }

        public string Private { get; set; } = "false";

        public string HintPath { get; set; }
    }
}

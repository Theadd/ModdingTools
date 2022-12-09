using System;
using System.Collections.Generic;
using System.Text;

namespace ModdingTools.Core
{
    public interface ICreatePluginSettings
    {
        public ICreatePluginSettings AutoComplete();

        public bool Validate(ref List<string> invalids);

        public string GetTargetPath();
    }
}

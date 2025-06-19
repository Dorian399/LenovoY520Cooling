using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LenovoY520Cooling
{
    internal class Configs:ConfigurationSection
    {
        [ConfigurationProperty("maxTemp",DefaultValue = (int)80)]
        public int maxTemp
        {
            get { return (int)this["maxTemp"];  }
            set { this["maxTemp"] = value; }
        }

        [ConfigurationProperty("minTemp", DefaultValue = (int)60)]
        public int minTemp
        {
            get { return (int)this["minTemp"]; }
            set { this["minTemp"] = value; }
        }

        [ConfigurationProperty("startMinimized", DefaultValue = false)]
        public bool startMinimized
        {
            get { return (bool)this["startMinimized"]; }
            set { this["startMinimized"] = value; }
        }

        [ConfigurationProperty("startWithWindows", DefaultValue = false)]
        public bool startWithWindows
        {
            get { return (bool)this["startWithWindows"]; }
            set { this["startWithWindows"] = value; }
        }
    }
}

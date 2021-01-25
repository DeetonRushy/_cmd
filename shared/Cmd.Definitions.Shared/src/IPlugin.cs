using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd.Shared
{
    public interface ICmdPlugin
    {
        string Name { get; } // Name of plugin
        string Description { get; } // Plugin description.
        string Author { get; } // Name of author.
        string Version { get; } // Plugin version.

        IPluginHost Host { get; set; } 

        void Initialize(); // called when the plugin has been loaded and is ready.
        void Unload(); // called when the plugin is unloaded.
    }

    public interface IPluginHost
    {
        IDictionary<string, object> Locals { get; }
        void RegisterPlugin(ICmdPlugin plugin);
    }
}

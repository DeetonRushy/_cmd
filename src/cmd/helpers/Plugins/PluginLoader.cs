using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{

    // The plugin loader, on construction we get the active plugins
    // from the object PluginRetreiver. 

    // This class inherits from PluginStorage, this allows us to store & track loaded plugins.
    // We need to track & store them in order to load them & unload them.

    // RECENT: This class will be re-worked once the plugin API & shared data is
    // properly implemented.

    class PluginLoader : PluginStorage
    {
        public PluginLoader() { }
        ~PluginLoader() { }

        public void _LoadAll() { }
    }
}

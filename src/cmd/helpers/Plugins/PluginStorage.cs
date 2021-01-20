using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    // Loaded plugin storage.

    class PluginStorage : CommandExecutor
    {
        protected List<IPlugin> _loaded { get; private set; }

        public PluginStorage()
        {
            _loaded = new List<IPlugin>();
        }

        ~PluginStorage() { }

        protected void _pluginLoaded(IPlugin _plugin)
        {
            G.L.OG("New plugin module loaded. Name=" + _plugin.Name);
            _loaded.Append(_plugin);
        }
    }
}

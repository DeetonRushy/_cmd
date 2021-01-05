using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class PluginStorage
    {
        protected List<IPlugin> _loaded { get; private set; }

        public PluginStorage()
        {
            _loaded = new List<IPlugin>();
        }

        ~PluginStorage() { }

        protected void _pluginLoaded(IPlugin _plugin)
        {
            _loaded.Append(_plugin);
        }
    }
}

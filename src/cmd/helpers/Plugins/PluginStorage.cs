using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    // This class is going to be re-worked once the plugin API & shared data is all finished.

    class PluginStorage : CommandExecutor
    {

        public PluginStorage()
        {
        }

        ~PluginStorage() { }

        protected void _pluginLoaded<T>(T _plugin)
        {

        }
    }
}

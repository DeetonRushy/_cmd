using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{

    // This interface will, at some point, be usable via any plugin.
    // We attach the type to their plugin API & when we load them
    // into our memory space, we fill the class in with our class below.

    // I said fill in because I don't really understand the terms to use.

    interface IHelperFactory
    {
        PluginLoader CreatePluginLoader();
        PluginRetreiver CreatePluginRetreiver(string path);
        PluginStorage CreatePluginStorage();
    }

    // Factory for creating types

    class HelperFactory : IHelperFactory
    {
        public PluginLoader CreatePluginLoader() => new PluginLoader();

        public PluginRetreiver CreatePluginRetreiver(string path) => new PluginRetreiver(path);

        public PluginStorage CreatePluginStorage() => new PluginStorage();
    }
}

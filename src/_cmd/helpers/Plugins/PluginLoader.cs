using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class PluginLoader : PluginStorage
    {
        const string dir = "data\\plugins";
        private object _lock = new object();
        public static bool allow_plugins = true;

        public string[] _Files
        {
            get;
            private set;
        }

        public PluginLoader()
        {
            _Files = new PluginRetreiver(dir).get();
        }

        public bool _LoadAll()
        {
            lock (_lock)
            {
                if(!allow_plugins)
                {
                    return allow_plugins;
                }
            }

            foreach(string _file in _Files)
            {
                if (_file.EndsWith(".dll"))
                {
                    try
                    {
                        System.Reflection.Assembly.LoadFile(System.IO.Directory.GetCurrentDirectory() + "\\" + _file);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to load -> " + _file);
                        continue;
                    }
                }
            }

            Type ext_Interface = typeof(IPlugin);

            Type[] all_loaded_plugins = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(p => ext_Interface.IsAssignableFrom(p) && p.IsClass)
                .ToArray();

            foreach(Type plugin in all_loaded_plugins)
            {
                IPlugin _plugin = (IPlugin)Activator.CreateInstance(plugin);
                _plugin._Go(); // call their initialization method.
                _pluginLoaded(_plugin);
            }

            return true;
        }
    }
}

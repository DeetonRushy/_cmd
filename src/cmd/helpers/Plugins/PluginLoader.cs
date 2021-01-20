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

    class PluginLoader : PluginStorage
    {
        const string dir = "data\\plugins"; // we use a constant directory to make things easier.
        private object _lock = new object();
        public static bool allow_plugins = true;

        public PluginLoader()
        {
            _Files = new PluginRetreiver(dir).get();
        }

        public string[] _Files
        {
            get;
            private set;
        }


        /// <summary>
        /// The main load all method. This loads & executes each plugin one by one.
        /// </summary>
        /// <returns>returns false if plugins are disabled, otherwise returns true.</returns>
        public bool _LoadAll()
        {
            lock (_lock)
            {
                if(!allow_plugins)
                {
                    G.L.OG("Plugins have been disabled by the command line.");
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
                        G.L.OG("Failed to load -> " + _file);
                        continue;
                    }

                    G.L.OG("Loading plugin : " + (dir + "-" + _file));
                }

                if (_file.EndsWith(".exe"))
                {
                    // that last 4 letters will be '.', 'e', 'x', 'e', lets remove them for the command name.
                    char[] resource = _file.ToCharArray();
                    int end_index = 0;

                    for(int i = 0; i < _file.Length; i++)
                    {
                        if(resource[i] == '\\') // we want to parse the file name and remove the backslashes.
                        {
                            end_index = i+1;
                        }
                    }

                    string _new = _file.Remove(0, end_index); // remove all extra directory names etc.
                    string _name = _new.Remove(_new.Length - 4); // remove .exe

                    MakeCommand(_name, "__plugin__" + _name, () => // create plugin with their executable name.
                    {

                        // this commands starts their process
                        // as a child to our process.

                        System.Diagnostics.Process process = new System.Diagnostics.Process
                        {
                            StartInfo = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = @"cmd.exe",
                                Arguments = "echo [cmd]: starting plugin...",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + "\\data\\plugins\\exec",
                                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                            }
                        };

                        try
                        {
                            process.Start();
                        }
                        catch
                        {
                            return RetType._C_SYSTEM_ERROR;
                        }

                        return RetType._C_SUCCESS;

                    }, "This is an executable plugin, no description is provided.");
                }
            }

            Type ext_Interface = typeof(IPlugin); // get the raw Type.

            Type[] all_loaded_plugins = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(p => ext_Interface.IsAssignableFrom(p) && p.IsClass)
                .ToArray();

            // we then get all types that use our interface.

            foreach(Type plugin in all_loaded_plugins)
            {
                IPlugin _plugin = (IPlugin)Activator.CreateInstance(plugin); // create the instance.
                _plugin._Go(); // call their initialization method.
                _pluginLoaded(_plugin);

                unsafe
                {
                    G.L.OG($"Plugin Info : name={_plugin.Name}, Desc={_plugin.Explaination}");
                }
            }

            return true;
        }
    }
}

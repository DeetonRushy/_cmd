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
                        if(resource[i] == '\\')
                        {
                            end_index = i+1;
                        }
                    }

                    string _new = _file.Remove(0, end_index);
                    string _name = _new.Remove(_new.Length - 4); 

                    MakeCommand(_name, "__plugin__" + _name, () =>
                    {

                        System.Diagnostics.Process process = new System.Diagnostics.Process
                        {
                            StartInfo = new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = @"cmd.exe",
                                Arguments = "echo [_cmd]: starting plugin...",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + "\\data\\data\\plugins\\exec",
                                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                            }
                        };

                        try
                        {
                            process.Start();
                        }
                        catch
                        {
                            return _cmdReturn._C_SYSTEM_ERROR;
                        }

                        return _cmdReturn._C_SUCCESS;

                    }, "This is an executable plugin, no description is provided.");
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

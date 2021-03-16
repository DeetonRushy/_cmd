using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace cmd
{

    // The plugin loader, on construction we get the active plugins
    // from the object PluginRetreiver. 

    // This class inherits from PluginStorage, this allows us to store & track loaded plugins.
    // We need to track & store them in order to load them & unload them.

    // RECENT: This class will be re-worked once the plugin API & shared data is
    // properly implemented.

    class PluginLoader
    {
        public static void LoadAllPlugins(ref IDictionary<Assembly, ICmdPlugin> pluginHost, ref CommandExecutor context ) {
            // firstly, we search for available plugins in the data/plugins directory.

            if ( !Directory.Exists( "data\\plugins" ) ) {
                return;
            }

            // We then search for every file in the directory that contains *.dll
            // It doesn't matter if they have their own folders.

            foreach(string pluginpath in Directory.GetFiles( "data\\plugins", "*.dll", SearchOption.AllDirectories ) ) {

                Assembly ass = null;

                try {
                    ass = Assembly.Load( pluginpath );
                }
                catch {
                    continue;
                }

                Type info = null;

                try {
                    Type[] types = ass.GetTypes();
                    Assembly core = AppDomain.CurrentDomain.GetAssemblies().Single( x => x.GetName().Name.Equals( "cmd" ) );
                    Type type = core.GetType( "cmd.ICmdPlugin" );

                    foreach(var _t in types ) {
                        if(type.IsAssignableFrom((Type) _t ) ) {
                            info = _t;
                            break;
                        }
                    }

                    if( info != null ) {
                        object o = Activator.CreateInstance( info );
                        ICmdPlugin plugin = ( ICmdPlugin ) o;

                        plugin.OnPluginLoad( context, G.host );
                        pluginHost.Add( core, plugin );
                    }
                }
                catch {
                }

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace _cmd
{
    class _EntryPoint
    {
        static void Main(string[] args)
        {
            bool c_Exit = false;
            bool KillLauncherProcess = false;
            Parser arg_worker = new Parser(args);

            arg_worker
                .set_error_on_required("Missing a required arguement.")
                .add_optional("--disable-plugins", ref PluginLoader.allow_plugins, false)

            Start _main = new Start();
            G.L.OG("Beginning CrtMain.");
            _main._CrtMain_Start();
        }
    }
}

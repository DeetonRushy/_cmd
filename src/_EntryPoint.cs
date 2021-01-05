using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class _EntryPoint
    {
        static void Main(string[] args)
        {
            bool c_Exit = false;
            Parser arg_worker = new Parser(args);

            arg_worker
                .add_optional("--disable-plugins", ref PluginLoader.allow_plugins, false)
                .add_optional("--cancel-init", ref c_Exit, true);

            if (c_Exit)
            {
                Environment.Exit(0);
            }

            Start _main = new Start();
            _main._CrtMain_Start();
        }
    }
}

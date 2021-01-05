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
            Parser arg_worker = new Parser(args);

            arg_worker
                .add_optional("--disable-plugins", ref PluginLoader.allow_plugins, false);

            Start _main = new Start();
            _main._CrtMain_Start();
        }
    }
}

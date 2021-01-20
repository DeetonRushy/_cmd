using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace cmd
{
    class Entry
    {
        static void Main(string[] args)
        {
            Parser arg_worker = new Parser(args);

            arg_worker
                .set_error_on_required("Missing a required arguement.")
                .add_optional("--disable-plugins", ref PluginLoader.allow_plugins, false)
                .add_optional("--disable-flush", ref Start.__flush_disabled__, true);

            Start _main = new Start();
            _main._CrtMain_Start(); // no idea why I called it CrtMain, just looks cool I guess lol.
        }
    }
}

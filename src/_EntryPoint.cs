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
            string[] test = { };

            arg_worker
                .set_error_on_required("Missing a required arguement.")
                .add_optional("--disable-plugins", ref PluginLoader.allow_plugins, false)
#if DEBUG
                .add_optional("--cancel-init", ref c_Exit, true) // debug arguement
#endif
                .add_required("--kill-launcher", ref KillLauncherProcess, true)
                .add_required("--Exec_PP01901.._-A1--", ref c_Exit, false)
                .add_required_with_answer("--test-data", ref test);

            foreach(string s in test)
            {
                Console.WriteLine(s);
            }

            Console.ReadLine();

            G.L.OG("Finished parsing command line.");

            if (c_Exit)
            {
                Environment.Exit(0);
            }

            if (KillLauncherProcess)
            {
                // Process was start via launcher

                G.L.OG("Terminating launcher.");

                foreach (Process _proc in Process.GetProcesses())
                {
                    if(_proc.ProcessName == "Launcher")
                    {
                        _proc.Kill();
                    }
                }
            }

            Start _main = new Start();
            G.L.OG("Beginning CrtMain.");
            _main._CrtMain_Start();
        }
    }
}

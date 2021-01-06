using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class Start : CommandExecutor
    {
        public Start() { }
        ~Start() { }

        public void _CrtMain_Start()
        {
            MakeCommand("##help", "__inter_help", () =>
            {
                Console.WriteLine();
                Console.WriteLine("##list : lists all commands.");
                return _cmdReturn._C_SUCCESS;

            }, "The basic help command built into _cmd.");

            MakeCommand("##list", "__inter_list", _List_Cmd, "Internal list command, lists all commands.");

            // We load the plugins once we have created the start object.
            // Once loaded, they will be able to access our plugin API
            // (which is not yet created.) 
            // All files and commands can they be developed externally.

            PluginLoader p_load = new PluginLoader();
            p_load._LoadAll();

            while (true)
            {
                #region __
                Console.Write(">> ");
                #endregion

                string command = Console.ReadLine();

                if (command == "exit")
                    Environment.Exit(0);

                _cmdReturn result = ExecuteCommand(command);
                ErrorTranslator _Ts = new ErrorTranslator(result);

                #region ___
                Console.Title = "Command returned -> " + _Ts.Error;
                Console.WriteLine();
                #endregion
            }
        }
    }
}

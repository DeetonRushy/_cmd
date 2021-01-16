using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class Start : CommandExecutor
    {

        public static string __version__ = "v1.0.0"; // __version__ is G.
        public object _lock;

        public static void MakeCommandStatic(string a, string b, Func<_cmdReturn> c, string d)
        {
            // the plan is to access the protected member from the child/base of this class.
            // we need to somehow call the function from the instance that we are parenting.
            // we cannot do this any other way because the child instance holds all command
            // data & information about loaded plugins etc.

            // TLDR: We need to access the non-static function from the child base class &
            // call it in here.
        }

        public Start() { _lock = new object(); }
        ~Start() { }

        private void LoadStartInternalPlugins()
        {
            MakeCommand("%help", "__inter_help", () =>
            {
                return ExecuteCommand("%listcmd");

            }, "The basic help command built into _cmd.");

            MakeCommand("%listcmd", "__inter_simple_list", () =>
            {

                return _List_Cmd_Simple();

            }, "The list command for users that don't want to know internal names etc.");

            MakeCommand("##list", "__inter_list", _List_Cmd, "Internal list command, lists all commands.");

            MakeCommand("%name", "__inter_name", () =>
            {
                Console.WriteLine();
                Console.WriteLine(Environment.UserName);
                return _cmdReturn._C_SUCCESS;
            }, "Outputs the environment username.");

            MakeCommand("%version", "__inter__ver__", () =>
            {
                string _ver = null;

                lock (_lock)
                {
                    _ver = __version__;
                }

                if (_ver == null)
                    return _cmdReturn._C_ACCESSVIOLATION;

                Console.WriteLine();
                Console.WriteLine("_cmd current version is " + _ver);

                return _cmdReturn._C_SUCCESS;

            }, "Outputs the current version of _cmd that is being used.");

#if DEBUG

            MakeCommand("%broken", "__DEBUG_BROKEN", () =>
            {
                Console.WriteLine(this);
                return _cmdReturn._C_ACCESSVIOLATION;

            }, "Broken command, made for debugging purposes.");

#endif

            MakeCommand("%cdirec", "__inter_platform", () =>
            {

                string _direc = System.IO.Directory.GetCurrentDirectory();
                Console.WriteLine("Currently directory -> " + _direc);
                return _cmdReturn._C_SUCCESS;

            }, "Outputs the current working directory of _cmd.");
        }

        public void WriteCmdStartText(int ms=0)
        {
            if (ms != 0)
                System.Threading.Thread.Sleep(ms);

            Console.WriteLine(
@"
Welcome to _cmd!
This software is open source & free to use.
https://github.com/DeetonRushy/_cmd

");
            Console.WriteLine();
        }

        public void _CrtMain_Start()
        {
            LoadStartInternalPlugins();
            WriteCmdStartText(50);

            // We load the plugins once we have created the start object.
            // Once loaded, they will be able to access our plugin API
            // (which is not yet created.) 
            // All files and commands can they be developed externally.

            PluginLoader p_load = new PluginLoader();
            p_load._LoadAll();



            while (true)
            {
                #region __
                Console.Write("-> ");
                #endregion

                string command = Console.ReadLine();

                if (command == "exit")
                    Environment.Exit(0);

                _cmdReturn result = ExecuteCommand(command);
                ErrorTranslator _Ts = new ErrorTranslator(result);

                #region ___
                Console.Title = "Command returned -> " + _Ts.Error + " | " + __version__;
                Console.WriteLine();
                #endregion
            }
        }
    }
}

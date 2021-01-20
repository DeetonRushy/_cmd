using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    class Start : CommandExecutor
    {
        // if --disable-flush is passed as a parameter, this is true.
        // this then causes flush to cancel on execution.

        public static bool __flush_disabled__ = false;
        public object _lock;

        public static void MakeCommandStatic(string a, string b, Func<RetType> c, string d)
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
            #region HELP

            MakeCommand("%help", "__inter_help", () =>
            {
                return ExecuteCommand("%listcmd");

            }, "The basic help command built into cmd.");

            #endregion

            #region SIMPLE_HELP

            MakeCommand("help", "__inter__help", () => { return Run("%help"); }, "The basic help command.");

            #endregion

            #region LIST_CMD

            MakeCommand("%listcmd", "__inter_simple_list", () =>
            {

                return _Listcmd_Simple();

            }, "The list command for users that don't want to know internal names etc.");

            #endregion

            #region ADVANCED_LIST

            MakeCommand("##list", "__inter_list", _Listcmd, "Internal list command, lists all commands.");

            #endregion

            #region NAME

            MakeCommand("%name", "__inter_name", () =>
            {
                Console.WriteLine();
                Console.WriteLine(Environment.UserName);
                return RetType._C_SUCCESS;

            }, "Outputs the environment username.");

            #endregion

            #region VERSION

            MakeCommand("%version", "__inter__ver__", () =>
            {
                string _ver = null;

                lock (_lock)
                {
                    _ver = G.__version__;
                }

                if (_ver == null)
                    return RetType._C_ACCESSVIOLATION;

                Console.WriteLine();
                Console.WriteLine("cmd current version is " + _ver);

                return RetType._C_SUCCESS;

            }, "Outputs the current version of cmd that is being used.");

            #endregion

            #region DEBUG_BROKEN_COMMAND

#if DEBUG

            MakeCommand("%broken", "__DEBUG_BROKEN", () =>
            {
                Console.WriteLine(this);
                return RetType._C_ACCESSVIOLATION;

            }, "Broken command, made for debugging purposes.");

#endif

            #endregion

            #region CURRENT_DIRECTORY

            MakeCommand("%cdirec", "__inter_platform", () =>
            {

                string _direc = System.IO.Directory.GetCurrentDirectory();
                Console.WriteLine("Currently directory -> " + _direc);
                return RetType._C_SUCCESS;

            }, "Outputs the current working directory of cmd.");

            #endregion

            #region FLUSH

            MakeCommand("%flush", "__inter_danger_flush", () =>
            {

                lock (_lock)
                {
                    if (__flush_disabled__)
                    {
                        ErrorOutputHandler.Out(RetType._C_DISABLED, "--disable-flush was passed.");
                        return RetType._C_DISABLED;
                    }
                }

                Console.WriteLine("Executing this command will flush every command from this instance of cmd.");
                Console.Write("Are you sure? (Y/N): ");
                string result = Console.ReadLine().ToLower();

                if (result.Contains("y"))
                {
                    string[] results = new string[StorageWorkload.Count];
                    int index = 0;

                    foreach (KeyValuePair<string, Command> kv in StorageWorkload)
                    {
                        results[index] = kv.Key;
                        index++;
                    }

                    foreach(string cmd in results)
                    {
                        if (G.IsNull(cmd))
                            continue;

                        if (FlushCommand(cmd))
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    return RetType._C_SUCCESS;
                }

                return RetType._C_SUCCESS;

            }, "Flushes every active command from memory & active workload.");

            #endregion

            #region DEBUG_BROKEN_DUPLICATE

#if DEBUG
            MakeCommand("%flush", "__broken", () => { return RetType._C_FAILURE; }, "Broken");
#endif

            #endregion
        }

        public void WriteCmdStartText(int ms=0)
        {
            if (ms != 0)
                System.Threading.Thread.Sleep(ms);

            Console.WriteLine("Welcome to cmd!\nThis software is open source & free to use.\nhttps://github.com/DeetonRushy/cmd\n\r");
        }

        // Enters application mainloop.

        public void _CrtMain_Start()
        {
            LoadStartInternalPlugins();
            WriteCmdStartText(50);

            // We load the plugins once we have created the start object.
            // Once loaded, they will be able to access our plugin API
            // ( which is not yet created. ) 
            // All files and commands can they be developed externally.

            PluginLoader p_load = new PluginLoader();
            p_load._LoadAll();

            while (true)
            {
                #region InputPointer
                Console.Write("-> ");
                #endregion

                string command = Console.ReadLine();
                RetType result = ExecuteCommand(command);
                ErrorTranslator _Ts = new ErrorTranslator(result);

                #region Title&Seperator
                Console.Title = "Command returned -> " + _Ts.Error + " | " + G.__version__;
                Console.WriteLine();
                #endregion
            }
        }
    }
}

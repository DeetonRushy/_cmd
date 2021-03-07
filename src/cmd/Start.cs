using System;
using System.Collections.Generic;

namespace cmd
{
    public sealed class Start
    {
        // if --disable-flush is passed as a parameter, this is true.
        // this then causes flush to cancel on execution.

        #region CommandLineVariables

        public static bool __flush_disabled__ = false;
        public static bool __arguement_cmds_disabled__ = false;

        #endregion


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

            G.context.MakeCommand("%help", "__inter_help", () =>
            {
                return G.context.ExecuteCommand("help", "/?");

            }, "The basic help command built into cmd.");

            #endregion

            #region HELP_ARGS

            G.context.CreateCommand("help", "__inter_help_main", (args) =>
            {

                string _help()
                {
                    return "LISTING ARGUMENTS\n\n- /?\n- /list\n- /basic\n- /cdirec\n";
                }

                if(args[0] == "/?")
                {
                    // output info about available commands.

                    Console.WriteLine(_help());

                    return RetType._C_SUCCESS;
                }

                Parser arg_work = new Parser(args, false);

                #region InfoSetup

                bool basic = false;
                bool cdirec = false;
                bool list = false;

                #endregion

                arg_work
                     .add_optional("/basic", ref basic, true)
                     .add_optional("/cdirec", ref cdirec, true)
                     .add_optional("/list", ref list, true);

                if (basic)
                {
                    #region basic_information_output

                    Console.WriteLine("\nThis is an extremely flexable command prompt that supports");
                    Console.WriteLine("plugins, single commands, commands with arguements.");
                    Console.WriteLine("It's also open source, it can be found here : https://github.com/DeetonRushy/_cmd");

                    return RetType._C_SUCCESS;

                    #endregion
                }

                if (cdirec)
                {
                    #region cdirec_information

                    Console.WriteLine("\nThe command is '%cdirec'. It outputs the current location of this binary.");

                    return RetType._C_SUCCESS;

                    #endregion
                }

                if (list)
                {
                    #region list_information

                    Console.WriteLine("Type '%listcmd' to view all active commands.");
                    return RetType._C_SUCCESS;

                    #endregion
                }


                return RetType._C_SUCCESS;

            }, "The main help command that takes arguements for certain information.", null);

            #endregion

            #region SIMPLE_HELP

            G.context.MakeCommand("help", "__inter__help", () => { return G.context.ExecuteCommand("%help"); }, "The basic help command.");

            #endregion

            #region LIST_CMD

            G.context.MakeCommand("%listcmd", "__inter_simple_list", () =>
            {

                return G.context._Listcmd_Simple();

            }, "The list command for users that don't want to know internal names etc.");

            #endregion

            #region CLEAR_SINGLE

            G.context.MakeCommand("echo", "__inter_echo_safe", () =>
            {
                return G.context.ExecuteCommand("echo", "This", "command", "takes", "your", "text!");

            }, "This command is there to help someone who doesn't know how to use echo.");

            #endregion

            #region CLEAR

            G.context.MakeCommand("%clear", "__inter_clear", () =>
            {
                Console.Clear();
                return RetType._C_SUCCESS;

            }, "Clear the console.");

            #endregion

            #region ECHO

            G.context.CreateCommand("echo", "__inter_echo", (args) =>
            {

                Console.WriteLine();

                foreach(string sep in args)
                {
                    Console.Write(sep + " ");
                }

                Console.WriteLine();

                return RetType._C_SUCCESS;

            }, "Outputs text.", null);

            #endregion

            #region TIME

            G.context.MakeCommand("time", "__inter_time", () =>
            {

                var current = DateTime.Now;

                Console.WriteLine("Current time: " + current.ToLongTimeString());

                return RetType._C_SUCCESS;

            }, "Displays the current time.");

            #endregion

            #region RESET

            G.context.MakeCommand("_reset", "__inter_reset", () =>
            {
                G.Panic();
                return RetType._C_SUCCESS;

            }, "Resets the current instance.");

            #endregion

            #region NAME

            G.context.MakeCommand("%name", "__inter_name", () =>
            {
                Console.WriteLine();
                Console.WriteLine(Environment.UserName);
                return RetType._C_SUCCESS;

            }, "Outputs the environment username.");

            #endregion

            #region VERSION

            G.context.MakeCommand("%version", "__inter__ver__", () =>
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

            G.context.MakeCommand("%broken", "__DEBUG_BROKEN", () =>
            {
                Console.WriteLine(this);
                return RetType._C_ACCESSVIOLATION;

            }, "Broken command, made for debugging purposes.");

#endif

            #endregion

            #region CURRENT_DIRECTORY

            G.context.MakeCommand("%cdirec", "__inter_platform", () =>
            {

                string _direc = System.IO.Directory.GetCurrentDirectory();
                Console.WriteLine("Currently directory -> " + _direc);
                return RetType._C_SUCCESS;

            }, "Outputs the current working directory of cmd.");

            #endregion

            #region DEBUG_BROKEN_DUPLICATE

#if DEBUG
            G.context.MakeCommand("%flush", "__broken", () => { return RetType._C_FAILURE; }, "Broken");
#endif

            #endregion

            #region DEBUG_TEST_ARGUEMENTS

#if DEBUG

            G.context.CreateCommand("debug", "__inter_debug_params", (param) =>
            {
                Console.WriteLine();

                foreach(string _p in param)
                {
                    Console.WriteLine(_p);
                }

                return RetType._C_SUCCESS;

            }, "debug command for testing command parameters.", null);

#endif

            #endregion

            #region CD

            G.context.CreateCommand("cd", "__inter_cd", (d_info) =>
            {

                if(!System.IO.Directory.Exists(d_info[0]))
                {
                    G.Out(RetType._C_FAILURE, "Directory doesn't exist.");
                    return RetType._C_FAILURE;
                }

                G.current_directory = d_info[0];

                return RetType._C_SUCCESS;


            }, "Changes the current directory context.", null);

            #endregion

            #region COLOR

            G.context.CreateCommand("color", "__inter_color", (c) =>
            {
                if (c[0] == "help")
                {
                    Console.WriteLine("EXAMPLE: color front blue");
                    Console.WriteLine("EXAMPLE: color back pink");
                    return RetType._C_SUCCESS;
                }

                if (c.Length != 2)
                {
                    G.Out(RetType._C_FAILURE, "Try 'color help'.");
                    return RetType._C_FAILURE;
                }

                ConsoleColor final = ConsoleColor.White;
                bool type = false; // false is back, true is front

                if (c[0] == "front")
                    type = false;

                switch (c[1])
                {
                    case "red":
                        final = ConsoleColor.Red;
                        break;
                    case "blue":
                        final = ConsoleColor.Blue;
                        break;
                    case "green":
                        final = ConsoleColor.Green;
                        break;
                    case "pink":
                        final = ConsoleColor.Magenta;
                        break;
                    case "reset":
                        Console.ResetColor();
                        return RetType._C_SUCCESS;
                    default:
                        G.Out(RetType._C_FAILURE, "color isn't implemented.");
                        return RetType._C_FAILURE;
                }

                if (type)
                {
                    Console.ForegroundColor = final;
                }
                else
                {
                    Console.BackgroundColor = final;
                }

                return RetType._C_SUCCESS;

            }, "changes the console color.", null);

            #endregion

            #region DIR

            G.context.CreateCommand("dir", "__inter_touch", (args) =>
            {
                string _work = args[0]; // ignore the rest
                string work = null;

                if(_work == "help")
                {
                    Console.WriteLine("This command lists all items in a directory.");
                    Console.WriteLine("This uses unix file paths on windows to make typing much easier.");
                    Console.WriteLine("But it defaults to the C: drive.");
                    Console.WriteLine("Example Usage : dir \\users");
                    return RetType._C_SUCCESS;
                }

                if (!_work.Contains(":\\"))
                {
                    work = _work; // assume it contains C:\
                }
                else
                {
                    work = $"C:\\{_work}"; // support unix type paths
                }

                if (!System.IO.Directory.Exists(work))
                {
                    G.Out(RetType._C_IOERROR, "Directory doesn't exist.");
                    return RetType._C_IOERROR;
                }

                string[] d_items = System.IO.Directory.GetDirectories(work);
                string[] f_items = System.IO.Directory.GetFiles(work);

                foreach(string folder in d_items)
                {
                    Console.WriteLine("[folder] " + folder);
                }

                foreach(string file in f_items)
                {
                    Console.WriteLine("[file] " + file);
                }

                return RetType._C_SUCCESS;

            }, "Lists all items in a directory.", null);

            #endregion

            #region TOUCH

            G.context.CreateCommand("touch", "__inter_touch", (arg) =>
            {
                G.Out(RetType._C_RESOURCE_NOT_EXIST, "This command will be implemented once the global directory object works.");
                return RetType._C_SUCCESS;

            }, "Stamps a fresh timestamp on an existing file or creates a file.", null);

            #endregion

            #region CVarList

            G.context.MakeCommand( "lscvar", "__inter_cvar_ls", ( ) => {
                Console.WriteLine();

                foreach ( KeyValuePair<string, CVarContainer> kvp in G.host.Host ) {
                    Console.WriteLine( $"\"{kvp.Value.Name}: \"{kvp.Value.Value}\". \n\"{kvp.Value.Description}\"\n" );
                }

                return RetType._C_SUCCESS;

            }, "Lists all CVars with their name, value and description." );

            #endregion
        }

        public void WriteCmdStartText(int ms = 0)
        {
            Console.Title = "cmd .. " + G.__version__;

            if (ms != 0)
                System.Threading.Thread.Sleep(ms);

            Console.WriteLine("Welcome to cmd!\nThis software is open source & free to use.\nhttps://github.com/DeetonRushy/cmd\n\r");
        }

        // Enters application mainloop.

        public void exec()
        {
            LoadStartInternalPlugins();
            WriteCmdStartText(50);
            G.RCVars();

            // We load the plugins once we have created the start object.
            // Once loaded, they will be able to access our plugin API
            // ( which is not yet created. ) 
            // All files and commands can then be developed externally.

            PluginLoader.LoadAllPlugins( ref G.context.PluginWorkload, ref G.context );

            while (true)
            {
                #region InputPointer
                Console.Write(G.line_pointer);
                #endregion

                if (G.restart_needed)
                    break; // Panic has been called.

                string command = Console.ReadLine();

                #region InputParsing

                bool exec_w_params = false;
                string[] _work = command.Split();

                lock (_lock)
                {
                    if (__arguement_cmds_disabled__)
                        goto SkipParse;
                }

                if (_work.Length > 1)
                {
                    // the user entered more than one word,
                    // must contain arguements too.

                    exec_w_params = true;
                    command = _work[0]; // set command to the command name before we remove it from the array.
                    _work = G.RemoveAt(_work, 0); // remove the name of the command.
                }

                Console.WriteLine();

            SkipParse:


                #endregion

                #region CVarCheck

                if ( G.host.Exists( command ) ) {
                    if ( exec_w_params ) {
                        G.host.OnTypedCommand( command, _work );
                    }
                    else {
                        G.host.OnTypedCommand( command, null );
                    }

                    continue;
                }

                #endregion

                RetType result = exec_w_params ? G.context.ExecuteCommand(command, _work) : G.context.ExecuteCommand(command);
                ErrorTranslator _Ts = new ErrorTranslator(result);

                #region Title&Seperator
                Console.Title = "Command returned -> " + _Ts.Error + " | " + G.__version__;
                Console.WriteLine();
                #endregion
            }

        }
    }
}

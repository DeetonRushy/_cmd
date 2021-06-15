using System;
using System.Collections.Generic;
using LibGit2Sharp;
using System.Net;
using cmd_NonGit;

namespace cmd
{
    public sealed class Start
    {
        public object _lock;

        public Start() { _lock = new object(); }
        ~Start() { }

        public void WriteCmdStartText(int ms = 0)
        {
            if (ms != 0)
                System.Threading.Thread.Sleep(ms);

            Console.WriteLine("Welcome to cmd!\nThis software is open source & free to use.\nhttps://github.com/DeetonRushy/cmd\n\r");
        }

        // Enters application mainloop.

        public void exec()
        {
            G.context.LoadStartInternalPlugins();
            WriteCmdStartText(0);
            G.RCVars();

            // We load the plugins once we have created the start object.
            // Once loaded, they will be able to access our plugin API
            // ( which is not yet created. ) 
            // All files and commands can then be developed externally.

            // PluginLoader.LoadAllPlugins( ref G.context.PluginWorkload, ref G.context );

            while (true)
            {
                #region InputPointer
                Console.Write(G.line_pointer);
                #endregion

                if (G.restart_needed)
                    break; // Panic has been called.

                var inputReader = new DrInput();

                string command = inputReader.ReadLine( true );
                command.Replace( '\n', ' ' );

                #region InputParsing

                bool exec_w_params = false;
                string[] _work = command.Split();

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
                Console.Title = "Status: " + _Ts.Error + " | " + G.__version__;
                Console.WriteLine();
                #endregion
            }

        }
    }
}

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace cmd
{
    class CommandExecutor : CommandStorage, IDisposable
    {
        public CommandExecutor()
        {
            Instance = this;
        }

        ~CommandExecutor() { }

        public void Dispose()
        {
            SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

            if(_handle?.DangerousGetHandle() != null)
            {
                _handle?.Dispose();
            }
        }

        protected static CommandExecutor Instance
        {
            get;
            private set;
        }

        // The execute function for commands that do not take
        // any arguments.

        public RetType ExecuteCommand(string _rname) // rname = readable name, the name they type.
        {
            if (!Exists(_rname))
            {
                #region ExecuteCommandErrorOutput

                G.Out(RetType._C_FAILURE, $"{_rname} doesn't exist as a command. Try typing 'help'.");
                return RetType._C_FAILURE;

                #endregion
            }

            if (G._case_sensitive)
            {
                return Run(_rname.ToLower());
            }

            return Run(_rname);
        }

        // alternative, for commands with arguements.

        public RetType ExecuteCommand(string _rname, params string[] arguements)
        {
            // we don't need to check if the command exists, the param execute function does it all
            // for us.

            if (G._case_sensitive)
            {
                return Run(_rname.ToLower(), arguements);
            }

            return Run(_rname, arguements);
        }

        // The main function to create a command.

        // Nothing special happens in here, we just return a protected function
        // from command storage.

        public bool MakeCommand(string _typ, string _name, Func<RetType> fmt, string _desc)
        {
            return CreateCommand(_typ, _name, fmt, _desc);
        }

        #region command_definitions

        public RetType _Listcmd()
        {
            RetType result = RetType._C_SUCCESS;

            foreach(KeyValuePair<string, Command> _c in StorageWorkload)
            {
                try
                {
                    Console.WriteLine("Command -> " + _c.Key);
                    Console.WriteLine("Name -> " + _c.Value.Name);
                    Console.WriteLine("Description -> " + _c.Value.Description);
                    Console.WriteLine();
                }
                catch
                {
                    result = RetType._C_ACCESSVIOLATION;
                }
            }

            foreach(KeyValuePair<string, CommandWithArguements> _c in StorageWorkloadArgs)
            {
                try
                {
                    Console.WriteLine("Command -> " + _c.Key);
                    Console.WriteLine("Name -> " + _c.Value.Name);
                    Console.WriteLine("Description -> " + _c.Value.Description);
                    Console.WriteLine();
                }
                catch
                {
                    result = RetType._C_ACCESSVIOLATION;
                }
            }

            return result;
        }

        public RetType _Listcmd_Simple()
        {
            RetType result = RetType._C_SUCCESS;

            foreach (KeyValuePair<string, Command> _c in StorageWorkload)
            {
                try
                {
                    #region _Listcmd_Simple_Output

                    Console.Write("Command -> ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(_c.Key + "\n");
                    Console.ResetColor();
                    Console.Write("Desc    -> ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(_c.Value.Description + "\n");
                    Console.ResetColor();
                    Console.WriteLine();

                    #endregion
                }
                catch
                {
                    result = RetType._C_ACCESSVIOLATION;
                }
            }

            foreach (KeyValuePair<string, CommandWithArguements> _c in StorageWorkloadArgs)
            {
                try
                {
                    #region _Listcmd_Simple_Output_Args

                    Console.Write("Command -> ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(_c.Key + "\n");
                    Console.ResetColor();
                    Console.Write("Desc    -> ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(_c.Value.Description + "\n");
                    Console.ResetColor();
                    Console.WriteLine();

                    #endregion
                }
                catch
                {
                    result = RetType._C_ACCESSVIOLATION;
                }
            }

            return result;
        }


        #endregion
    }
}

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

        // This is the main function that is executes in our main loop.
        // We check if entered command exists & execute it.
        // On failure to exist, we output the translated version of the return type.
        // And give some extra information on the error.

        public RetType ExecuteCommand(string _rname) // rname = readable name, the name they type.
        {
            if (!Exists(_rname))
            {
                G.L.OG("User ran command that doesn't exist.");
                ErrorOutputHandler.Out(RetType._C_FAILURE, $"{_rname} doesn't exist as a command. Try typing 'help'.");
                return RetType._C_FAILURE;
            }

            G.L.OG("Executing " + _rname);
            return Run(_rname);
        }

        // The main function to create a command.

        // Nothing special happens in here, we just return an internal function
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

            return result;
        }


        #endregion
    }
}

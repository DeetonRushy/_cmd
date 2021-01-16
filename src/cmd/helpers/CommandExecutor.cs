using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class CommandExecutor : CommandStorage
    {
        public CommandExecutor()
        {
            Instance = this;
        }

        public CommandExecutor(ref CommandExecutor other, string a, string b, Func<_cmdReturn> c, string d)
        {
            other.CreateCommand(a, b, c, d);
        }

        ~CommandExecutor() { }

        protected static CommandExecutor Instance
        {
            get;
            private set;
        }

        public _cmdReturn ExecuteCommand(string _rname) // rname = readable name, the name they type.
        {
            if (!Exists(_rname))
            {
                G.L.OG("User ran command that doesn't exist.");
                ErrorOutputHandler.Out(_cmdReturn._C_FAILURE, $"{_rname} doesn't exist as a command. Try typing ##help.");
                return _cmdReturn._C_FAILURE;
            }

            G.L.OG("Executing " + _rname);
            return Run(_rname);
        }

        public bool MakeCommand(string _typ, string _name, Func<_cmdReturn> fmt, string _desc)
        {
            return CreateCommand(_typ, _name, fmt, _desc);
        }

        public _cmdReturn _List_Cmd()
        {
            _cmdReturn result = _cmdReturn._C_SUCCESS;

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
                    result = _cmdReturn._C_ACCESSVIOLATION;
                }
            }

            return result;
        }

        public _cmdReturn _List_Cmd_Simple()
        {
            _cmdReturn result = _cmdReturn._C_SUCCESS;

            foreach (KeyValuePair<string, Command> _c in StorageWorkload)
            {
                try
                {
                    #region _List_Cmd_Simple_Output

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
                    result = _cmdReturn._C_ACCESSVIOLATION;
                }
            }

            return result;
        }
    }
}

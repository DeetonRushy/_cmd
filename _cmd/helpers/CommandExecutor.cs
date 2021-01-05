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

        }

        ~CommandExecutor() { }

        public _cmdReturn ExecuteCommand(string _rname) // rname = readable name, the name they type.
        {
            if (!Exists(_rname))
            {
                ErrorOutputHandler.Out(_cmdReturn._C_FAILURE, $"{_rname} doesn't exist as a command.");
                return _cmdReturn._C_FAILURE;
            }

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
    }
}

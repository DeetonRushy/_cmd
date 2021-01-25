using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{

    // _typ = rname = readable name, typeable command.
    class CommandStorage
    {
        public CommandStorage()
        {
            StorageWorkload   = new Dictionary<string, Command>();
            StorageWorkloadArgs = new Dictionary<string, CommandWithArguements>();
        }

        ~CommandStorage() { }

        // The workload for commands without arguements.

        protected IDictionary<string, Command> StorageWorkload; // Main command work load. 

        // The workload for commands that take arguements.

        protected IDictionary<string, CommandWithArguements> StorageWorkloadArgs; // WIP, will hold all commands that have arguements.

        protected bool CreateCommand(string _typ, string _name, Func<RetType> fmt, string _desc)
        {
            if (StorageWorkload.ContainsKey(_typ))
            {
                return true;
            }           

            Command _new = new Command(_name, fmt, _desc);
            StorageWorkload.Add(_typ, _new);

            return StorageWorkload.ContainsKey(_typ);
        }

        /// <summary>
        /// Create a command that accepts string arguements.
        /// </summary>
        /// <param name="_typ">The command name, what to type into the console. (visable)</param>
        /// <param name="inter_name">The internal name that _cmd will recognise the command by. (not visable)</param>
        /// <param name="fmt">The delegate that executes your command/code.</param>
        /// <param name="_desc">Description of your command, this will be displayed on the help command.</param>
        /// <param name="expected">The expected arguements, this can be null if you expect no specific arguements.</param>
        /// <returns></returns>
        protected bool CreateCommand(string _typ, string inter_name, Func<string[], RetType> fmt, string _desc, params string[] expected)
        {
            if (StorageWorkloadArgs.ContainsKey(_typ))
            {
                Console.WriteLine($"Tried to create command with arguements that already exists. Name={inter_name}");
                return true; // return true because the command does exist.
            }

            CommandWithArguements result = new CommandWithArguements(
                _typ, // type-able name.
                inter_name, // internal name
                fmt, // delegate
                _desc, // description
                expected // expected params, **can** be empty.
                );

            StorageWorkloadArgs.Add(_typ, result);

            return StorageWorkloadArgs.ContainsKey(_typ);
        }

        protected RetType Run(string _typ)
        {
            if (G._case_sensitive)
            {
                foreach(KeyValuePair<string, Command> kv in StorageWorkload)
                {
                    if(kv.Key.ToLower() == _typ.ToLower())
                    {
                        return StorageWorkload[kv.Key].Function.Invoke();
                    }
                }
            }

            return StorageWorkload[_typ].Function.Invoke();
        }

        protected RetType Run(string _typ, string[] _params)
        {
            if (!ExistsW(_typ))
            {
                #region RunOverrideErrorOutput

                G.Out(RetType._C_RESOURCE_NOT_EXIST, $"type 'help' for information.");
                return RetType._C_FAILURE;

                #endregion
            }

            if (G._case_sensitive)
            {
                foreach (KeyValuePair<string, Command> kv in StorageWorkload)
                {
                    if (kv.Key.ToLower() == _typ.ToLower())
                    {
                        DelegateHandler _worker = new DelegateHandler(
                             StorageWorkloadArgs[kv.Key].Function,
                             ref _params);

                        _worker.Execute(out RetType _result);
                        return _result;
                    }
                }
            }

            DelegateHandler worker = new DelegateHandler(
                StorageWorkloadArgs[_typ].Function,
                ref _params);

            worker.Execute(out RetType result);
            return result;
        }

        protected bool FlushCommand(string _typ)
        {
            if (!StorageWorkload.ContainsKey(_typ))
            {
                Console.WriteLine("Command doesn't exist.");
                return false;
            }

            return StorageWorkload.Remove(_typ);
        }

        protected Command RetreiveCommand(string _typ)
        {
            if (!StorageWorkload.ContainsKey(_typ))
                return null;

            string __name = StorageWorkload[_typ].Name;
            Func<RetType> __fmt = StorageWorkload[_typ].Function;
            string __desc = StorageWorkload[_typ].Description;

            return new Command(__name, __fmt, __desc);
        }

        protected bool Exists(string _typ)
        {
            if (G._case_sensitive)
            {
                return StorageWorkload.ContainsKey(_typ.ToLower());
            }

            return StorageWorkload.ContainsKey(_typ);
        }

        protected bool ExistsW(string _typ)
        {
            if (G._case_sensitive)
            {
                return StorageWorkloadArgs.ContainsKey(_typ.ToLower());
            }

            return StorageWorkloadArgs.ContainsKey(_typ);
        }
    }
}

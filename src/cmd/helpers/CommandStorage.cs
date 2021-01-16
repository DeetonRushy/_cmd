using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{

    // _typ = rname = readable name, typeable command.
    class CommandStorage
    {
        public CommandStorage()
        {
            StorageWorkload   = new Dictionary<string, Command>();
            cmdWArgs_Workload = new Dictionary<string, CommandWithArguements>();
        }

        ~CommandStorage() { }

        protected IDictionary<string, Command> StorageWorkload;
        protected IDictionary<string, CommandWithArguements> cmdWArgs_Workload;

        protected bool CreateCommand(string _typ, string _name, Func<_cmdReturn> fmt, string _desc)
        {
            if (StorageWorkload.ContainsKey(_typ))
            {
                G.L.OG($"Tried to create command that already exists. Name={_name}");
                return true;
            }           

            Command _new = new Command(_name, fmt, _desc);
            StorageWorkload.Add(_typ, _new);

            return StorageWorkload.ContainsKey(_typ);
        }

        protected _cmdReturn Run(string _typ)
        {
            return StorageWorkload[_typ].Function.Invoke();
        }

        protected bool FlushCommand(string _typ)
        {
            if (!StorageWorkload.ContainsKey(_typ))
                return false;

            G.L.OG("Flushing " + _typ + " from loaded commands.");

            return StorageWorkload.Remove(_typ);
        }

        protected Command RetreiveCommand(string _typ)
        {
            if (!StorageWorkload.ContainsKey(_typ))
                return null;

            G.L.OG("Command has been requested, fetching data...");

            string __name = StorageWorkload[_typ].Name;
            Func<_cmdReturn> __fmt = StorageWorkload[_typ].Function;
            string __desc = StorageWorkload[_typ].Description;

            G.L.OG("Command data found, supplying new object.");

            return new Command(__name, __fmt, __desc);
        }

        protected bool Exists(string _typ)
        {
            return StorageWorkload.ContainsKey(_typ);
        }
    }
}

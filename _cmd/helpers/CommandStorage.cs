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
            StorageWorkload = new Dictionary<string, Command>();
        }

        ~CommandStorage() { }

        protected IDictionary<string, Command> StorageWorkload;

        protected bool CreateCommand(string _typ, string _name, Func<_cmdReturn> fmt, string _desc)
        {
            if (StorageWorkload.ContainsKey(_typ))
                return true;

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

            return StorageWorkload.Remove(_typ);
        }

        protected Command RetreiveCommand(string _typ)
        {
            if (!StorageWorkload.ContainsKey(_typ))
                return null;

            string __name = StorageWorkload[_typ].Name;
            Func<_cmdReturn> __fmt = StorageWorkload[_typ].Function;
            string __desc = StorageWorkload[_typ].Description;

            return new Command(__name, __fmt, __desc);
        }

        protected bool Exists(string _typ)
        {
            return StorageWorkload.ContainsKey(_typ);
        }
    }
}

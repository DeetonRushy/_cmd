using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static G;

namespace cmd
{

    // _typ = rname = readable name, typeable command.
    public class CommandStorage
    {
        public CommandStorage()
        {
            StorageWorkload   = new Dictionary<string, Command>();
            StorageWorkloadArgs = new Dictionary<string, CommandWithArguements>();
        }

        ~CommandStorage() { }

        // The workload for commands without arguements.

        public IDictionary<string, Command> StorageWorkload; // Main command work load. 

        // The workload for commands that take arguements.

        public IDictionary<string, CommandWithArguements> StorageWorkloadArgs; // WIP, will hold all commands that have arguements.

        public bool CreateCommand(string SectionName, Func<RetType> fmt)
        {
            if ( !cfg.Sections.ContainsSection(SectionName) ) {
                return MbErr( $"Requested to read section that does not exist. ({SectionName})", false);
            }

            var _name = G.cfg[SectionName]["Name"];
            var _inam = G.cfg[SectionName]["InternalName"];
            var _desc = G.cfg[SectionName]["Description"];

            if (StorageWorkload.ContainsKey(_name))
            {
                return true;
            }           

            Command _new = new Command(_name, fmt, _desc);
            StorageWorkload.Add(_name, _new);

            return StorageWorkload.ContainsKey(_name);
        }

        /// <summary>
        /// Create a command
        /// </summary>
        /// <param name="_typ"></param>
        /// <returns></returns>
        public RetType Run(string _typ)
        {
            return StorageWorkload[_typ].Function.Invoke();
        }

        public RetType Run(string _typ, string[] _params)
        {
            if (!ExistsW(_typ))
            {
                #region RunOverrideErrorOutput

                Out(RetType._C_FAILURE, $"type 'help' for information.");
                return RetType._C_FAILURE;

                #endregion
            }

            var worker = new DelegateHandler(
                StorageWorkloadArgs[_typ].Function,
                ref _params);

            worker.Execute(out RetType result);
            return result;
        }

        public bool FlushCommand(string _typ)
        {
            if (!StorageWorkload.ContainsKey(_typ))
            {
                Console.WriteLine("Command doesn't exist.");
                return false;
            }

            return StorageWorkload.Remove(_typ);
        }

        public Command RetreiveCommand(string _typ)
        {
            if (!StorageWorkload.ContainsKey(_typ))
                return null;

            string __name = StorageWorkload[_typ].Name;
            Func<RetType> __fmt = StorageWorkload[_typ].Function;
            string __desc = StorageWorkload[_typ].Description;

            return new Command(__name, __fmt, __desc);
        }

        public bool Exists(string _typ)
        {
            return StorageWorkload.ContainsKey(_typ);
        }

        public bool ExistsW(string _typ)
        {
            return StorageWorkloadArgs.ContainsKey(_typ);
        }
    }
}

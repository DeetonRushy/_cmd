using System;
using System.Collections.Generic;
using System.Reflection;

namespace cmd
{

    // _typ = rname = readable name, typeable command.
    public class CommandStorage
    {
        public CommandStorage()
        {
            StorageWorkload   = new Dictionary<string, Command>();
            StorageWorkloadArgs = new Dictionary<string, CommandWithArguements>();
            PluginWorkload = new Dictionary<Assembly, ICmdPlugin>();
        }

        ~CommandStorage() { }

        // The workload for commands without arguements.

        public IDictionary<string, Command> StorageWorkload; // Main command work load. 

        // The workload for commands that take arguements.

        public IDictionary<string, CommandWithArguements> StorageWorkloadArgs; // WIP, will hold all commands that have arguements.


        public IDictionary<Assembly, ICmdPlugin> PluginWorkload;
        public bool CreateCommand(string SectionName, Func<RetType> fmt)
        {
            if ( !G.cfg.Sections.ContainsSection(SectionName) ) {
                return false;
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

        public RetType Run(string _typ, string[] _params)
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
            if (G._case_sensitive)
            {
                return StorageWorkload.ContainsKey(_typ.ToLower());
            }

            return StorageWorkload.ContainsKey(_typ);
        }

        public bool ExistsW(string _typ)
        {
            if (G._case_sensitive)
            {
                return StorageWorkloadArgs.ContainsKey(_typ.ToLower());
            }

            return StorageWorkloadArgs.ContainsKey(_typ);
        }
    }
}

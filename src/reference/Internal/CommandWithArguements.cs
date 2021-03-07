using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    public class CommandWithArguements
    {
        public CommandWithArguements(string _cname, string inter_name, Func<string[], RetType> _func, string _desc, params string[] _args)
        {
            Name         = _cname;
            InternalName = inter_name;
            Function     = _func;
            Description  = _desc;
            Parameters   = _args;
        }

        public string Name
        {
            get;
            private set;
        }

        public string InternalName
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public Func<string[], RetType> Function
        {
            get;
            private set;
        }

        public string[] Parameters
        {
            get;
            private set;
        }
    }
}

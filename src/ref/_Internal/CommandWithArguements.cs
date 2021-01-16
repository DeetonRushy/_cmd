using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class CommandWithArguements
    {
        public CommandWithArguements(string _cname, Delegate _func, string _desc, params string[] _args)
        {
            Name        = _cname;
            Function    = _func;
            Description = _desc;
            Parameters  = _args;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public Delegate Function
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

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

        /// <summary>
        /// The name of the command. (typeable name)
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The internal name used by cmd to refer to the command.
        /// </summary>
        public string InternalName
        {
            get;
            private set;
        }

        /// <summary>
        /// The brief description of the command and it's functionality.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// The delegate that is executed when the command name is typed.
        /// </summary>
        public Func<string[], RetType> Function
        {
            get;
            private set;
        }

        /// <summary>
        /// The parameters that are expected, usually null.
        /// </summary>
        public string[] Parameters
        {
            get;
            private set;
        }
    }
}

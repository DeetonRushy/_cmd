using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class Command
    {
        public Command(string _name, Func<_cmdReturn> fmt, string _desc)
        {
            Name = _name;
            Function = fmt;
            Description = _desc;
        }

        ~Command() { }

        public string Name
        {
            get;
            private set;

        } = "default";

        public Func<_cmdReturn> Function
        {
            get;
            private set;

        } = null;

        public string Description
        {
            get;
            private set;

        } = "default";
    }
}

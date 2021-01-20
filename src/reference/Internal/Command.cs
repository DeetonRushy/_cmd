using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    class Command
    {
        public Command(string _name, Func<RetType> fmt, string _desc, params string[] _Arguements)
        {
            G.L.OG("New command has been created!",
                "#################################################",
                _name,
                fmt.Method.MethodHandle.Value.ToString(),
                _desc,
                "#################################################");

            Name = _name;
            Function = fmt;
            Description = _desc;
            Arguements = _Arguements;
        }

        ~Command() { }

        public string Name
        {
            get;
            private set;

        } = "default";

        public Func<RetType> Function
        {
            get;
            private set;

        } = null;

        public string Description
        {
            get;
            private set;

        } = "default";

        public string[] Arguements
        {
            get;
            private set;

        } = { };
    }
}

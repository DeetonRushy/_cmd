using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    public class Command
    {
        public Command(string _name, Func<RetType> fmt, string _desc, params string[] _Arguements)
        {
            Name = _name;
            Function = fmt;
            Description = _desc;
            Arguements = _Arguements;
        }

        ~Command() { }

        public static bool operator ==(Command a, Command b) => a.Name == b.Name;

        public static bool operator !=(Command a, Command b) => a.Name != b.Name;

        public override bool Equals(object obj)
        {
            try
            {
                return (Command)obj == this; // cast may fail.
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

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

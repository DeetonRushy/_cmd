using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    public class Command
    {
        public Command( string _name, Func<RetType> fmt, string _desc )
        {
            Name = _name;
            Function = fmt;
            Description = _desc;
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

        /// <summary>
        /// The name of the command ( typeable )
        /// </summary>
        public string Name
        {
            get;
            private set;

        } = "default";

        /// <summary>
        /// The function that is executed on the command name being typed.
        /// </summary>
        public Func<RetType> Function
        {
            get;
            private set;

        } = null;

        /// <summary>
        /// The brief description explaining the command and its functionality.
        /// </summary>
        public string Description
        {
            get;
            private set;

        } = "default";

        /// <summary>
        /// The arguments.
        /// </summary>
        [Obsolete]
        public string[] Arguements
        {
            get;
            private set;

        } = { };
    }
}

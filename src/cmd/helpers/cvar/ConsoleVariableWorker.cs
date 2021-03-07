using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd {

    public class CVarContainer {

        public CVarContainer(string name_, string desc_, string init, Func<string[], RetType> OnChange ) {
            Name = name_;
            Description = desc_;
            Value = init;
            OnValueChange = OnChange;
        }

        public string Name { get; }

        public string Description { get; }

        public string Value { get; set; }

        public Func<string[], RetType> OnValueChange { get; }

    }

    public class CVarHost {

        public CVarHost( ) {
            Host = new Dictionary<string, CVarContainer>();
        }

        public bool OnTypedCommand( string name, string[] p ) {

            if ( !Host.ContainsKey ( name ) ) {
                return false;
            }

            if ( p == null ) {
                Console.WriteLine( $"{name}: \"{Host[name].Value}\"\n" );
                return true;
            }

            var value = p[0];
            var rs = Host[name].OnValueChange( p );

            if ( rs == RetType._C_SUCCESS ) {
                Host[name].Value = G.StringArrayToString(p).Trim();
            }
            else {
                G.Out( rs, $"CVar hooked function didn't accept input. ({name})" );
            }

            Console.WriteLine( $"{name}: \"{Host[name].Value}\"\n" );

            return true;
        }

        public void CreateCVar( string name, Func<string[], RetType> OnChange, string initialValue, string description = "default") {

            var _a = new CVarContainer( name, description, initialValue, OnChange );
            Host.Add( name, _a );

            G.L.OG( $"New CVar registered, (name={name}, desc={description})" );
        }

        public bool Exists(string Name ) {
            return Host.ContainsKey( Name );
        }

        public readonly IDictionary<string, CVarContainer> Host;

    }
}

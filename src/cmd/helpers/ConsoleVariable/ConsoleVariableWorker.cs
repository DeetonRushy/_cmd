using System;
using System.Collections.Generic;

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


        /// <summary>
        /// CVarHost constructor, initializes the dictionary that holds CVarContainers.
        /// </summary>
        public CVarHost( ) {
            Host = new Dictionary<string, CVarContainer>();
            G.L.OG( "[cvarhost] initialized!" );
        }

        /// <summary>
        /// Force changes a cvar, can be used to override returning <c>RetType._C_SUCCESS</c>
        /// in your lambda. Instead, return <c>RetType._C_DUMMY_VL</c> if you plan to use this
        /// to set your cvar.
        /// </summary>
        /// <param name="name">The name of the cvar.</param>
        /// <param name="value">The value it will be forced to.</param>
        public void ModifyCVarForce( string name, string value ) {
            if ( !Exists( name ) ) {
                return;
            }

            G.L.OG( $"[cvarhost] {name} is being forcevalued to " + value );

            Host[name].Value = value;
        }

        /// <summary>
        /// This is executed everytime someone types a cvar into the console.
        /// </summary>
        /// <param name="name">The name of the cvar.</param>
        /// <param name="p">The parameters if there. <c>null</c> otherwise.</param>
        /// <returns><c>true</c> if the operation was a success, otherwise <c>false</c>.</returns>
        public bool OnTypedCommand( string name, string[] p ) {

            if ( !Host.ContainsKey ( name ) ) {
                return false;
            }

            if ( p == null ) {
                G.L.OG( "[cvarhost] printing value, user had no argument." );
                Console.WriteLine( $"{name}: \"{Host[name].Value}\"\n" );
                return true;
            }

            var value = p[0];
            G.L.OG( $"[cvarhost] passing {value} to {name}'s lambda." );
            var rs = Host[name].OnValueChange( p );

            if ( rs == RetType._C_SUCCESS ) {
                G.L.OG( $"[cvarhost] {name} accepted input!" );
                Host[name].Value = G.StringArrayToString(p).Trim();
            }
            else {
                G.L.OG( $"[cvarhost] {name} didn't accept the value." );
                G.Out( rs, $"CVar hooked function didn't accept input. ({name})" );
            }

            Console.WriteLine( $"{name}: \"{Host[name].Value}\"\n" );

            return true;
        }

        /// <summary>
        /// Creates a new Cvar available for usage immediately.
        /// </summary>
        /// <param name="name">Desired cvar name.</param>
        /// <param name="OnChange">Function to handle to change of the cvar.</param>
        /// <param name="initialValue">Default value.</param>
        /// <param name="description">Brief description of this cvar.</param>
        public void CreateCVar( string name, Func<string[], RetType> OnChange, string initialValue, string description = "default") {

            var _a = new CVarContainer( name, description, initialValue, OnChange );
            Host.Add( name, _a );

            G.L.OG( $"New CVar registered, (name={name}, desc={description})" );
        }

        /// <summary>
        /// Clears every cvar from the current context.
        /// </summary>
        public void _Reset( ) {
            Host.Clear();
        }

        /// <summary>
        /// Checks if a cvar exists.
        /// </summary>
        /// <param name="Name">The name of the cvar you want to verify exists.</param>
        /// <returns><c>true</c> if exists, otherwise <c>false</c>.</returns>
        public bool Exists(string Name ) {
            return Host.ContainsKey( Name );
        }

        /// <summary>
        /// The container for all CVarContainers.
        /// </summary>
        public readonly IDictionary<string, CVarContainer> Host;

    }
}

using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net;
using LibGit2Sharp;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Speech.Synthesis;
using System.IO;
using System.Linq;

using static G;

namespace cmd {
    public class CommandExecutor : CommandStorage, IDisposable {
        [Obsolete( "Construct the class with no arguments." )]
        public CommandExecutor( [CallerFilePath] string fp = "" ) {
            // redundant
            // code deleted.
        }

        public CommandExecutor( ) {

        }

        ~CommandExecutor( ) { }

        public void Dispose( ) {
            SafeHandle _handle = new SafeFileHandle( IntPtr.Zero, true );

            if ( _handle?.DangerousGetHandle() != null ) {
                _handle?.Dispose();
            }
        }

        // The execute function for commands that do not take
        // any arguments.

        public RetType ExecuteCommand( string _rname ) // rname = readable name, the name they type.
        {
            // make sure the command exists.

            if ( !Exists( _rname ) ) {
                #region ExecuteCommandErrorOutput

                // the command does not exist, return failure.
                return RetType._C_FAILURE;

                #endregion
            }

            // run the command.
            return Run( _rname );
        }

        // alternative, for commands with arguements.

        public RetType ExecuteCommand( string _rname, params string[] arguments ) {
            // we don't need to check if the command exists, the param execute function does it all
            // for us.

            return Run( _rname, arguments );
        }

        // The main function to create a command.

        // Nothing special happens in here, we just return a protected function
        // from command storage.

        public bool MakeCommand( string SectionName, Func<RetType> fmt ) {
            return CreateCommand( SectionName, fmt );
        }

        /// <summary>
        /// Create a command that accepts string arguements.
        /// </summary>
        /// <param name="_typ">The command name, what to type into the console. (visable)</param>
        /// <param name="inter_name">The internal name that _cmd will recognise the command by. (not visable)</param>
        /// <param name="fmt">The delegate that executes your command/code.</param>
        /// <param name="_desc">Description of your command, this will be displayed on the help command.</param>
        /// <param name="expected">The expected arguments, this can be null if you expect no specific arguements.</param>
        /// <returns></returns>
        public bool CreateCommand( string SectionName, Func<string[], RetType> fmt ) {

            if ( !G.cfg.Sections.ContainsSection( SectionName ) ) {
                return false;
            }

            var _typ = G.cfg[SectionName]["Name"];
            var inter_name = G.cfg[SectionName]["InternalName"];
            var _desc = G.cfg[SectionName]["Description"];

            if ( StorageWorkloadArgs.ContainsKey( _typ ) ) {
                return true; // return true because the command already exists.
            }

            CommandWithArguements result = new CommandWithArguements(
                _typ, // type-able name.
                inter_name, // internal name
                fmt, // delegate
                _desc, // description
                null // expected params, **can** be empty.
                );

            StorageWorkloadArgs.Add( _typ, result );

            return StorageWorkloadArgs.ContainsKey( _typ );
        }

        #region command_definitions

        public RetType __cmd_ls( ) {
            foreach ( KeyValuePair<string, Command> _command in StorageWorkload ) {
                Console.WriteLine( $"{_command.Value.Name} = {_command.Value.Description}" );
            }

            foreach ( KeyValuePair<string, CommandWithArguements> _command in StorageWorkloadArgs ) {
                Console.WriteLine( $"{_command.Value.Name} - {_command.Value.Description}" );
            }

            return RetType._C_SUCCESS;
        }

        public void LoadStartInternalPlugins( ) {

            #region HELP

            G.context.MakeCommand( "STD_HELP", ( ) => {
                return G.context.ExecuteCommand( "help", "/?" );

            } );

            #endregion

            #region HELP_ARGS

            G.context.CreateCommand( "STD_HELP", ( args ) => {

                string _help( ) {
                    return "LISTING ARGUMENTS\n\n- /?\n- /list\n- /basic\n- /cdirec\n";
                }

                if ( args[0] == "/?" ) {
                    // output info about available commands.

                    Console.WriteLine( _help() );

                    return RetType._C_SUCCESS;
                }

                Parser arg_work = new Parser( args, false );

                #region InfoSetup

                bool basic = false;
                bool cdirec = false;
                bool list = false;

                #endregion

                arg_work
                     .add_optional( "/basic", ref basic, true )
                     .add_optional( "/cdirec", ref cdirec, true )
                     .add_optional( "/list", ref list, true );

                if ( basic ) {
                    #region basic_information_output

                    Console.WriteLine( "\nThis is an extremely flexable command prompt that supports" );
                    Console.WriteLine( "plugins, single commands, commands with arguements." );
                    Console.WriteLine( "It's also open source, it can be found here : https://github.com/DeetonRushy/_cmd" );

                    return RetType._C_SUCCESS;

                    #endregion
                }

                if ( cdirec ) {
                    #region cdirec_information

                    Console.WriteLine( "\nThe command is '%cdirec'. It outputs the current location of this binary." );

                    return RetType._C_SUCCESS;

                    #endregion
                }

                if ( list ) {
                    #region list_information

                    Console.WriteLine( "Type 'lscmd' to view all active commands." );
                    return RetType._C_SUCCESS;

                    #endregion
                }


                return RetType._C_SUCCESS;

            } );

            #endregion

            #region LIST_CMD

            G.context.MakeCommand( "STD_LIST_COMMANDS", ( ) => {

                return G.context.__cmd_ls();

            } );

            #endregion

            #region ECHO_SINGLE

            G.context.MakeCommand( "STD_ECHO", ( ) => { return G.context.ExecuteCommand( "echo", "This", "command", "takes", "your", "text!" ); } );

            #endregion

            #region CLEAR

            G.context.MakeCommand( "STD_CLEAR", ( ) => {
                Console.Clear();
                return RetType._C_SUCCESS;

            } );

            #endregion

            #region ECHO

            G.context.CreateCommand( "STD_ECHO", ( args ) => {

                Console.WriteLine();
                Console.WriteLine( G.StringArrayToString( args ) );
                Console.WriteLine();

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region TIME

            G.context.MakeCommand( "STD_TIME", ( ) => {

                var current = DateTime.Now;

                Console.WriteLine( "Current time: " + current.ToLongTimeString() );

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region RESET

            G.context.MakeCommand( "STD_CONTEXT_RESET", ( ) => {
                G.Panic();
                return RetType._C_SUCCESS;

            } );

            #endregion

            #region NAME

            G.context.MakeCommand( "STD_NAME", ( ) => {
                Console.WriteLine();
                Console.WriteLine( Environment.UserName );
                return RetType._C_SUCCESS;

            } );

            #endregion

            #region VERSION

            G.context.MakeCommand( "STD_VERSION", ( ) => {
                string _ver = null;
                _ver = G.__version__;

                if ( _ver == null )
                    return RetType._C_ACCESSVIOLATION;

                Console.WriteLine();
                Console.WriteLine( "cmd current version is " + _ver );

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region CD

            G.context.CreateCommand( "STD_CHANGE_DIRECTORY", ( d_info ) => {

                G.host.OnTypedCommand( "cmd.directory", d_info );
                return RetType._C_DUMMY_VL;

            } );

            #endregion

            #region COLOR

            G.context.CreateCommand( "STD_COLOUR", ( c ) => {
                if ( c[0] == "help" ) {
                    Console.WriteLine( "EXAMPLE: color front blue" );
                    Console.WriteLine( "EXAMPLE: color back pink" );
                    return RetType._C_SUCCESS;
                }

                if ( c.Length != 2 ) {
                    G.Out( RetType._C_FAILURE, "Try 'color help'." );
                    return RetType._C_FAILURE;
                }

                ConsoleColor final = ConsoleColor.White;
                bool type = false; // false is back, true is front

                if ( c[0] == "front" )
                    type = false;

                switch ( c[1] ) {
                case "red":
                    final = ConsoleColor.Red;
                    break;
                case "blue":
                    final = ConsoleColor.Blue;
                    break;
                case "green":
                    final = ConsoleColor.Green;
                    break;
                case "pink":
                    final = ConsoleColor.Magenta;
                    break;
                case "reset":
                    Console.ResetColor();
                    return RetType._C_SUCCESS;
                default:
                    G.Out( RetType._C_FAILURE, "color isn't implemented." );
                    return RetType._C_FAILURE;
                }

                if ( type ) {
                    Console.ForegroundColor = final;
                }
                else {
                    Console.BackgroundColor = final;
                }

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region CVARLIST

            G.context.MakeCommand( "STD_LIST_CONSOLE_VARIABLES", ( ) => {
                Console.WriteLine();

                foreach ( KeyValuePair<string, CVarContainer> kvp in G.host.Host ) {
                    Console.WriteLine( $"\"{kvp.Value.Name}: \"{kvp.Value.Value}\". \n\"{kvp.Value.Description}\"\n" );
                }

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region GIT

            G.context.CreateCommand( "STD_GIT", ( args ) => {

                if ( args[0] == "--help" ) {
                    Console.WriteLine( "git clone <link> <path>\nPath can be 'default' and path will be C:\\Users\\<name>\\<repo-name>" );
                    return RetType._C_SUCCESS;
                }

                if ( args.Length != 3 ) {
                    return RetType._C_INVALID_PARAMS;
                }

                if ( args[0] != "clone" ) {
                    return RetType._C_RESOURCE_NOT_EXIST;
                }

                string url = args[1];
                string path = args[2];

                if ( path == "default" ) {
                    path = "C:\\Users\\" + Environment.UserName;
                }

                if ( !/*System.IO.*/Directory.Exists( path ) ) {
                    return RetType._C_INVALID_PARAMS;
                }

                using ( WebClient wc = new WebClient() ) {
                    try {
                        wc.DownloadFile( url, $"data\\temp\\{G.RandomString( 25 )}.tmp" );
                    }
                    catch ( ArgumentNullException ) {
                        return RetType._C_INVALID_PARAMS;
                    }
                    catch ( WebException ) {
                        return RetType._C_INVALID_PARAMS;
                    }
                    catch ( NotSupportedException ) {
                        return RetType._C_INVALID_PARAMS;
                    }
                    catch {
                        return RetType._C_INVALID_PARAMS;
                    }

                }

                // all checks have succeeded, let just attempt to clone it.

                string prjName = string.Empty;
                string[] spl = url.Split( '/' );
                prjName = spl[spl.Length - 1].Split( '.' )[0];

                System.IO.DirectoryInfo _a = null;

                try {
                    _a = System.IO.Directory.CreateDirectory( System.IO.Path.Combine( path, prjName ) );
                }
                catch {
                    Console.WriteLine( $"GIT: ERR: Access to path ({System.IO.Path.Combine( path, prjName )}) is denied." );
                    return RetType._C_ACCESSVIOLATION;
                }

                try {
                    Repository.Clone( url, _a.FullName );
                }
                catch ( RecurseSubmodulesException ex ) {
                    Console.WriteLine( "GIT: ERR: " + ex.Message + ". For help, go to " + ex.HelpLink );
                    return RetType._C_FAILURE;
                }
                catch ( UserCancelledException ex ) {
                    Console.WriteLine( "GIT: ERR: " + ex.Message + ". For help, go to " + ex.HelpLink );
                    return RetType._C_FAILURE;
                }

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region POINT_STAT

            G.context.CreateCommand( "STD_CSTAT", ( ) => {

                string _it = string.Empty;

                using ( WebClient wc = new WebClient() ) {
                    try {
                        _it = wc.DownloadString( "https://raw.githubusercontent.com/DeetonRushy/cmd/master/build/rel_stab_stat.txt" );
                    }
                    catch {
                        _it = "Unknown";
                    }
                }

                ConsoleColor c = (_it == "Stable") ? ConsoleColor.Red : ConsoleColor.Green;

                Console.Write( "Status: " );
                Console.ForegroundColor = c;
                Console.Write( _it );
                Console.ResetColor();

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region CMD_UPDATE

            G.context.CreateCommand( "STD_UPDATE", ( ) => {

                float ex_version;
                float in_version = float.Parse( G.__version__ );

                using ( WebClient wc = new WebClient() ) {
                    try {
                        ex_version = float.Parse( wc.DownloadString( "https://raw.githubusercontent.com/DeetonRushy/cmd/master/build/rel_upd_ver.txt" ).Trim() );
                    }
                    catch {
                        ex_version = -1.0f;
                    }
                }

                if ( ex_version == in_version ) {
                    Console.WriteLine( $"Local version is up to date! ({ex_version})" );
                    return RetType._C_SUCCESS;
                }

                if ( ex_version > in_version ) {
                    Console.WriteLine( $"Current build is out of date! [Local({in_version}), Stable({ex_version})]" );
                    return RetType._C_SUCCESS;
                }

                if ( ex_version < in_version ) {
                    G.CriticalError( "The current build is somehow ahead in versions than any stable. This build may be tampered with." );
                }

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region STRACE

            G.context.MakeCommand( "STD_STACK_TRACE", ( ) => {

                var trace = new StackTrace();
                int i = 0;
                string _this = "(strace)";

                foreach ( StackFrame frame in trace.GetFrames() ) {
                    Console.Write( $"[(IL Offset): {frame.GetILOffset()}] {frame.GetMethod().Name} " );
                    if ( i == 0 ) {
                        Console.Write( _this );
                    }
                    i++;
                    Console.WriteLine();
                }

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region CLEAR_TEMP

            G.context.CreateCommand( "STD_CLEAR_TEMP", ( ) => {

                foreach ( var fl in System.IO.Directory.GetFiles( "data\\temp", "*.*" ) ) {

                    try {
                        System.IO.File.Delete( fl );
                    }
                    catch {
                        continue;
                    }
                }

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region WGET

            G.context.CreateCommand( "STD_WEB_GET", ( args ) => {

                if ( args[0] == "--help" ) {
                    Console.WriteLine( "wget <link> <path>" );
                    return RetType._C_SUCCESS;
                }

                if ( args.Length != 2 ) {
                    return RetType._C_INVALID_PARAMS;
                }

                using ( WebClient tmpwc = new WebClient() ) {

                    try {
                        tmpwc.DownloadFile( args[0], $"data\\temp\\{G.RandomString( 25 )}.tmp" );
                    }
                    catch ( WebException ex ) {
                        Console.WriteLine( $"{args[0]} is not a valid weblink, extra info: " + ex.Message );
                    }
                    catch {
                        Console.WriteLine( $"{args[0]} is not a valid weblink." );
                        return RetType._C_INVALID_PARAMS;
                    }

                }

                if ( System.IO.File.Exists( args[1] ) ) {

                    if ( args[1] != "default" ) {
                        Console.WriteLine( $"File already exists. ({args[1]}) or directory doesn't." );
                        return RetType._C_SYSTEM_ERROR;
                    }

                    args[1] = G.host.Host["cmd.directory"].Value;
                }

                // all should be good, lets download it.

                using ( WebClient wc = new WebClient() ) {
                    try {
                        wc.DownloadFile( args[0], args[1] );
                    }
                    catch ( WebException wex ) {
                        Console.WriteLine( $"There was an error when attempting to download the file. [{wex.Message}([{wex.Status}])]" );
                        return RetType._C_FAILURE;
                    }

                    Console.WriteLine( "Success, file written " + args[1] + " to " + G.host.Host["cmd.directory"].Value ); ;

                }

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region TTS

            G.context.CreateCommand( "STD_TEXT_TO_SPEECH", ( args ) => {
                var str_to_speak = G.StringArrayToString( args );

                var worker = new SpeechSynthesizer();
                worker.SetOutputToDefaultAudioDevice();
                worker.Speak( str_to_speak );

                worker.Dispose();

                return RetType._C_SUCCESS;
            } );

            #endregion

            #region LS

            G.context.CreateCommand( "STD_LIST_DIRECTORY", ( ) => {

                var cdirc = G.host.Host["cmd.directory"].Value;

                Console.WriteLine( $"[({cdirc})]" );

                string cvt( string fp ) {
                    var _a = fp.Split( '\\' );
                    return _a[_a.Length - 1];
                }

                IDictionary<string, DirectoryInfo> _FolderInformation = new Dictionary<string, DirectoryInfo>();

                foreach ( string _i in Directory.GetDirectories( cdirc ) ) {
                    DirectoryInfo info = new DirectoryInfo( _i );
                    _FolderInformation.Add( _i, info );
                }

                foreach ( KeyValuePair<string, DirectoryInfo> inf in _FolderInformation ) {
                    string Parent = String.IsNullOrEmpty( inf.Value.Parent.Name ) ? inf.Value.Parent.Name : "No Parent";
                    Console.WriteLine( $"[folder] {cvt( inf.Key )}     | ({Parent})  | {inf.Value.LastWriteTime}" );
                }

                IDictionary<string, FileInfo> _FileInformation = new Dictionary<string, FileInfo>();

                foreach ( string _i in Directory.GetFiles( cdirc ) ) {
                    FileInfo info = new FileInfo( _i );
                    _FileInformation.Add( _i, info );
                }

                foreach ( KeyValuePair<string, FileInfo> inf in _FileInformation ) {
                    Console.WriteLine( $"[file] {cvt( inf.Key )}    | {inf.Value.Length}b  | {inf.Value.LastWriteTime}" );
                }

                return RetType._C_SUCCESS;
            } );

            #endregion

            #region TOUCH

            context.CreateCommand( "STD_TOUCH", ( args ) => {

                string _arg = args[0];

                if ( File.Exists( Path.Combine( Environment.CurrentDirectory, _arg ) ) ) {
                    File.SetLastAccessTime( Path.Combine( Environment.CurrentDirectory, _arg ), DateTime.Now );
                    return RetType._C_SUCCESS;
                }

                try {
                    File.Create( Path.Combine( Environment.CurrentDirectory, _arg ) ).Dispose();
                }
                catch {
                    G.Out( RetType._C_IOERROR, "Access is denied." );
                    return RetType._C_IOERROR;
                }

                return RetType._C_SUCCESS;

            } );

            #endregion

            #region DEL - DELETE FILE

            context.CreateCommand( "FS_OP_DEL", ( args ) => {
                string FileName = args[0]; // expected.
                bool addpath = false;

                if ( args.Length == 2 ) {
                    if ( args[1] == "-P" ) {
                        addpath = true;
                    }
                }

                if ( addpath ) {
                    FileName = Environment.CurrentDirectory + $"\\{FileName}";
                }

                if ( !File.Exists( FileName ) ) {
                    return RetType._C_INVALID_PARAMS;
                }

                File.Delete( FileName );

                return RetType._C_SUCCESS;
            } );

            #endregion

            context.CreateCommand( "THIS_IS_A_TEST", ( ) => RetType._C_SUCCESS );
        }

        #endregion
    }
}

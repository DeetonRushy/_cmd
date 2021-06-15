using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using cmd;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

public static class G {

    public static GOG l = new GOG( "data/logs" );

#region MB_ERROR

    public static T MbErr<T>(string msg, T v ) {
        MessageBox.Show( msg + $"\n\n{GetStackTraceFmt()}", "error" );
        return v;
    }

#endregion

    // Returns a string[] as a string.
#region String Array Converter

    public static string StringArrayToString( string[] arr ) {
        string res = "";

        foreach ( string _val in arr ) {
            res += _val + " ";
        }

        return res.Trim();
    }

    #endregion

    // Utility command, returns a random string with specified length.
#region RandomString

    private static Random random = new Random();
    public static string RandomString( int length ) {
        const string chars = "abcdefghijlmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string( Enumerable.Repeat( chars, length )
          .Select( s => s[random.Next( s.Length )] ).ToArray() );
    }

    #endregion

    // The context for our console variables.
#region Console Variable Host & Initialization.

    public static CVarHost host = new CVarHost();

    public static void RCVars( ) {
        host.CreateCVar( "console.bufferwidth", ( _arg ) => {

            if ( !int.TryParse( _arg[0], out int arg ) ) {
                return RetType._C_FAILURE;
            }

            if ( arg < Console.BufferWidth || arg >= short.MaxValue ) {
                return RetType._C_FAILURE;
            }

            Console.BufferWidth = ( short ) arg;
            return RetType._C_SUCCESS;

        }, Console.BufferWidth.ToString(), "The buffer-width size." );

        host.CreateCVar( "console.title", ( _arg ) => {
            Console.Title = StringArrayToString( _arg ) + $" | {__version__} | {MD5_CHECKSUM}";
            return RetType._C_SUCCESS;

        }, Console.Title, "The console title." );

        // create a thread to update the title cvar

        var titleWorker = new Thread( ( ) => {

            while ( true ) {
                var original = Console.Title;

                if ( original != Console.Title ) {
                    host.ModifyCVarForce( "console.title", Console.Title );
                    original = Console.Title;
                }

                Thread.Sleep( 10 );
            }

        } );

        titleWorker.Start();

        host.CreateCVar( "cmd.pointer", ( _arg ) => {
            string _new = StringArrayToString( _arg );

            if ( !(_new.Length <= 25) ) {
                return RetType._C_FAILURE;
            }

            line_pointer = _new;
            return RetType._C_SUCCESS;
        }, line_pointer, "The prompt text." );

        host.CreateCVar( "console.foregroundcolor", ( args ) => {

            ConsoleColor GetColor( string str ) {
                switch ( str ) {
                case "red":
                    return ConsoleColor.Red;
                case "green":
                    return ConsoleColor.Green;
                case "blue":
                    return ConsoleColor.Blue;
                case "cyan":
                    return ConsoleColor.Cyan;
                case "yellow":
                    return ConsoleColor.Yellow;
                case "purple":
                    return ConsoleColor.Magenta;
                case "-reset":
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
                }
            }

            if ( GetColor( args[0] ) == ConsoleColor.White ) {

                Console.ForegroundColor = ConsoleColor.White;
                host.ModifyCVarForce( "Console.ForegroundColor", "default-white" );
                return RetType._C_DUMMY_VL;
            }

            Console.ForegroundColor = GetColor( args[0] );

            return RetType._C_SUCCESS;

        }, "white", "The console foreground color." );

        host.CreateCVar( "console.cursorsize", ( args ) => {
            if ( !int.TryParse( args[0], out int _res ) ) {
                return RetType._C_ACCESSVIOLATION;
            }

            try {
                Console.CursorSize = _res;
            }
            catch {
                return RetType._C_DISABLED;
            }

            return RetType._C_SUCCESS;
        }, Console.CursorSize.ToString(), "The console cursor size." );

        host.CreateCVar( "console.cursorvisible", ( args ) => {

            if ( args[0].ToLower() == "true" ) {
                Console.CursorVisible = true;
                return RetType._C_SUCCESS;
            }
            else {
                Console.CursorVisible = false;
                host.ModifyCVarForce( "Console.CursorVisible", "false" );
                return RetType._C_DUMMY_VL;
            }
        }, Console.CursorVisible.ToString(), "The boolean determining the visability of the cursor." );

        host.CreateCVar( "cmd.directory", ( args ) => {

            var directory = args[0];

            if ( System.IO.Directory.Exists( directory ) ) {
                Environment.CurrentDirectory = directory;
                host.ModifyCVarForce( "cmd.directory", Environment.CurrentDirectory );
                var dr = Environment.CurrentDirectory.Replace( '\\', '/' );
                line_pointer = $"{Environment.UserName}@{Environment.MachineName}:{dr}~$ ";
                return RetType._C_DUMMY_VL;
            }

            return RetType._C_INVALID_PARAMS;

        }, Environment.CurrentDirectory, "The current directory context of cmd." );
    }

    #endregion

    // Shows a messagebox with information, then exits.
    #region Critical Error Helper

    public static string GetStackTraceFmt( ) {
        string errorInformation = "StackTrace Below (may not be needed, error occured.)\n";

        foreach ( StackFrame frame in new StackTrace().GetFrames() ) {
            errorInformation += "\n\n" + $"[{frame.GetMethod()}({frame.GetFileLineNumber()})]: {frame.GetMethod()}+{frame.GetNativeOffset()}";
        }

        return errorInformation;
    }

    public static void CriticalError( string message, [CallerMemberName] string name = "", [CallerLineNumber] int line = 0 ) {

        var errorInformation = GetStackTraceFmt();

        errorInformation += "\n\n" + message;

        switch( MessageBox.Show( errorInformation, "Error", MessageBoxButtons.AbortRetryIgnore ) ) {
        case DialogResult.Retry:
            MessageBox.Show( "This could result in a stackoverflow." );
            __on_start();
            break;
        case DialogResult.Abort:
            break;
        case DialogResult.Ignore:
            return;
        }

        Environment.Exit( -0xF );
    }

    #endregion

    // Configuration data. Set in __on_start.
#region Configuration Data

    public static IniData cfg;

    #endregion

    // work that is done instantly when the program starts.
#region Work to be done instantly when the program is loaded

    private static string DEFAULT_CONFIG_PATH = $"data\\config\\default_config.ini";

    private static string MD5_CHECKSUM = string.Empty;

    public static void __on_start() {

        Console.WriteLine( Directory.GetCurrentDirectory() );

        // We need to get the Md5 checksum of this file.

        var Parser = new FileIniDataParser();

        if ( !File.Exists( DEFAULT_CONFIG_PATH ) ) {

            Console.WriteLine( $"Failed to find default configuration file. [{DEFAULT_CONFIG_PATH}]" );
            CriticalError( $"Failed to find default configuration file. [{DEFAULT_CONFIG_PATH}]" );

        }

        cfg = Parser.ReadFile( DEFAULT_CONFIG_PATH );

        Verify( () => cfg != null, ( ) => {
            CriticalError( "Found configuration file, but object is NULL. Cannot proceed.\n" + 
                "Make sure data\\config\\default_config.ini has not been deleted." + 
                "\nIf it has, re-install cmd." + 
                "\nRun cmd with --reinstall to complete this, or use the installer.");
            return -1;
        } );

        using ( var md5 = MD5.Create() ) {

            string _md5 = string.Empty;

            // Simply crash if the main executable isn't present.
            // QUESTION: What the fuck is running? They need to re-name the file.

            using ( var stream = File.OpenRead( "cmd_NonGit.exe" ) ) {
                _md5 = BitConverter.ToString( md5.ComputeHash( stream ) ).Replace( "-", "" ).ToLowerInvariant();
            }

            using ( WebClient wc = new WebClient() ) {
                string fresh_md5 = wc.DownloadString( cfg["Int"]["Uri"] );

                if ( !(_md5 == fresh_md5) ) {
                    Console.WriteLine( "FILE INTEGRITY CANNOT BE VERIFIED, PROCEED WITH CAUTION." );
#if !DEBUG
                    CriticalError( $"File integrity cannot be verified. ({_md5} != {fresh_md5}).\nIf you're a developer modifying this, go into __on_start and modify the verification." );
#endif
                }

                MD5_CHECKSUM = _md5;
                Console.Title = $"Cmd | {__version__} -- {_md5}";
            }
        }

        AppDomain.CurrentDomain.ProcessExit += ( arg0, arg1 ) => {
            Console.WriteLine( "Program has began shutdown, goodbye!" );
            System.Diagnostics.Debug.WriteLine( "Program has shutdown, goodbye!" );
        };

        AppDomain.CurrentDomain.UnhandledException += ( a1, a2 ) => {
            CriticalError( $"There was an unhandled exception. ({a2.ExceptionObject})" );
        };

        // Start the watcher that checks for crashes.

        ProcessStartInfo _info = new ProcessStartInfo {
            FileName = "data\\helpers\\watcher.exe",
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        try {
            Process.Start( _info );
        }
        catch {
            // Oh well, no watcher.

            // It's not needed, more of a QOL feature,
            // So don't stress it too much.
        }
    }

#endregion

    // our command executor context, here as a global so it can be passed into plugins.
#region ExecutionContext

    public static CommandExecutor context = new CommandExecutor();

#endregion

    // Utility command, checks if (T) is null.
#region IsNull

    public static bool IsNull<T>( T var ) {
        return (var == null);
    }

#endregion

    // Utility command, compares two types. (T1) == (T2)
#region Compare

    public static bool Compare<T1, T2>( T1 one, T2 two ) {
        return one.Equals( two );
    }

#endregion

    // Utility command, returns value at said index in said array.
#region IndexAt

    public static T IndexAt<T>( ref T[] arr, int index ) {
        return arr[index];
    }

#endregion

    // The global variable for our version.
#region Version

    public static string __version__ => cfg["Global"]["Version"];

#endregion

    // Removes a region from an array.
#region RemoveAt

    public static T[] RemoveAt<T>( T[] source, int index ) // Quite expensive
    {
        T[] dest = new T[source.Length - 1];

        if ( index > 0 )
            Array.Copy( source, 0, dest, 0, index );

        if ( index < source.Length - 1 ) {
            Array.Copy( source, (index + 1), dest, index, (source.Length - index - 1) );
        }

        return dest;
    }

#endregion

    // The program instance, our Start object.
#region Instance

    public static Start _main = new Start();

#endregion

    // Panic, call if something goes terribly wrong. 
#region Panic

    public static bool restart_needed = false;

    public static void Panic( ) {
        // panic basically resets the entire running instance, seen above.
        Console.Clear();
        restart_needed = true;
    }

#endregion

    // The pointer text on the command line.
#region PointerText

    public static string line_pointer = $"{Environment.UserName}@{Environment.MachineName}:{Environment.CurrentDirectory}~$ ";

#endregion

    // Our handler for errors, call with return type & extra info if needed.

#region ErrorOutput

    public static void Out( RetType status, string additional = "" ) {

        if ( status == RetType._C_DUMMY_VL ) {
            return;
        }

        ErrorTranslator unit = new ErrorTranslator( status );

        string err = unit.Error;
        string time = unit.TranslationTime.ToShortTimeString();

        if ( additional == "" )
            additional = "No additional information provided.";

#region out

        Console.WriteLine();
        Console.WriteLine( $"[{time}] {err} | {additional}" );
        Console.WriteLine();

#endregion
    }

#endregion

    // Global variable for wether we're running case sensitive or not.
#region CaseSensitive

    [Obsolete]
    public static bool _case_sensitive = true;

#endregion

    // Our __LINE__ & __FILE__. Like the C macros.
#region FILE_LINE

    public static int __LINE__([CallerLineNumber] int lineNumber = 0 ) {
        return lineNumber;
    }

    public static string __FILE__([CallerFilePath] string fp = "" ) {
        return fp;
    }

    #endregion

    // Verify a condition, complete an action on failure.
#region VERIFY

    public const int VERIFIED = -0xFFFFFF7;

    public static int Verify(Func<bool> Result, Func<int> Action) {
        if ( !Result() ) {
            return Action();
        }

        return VERIFIED;
    }

    #endregion
}

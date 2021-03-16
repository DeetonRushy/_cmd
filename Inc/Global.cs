using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using IniParser;
using IniParser.Model;
using cmd;

public static class G {

    // Global instance of our file logger.
    #region Logger Instance

    public static GOG L = new GOG( "data\\logs" );

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

            L.OG( "[console.bufferwidth] allowing edit - " + arg );

            Console.BufferWidth = ( short ) arg;
            return RetType._C_SUCCESS;

        }, Console.BufferWidth.ToString(), "The buffer-width size." );

        host.CreateCVar( "console.title", ( _arg ) => {

            L.OG( "[console.title] allowing edit - " + StringArrayToString( _arg ) );
            Console.Title = StringArrayToString( _arg );
            return RetType._C_SUCCESS;

        }, Console.Title, "The console title." );

        host.CreateCVar( "cmd.pointer", ( _arg ) => {
            string _new = StringArrayToString( _arg );

            if ( !(_new.Length <= 25) ) {
                return RetType._C_FAILURE;
            }

            L.OG( "[cmd.pointer] allowing edit - " + _new );

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

                L.OG( "[console.foregroundcolor] allowing -reset" );

                Console.ForegroundColor = ConsoleColor.White;
                host.ModifyCVarForce( "Console.ForegroundColor", "default-white" );
                return RetType._C_DUMMY_VL;
            }

            L.OG( "[console.foregroundcolor] allowing edit - " + GetColor( args[0] ) );
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

            L.OG( "[console.cursorsize] accepted size - " + _res );

            return RetType._C_SUCCESS;
        }, Console.CursorSize.ToString(), "The console cursor size." );

        host.CreateCVar( "console.cursorvisible", ( args ) => {

            if ( args[0].ToLower() == "true" ) {
                G.L.OG( "[console.cursorvisible] allowing edit - True" );
                Console.CursorVisible = true;
                return RetType._C_SUCCESS;
            }
            else {
                Console.CursorVisible = false;
                G.L.OG( "[console.cursorvisible] allowing edit - True" );
                host.ModifyCVarForce( "Console.CursorVisible", "false" );
                return RetType._C_DUMMY_VL;
            }
        }, Console.CursorVisible.ToString(), "The boolean determining the visability of the cursor." );

        // we create a seperate thread for updating the value of cmd.tickcount

        host.CreateCVar( "cmd.directory", ( args ) => {

            var directory = args[0];

            if ( System.IO.Directory.Exists( directory ) ) {
                Environment.CurrentDirectory = directory;
                host.ModifyCVarForce( "cmd.directory", directory );
                return RetType._C_DUMMY_VL;
            }

            return RetType._C_INVALID_PARAMS;

        }, Environment.CurrentDirectory, "The current directory context of cmd." );
    }

    #endregion

    // Shows a messagebox with information, then exits.
    #region Critical Error Helper

    public static void CriticalError(string message, int errcode = -1) {
        MessageBox.Show( message, "Critical Error", MessageBoxButtons.OK );
        Environment.Exit( errcode );
    }

    #endregion

    // Configuration data. Set in __on_start.
    #region Configuration Data

    public static IniData cfg;

    #endregion

    // work that is done instantly when the program starts.
    #region Work to be done instantly when the program is loaded

    private const string DEFAULT_CONFIG_PATH = "data\\config\\default_config.ini";

    public static void __on_start() {

        AppDomain.CurrentDomain.ProcessExit += ( arg0, arg1 ) => {
            Console.WriteLine( "Program has began shutdown, goodbye!" );
            System.Diagnostics.Debug.WriteLine( "Program has shutdown, goodbye!" );
        };

        #region Parse default configuration values.

        var Parser = new FileIniDataParser();

        if ( !System.IO.File.Exists( DEFAULT_CONFIG_PATH ) ) {
            CriticalError( $"Failed to find default configuration file. [{DEFAULT_CONFIG_PATH}]" );
        }

        cfg = Parser.ReadFile( DEFAULT_CONFIG_PATH );

        // now we setup.

        Console.Title = cfg["Interface"]["Title"];
        __version__ = cfg["Global"]["Version"];

        #endregion

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

    public static string __version__ = "1.0.4";

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

    public static string line_pointer = $"{Environment.UserName}@{Environment.MachineName}~$ ";

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
        Console.WriteLine( $"{err} at {time} | Additional Info: {additional}" );
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
}

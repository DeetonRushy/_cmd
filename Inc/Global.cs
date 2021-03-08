using System;
using System.Linq;

namespace cmd
{
    public class G
    {
        #region LoggerInstance

        public static GOG L = new GOG("data\\logs");

        #endregion

        #region str_arr_to_str

        public static string StringArrayToString( string[] arr ) {
            string res = "";

            foreach ( string _val in arr ) {
                res += _val + " ";
            }

            return res;
        }

        #endregion

        #region RandomString

        private static Random random = new Random();
        public static string RandomString( int length ) {
            const string chars = "abcdefghijlmnopqrstuvwxyz";
            return new string( Enumerable.Repeat( chars, length )
              .Select( s => s[random.Next( s.Length )] ).ToArray() );
        }

        #endregion

        #region CVARHOST

        public static CVarHost host = new CVarHost();

        public static void RCVars( ) {
            host.CreateCVar( "console.bufferwidth", (_arg) => {

                if( !int.TryParse( _arg[0], out int arg ) ) {
                    return RetType._C_FAILURE;
                }

                if ( arg < Console.BufferWidth || arg >= short.MaxValue ) {
                    return RetType._C_FAILURE;
                }

                Console.BufferWidth = ( short ) arg;
                return RetType._C_SUCCESS;

            }, Console.BufferWidth.ToString(), "The buffer-width size." );

            host.CreateCVar( "console.title", ( _arg ) => {

                Console.Title = StringArrayToString(_arg);
                return RetType._C_SUCCESS;

            }, Console.Title, "The console title." );

            host.CreateCVar( "cmd.pointer", ( _arg ) => {
                string _new = StringArrayToString( _arg );

                if ( !(_new.Length <= 25) ) {
                    return RetType._C_FAILURE;
                }

                line_pointer = _new;
                return RetType._C_SUCCESS;
            }, line_pointer, "The prompt text.");

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
        }

        #endregion

        #region ExecutionContext

        public static CommandExecutor context = new CommandExecutor();

        #endregion

        #region IsNull

        public static bool IsNull<T>(T var)
        {
            return (var == null);
        }

        #endregion

        #region Compare

        public static bool Compare<T1, T2>(T1 one, T2 two)
        {
            return one.Equals(two);
        }

        #endregion

        #region IndexAt

        public static T IndexAt<T>(ref T[] arr, int index)
        {
            return arr[index];
        }

        #endregion

        #region Version

        public static string __version__ = "1.0.4";

        #endregion

        #region RemoveAt

        public static T[] RemoveAt<T>(T[] source, int index) // Quite expensive
        {
            T[] dest = new T[source.Length - 1];

            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if(index < source.Length - 1)
            {
                Array.Copy(source, (index + 1), dest, index, (source.Length - index - 1));
            }

            return dest;
        }

        #endregion

        #region Instance

        public static Start _main = new Start();

        #endregion

        #region Panic

        public static bool restart_needed = false;

        public static void Panic()
        {
            // panic basically resets the entire running instance, seen above.
            Console.Clear();
            restart_needed = true;
        }

        #endregion

        #region PointerText

        public static string line_pointer = $"{Environment.UserName}@{Environment.MachineName}~$ ";

        #endregion

        #region Directory

        public static string current_directory = System.IO.Directory.GetCurrentDirectory();

        #endregion

        #region ErrorOutput

        public static void Out(RetType status, string additional = "")
        {

            if ( status == RetType._C_DUMMY_VL ) {
                return;
            }

            ErrorTranslator unit = new ErrorTranslator(status);

            string err = unit.Error;
            string time = unit.TranslationTime.ToShortTimeString();

            if (additional == "")
                additional = "No additional information provided.";

            #region out

            Console.WriteLine();
            Console.WriteLine($"{err} at {time} | Additional Info: {additional}");
            Console.WriteLine();

            #endregion
        }

        #endregion

        #region CaseSensitive

        public static bool _case_sensitive = true;

        #endregion
    }
}

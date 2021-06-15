
namespace cmd
{
    class Entry
    {
        static void Main(string[] args)
        {
            G.__on_start();

            Parser arg_worker = new Parser(args);
            bool IsFirstTime = true;

            arg_worker
                .add_optional( "--disable-flush", ref Start.__flush_disabled__, true )
                .add_optional( "--disable-arguements", ref Start.__arguement_cmds_disabled__, true ) // __ is reserved for compiler generated symbols but eh
                .add_optional( "--first-time-init", ref IsFirstTime, true )
                #region SCRIPT

                .add_required_with_input( "--script", ( _args ) => {

                    if ( !System.IO.File.Exists( _args ) ) {
                        return RetType._C_SYSTEM_ERROR;
                    }

                    string[] fdata = System.IO.File.ReadAllLines( _args );

                    if ( fdata.Length == 0 ) {
                        return RetType._C_FAILURE;
                    }

                    foreach(var line in fdata ) {
                        G.context.ExecuteCommand( line );
                    }

                    return RetType._C_SUCCESS;

                } );

                #endregion

            if ( IsFirstTime ) {
                System.IO.Directory.CreateDirectory( "data\\temp" );
                System.IO.Directory.CreateDirectory( "data\\plugins" );
                System.IO.Directory.CreateDirectory( "data\\config" );
            }

        Reset:

            do
            {
                G._main.exec(); // executes once & we start. This function will only ever return when Panic has been called.

            } while (!G.restart_needed);

            #region PANIC_HANDLE

            G._main = new Start(); // reset it entirely.
            G.host._Reset();
            G.restart_needed = false;

            #endregion

            goto Reset;
        }
    }
}

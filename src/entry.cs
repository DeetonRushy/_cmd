using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace cmd
{
    class Entry
    {
        static void Main(string[] args)
        {
            G.StorageWorker.Save( "data\\temp\\saved\\s1.json" );
            Parser arg_worker = new Parser(args);
            bool IsFirstTime = false;

            arg_worker
                .add_optional( "--disable-flush", ref Start.__flush_disabled__, true )
                .add_optional( "--disable-arguements", ref Start.__arguement_cmds_disabled__, true ) // __ is reserved for compiler generated symbols but eh
                .add_optional( "--disable-case-sens", ref G._case_sensitive, false )
                .add_optional( "--first-time-init", ref IsFirstTime, true );


            try {
                G.StorageWorker.Load( "data\\temp\\saved\\s1.json" );
            }
            catch {
                G.L.OG( "Save file not present, attempting to continue." );
            }

            // we check if it's our first time, if it is we want to create all directorys we may use.
            // the launcher will decide if it's a first time init.

            if ( IsFirstTime ) {
                System.IO.Directory.CreateDirectory( "data" ); // The main directory for all used data.
                System.IO.Directory.CreateDirectory( "data\\plugins" ); // The plugin directory.
                System.IO.Directory.CreateDirectory( "data\\temp" ); // used for temporary storage.

                // we write all constants to temp file.

                G.StorageWorker.Write( "__version__", "v1.2.4" );
            }

        Reset:

            do
            {
                G._main.exec(); // executes once & we start. This function will only ever return when Panic has been called.

            } while (!G.restart_needed);

            #region PANIC_HANDLE

            G._main = new Start(); // reset it entirely.
            G.restart_needed = false;

            #endregion


            G.StorageWorker.Save( "data\\temp\\saved\\s1.json" );
            goto Reset;
        }
    }
}

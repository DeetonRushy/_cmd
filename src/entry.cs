using CommandLine;
using System;
using cmd.Tests;

namespace cmd
{
    class Entry
    {
        static Tests.Parser parser;

        static void Main(string[] args)
        {
            G.l.OG( "entry point hit." );
            parser = new Tests.Parser( args );

            parser.AddArgument( 'T', "This is a test", "test", AType.REQUIRED, ( p ) => {
                Console.WriteLine( p );
                return 1;
            } );

            parser.Parse();

            G.__on_start();

            if ( true ) {
                System.IO.Directory.CreateDirectory( "data\\temp" );
                System.IO.Directory.CreateDirectory( "data\\plugins" );
                System.IO.Directory.CreateDirectory( "data\\config" );
                System.IO.Directory.CreateDirectory( "data\\helpers" );
            }

            #region _start 

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

            #endregion
        }
    }
}

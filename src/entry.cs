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
            Parser arg_worker = new Parser(args);
            bool IsFirstTime = false;

            arg_worker
                .add_optional( "--disable-flush", ref Start.__flush_disabled__, true )
                .add_optional( "--disable-arguements", ref Start.__arguement_cmds_disabled__, true ) // __ is reserved for compiler generated symbols but eh
                .add_optional( "--disable-case-sens", ref G._case_sensitive, false )
                .add_optional( "--first-time-init", ref IsFirstTime, true );


        Reset:

            do
            {
                G._main.exec(); // executes once & we start. This function will only ever return when Panic has been called.

            } while (!G.restart_needed);

            #region PANIC_HANDLE

            G._main = new Start(); // reset it entirely.
            G.restart_needed = false;

            #endregion

            goto Reset;
        }
    }
}

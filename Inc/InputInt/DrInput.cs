using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using static System.Console;

namespace cmd {

    class DrInput {

        /// <summary>
        /// Holds the last 64 characters entered into the stdin.
        /// </summary>
        /// 
        // at some point this would be nice for a CTRL+Z shortcut

        public Stack<char> pChars = new Stack<char>( 64 );

        /// <summary>
        /// Most recent text copied from the buffer.
        /// </summary>
        public string Copied { get; set; }

        private bool _cursorVisible = false;

        public bool CursorVisible {
            get {
                return _cursorVisible;
            }

            set {
                _cursorVisible = value;
                Console.CursorVisible = value;
            }
        }

        /// <summary>
        /// Holds all commands that were entered before indexing using ConsoleKey.UpArrow
        /// </summary>
        public Stack<string> pPastCommands = new Stack<string>();

        /// <summary>
        /// Holds all commands that were entered before indexing using ConsoleKey.DownArrow
        /// </summary>
        public Stack<string> pFutureCommands = new Stack<string>();

        /// <summary>
        /// The builder for the current input being read from stdin.
        /// </summary>
        public StringBuilder cCommand = new StringBuilder();

        public string Text => cCommand.ToString();

        public DrInput( ) {
            pChars.Push( '\0' );
            pPastCommands.Push( "" );
            pFutureCommands.Push( "" );
        }

        private void AddFutureCommand( string _Recent ) {
            if ( _Recent is null ) {
                return; // don't care, doesn't really matter.
            }

            pFutureCommands.Push( _Recent );
        }

        private void AddPastCommand( string _Recent ) {
            if ( _Recent is null ) {
                return; // see above
            }

            pPastCommands.Push( _Recent );
        }

        private void ReplaceCommand( string _Command ) {
            cCommand.Clear();
            cCommand.Append( _Command );
        }

        private string TryFetchRecentPastCommand( bool output ) {

            // do not catch without specifiers, 
            // compiler must emit exceptions.

            try {
                var res = pPastCommands.Pop();

                if ( output ) {
                    Write( res );
                }

                return res;
            }
            catch ( InvalidOperationException /* Stack Empty */) { // if the stack is empty, there isn't a recent command.
                return string.Empty;
            }
        }

        private string TryFetchRecentFutureCommand( bool output ) {
            try {
                var res = pFutureCommands.Pop();

                if ( output ) {
                    Write( res );
                }

                return res;
            }
            catch ( InvalidOperationException /* Stack Empty */ ) {
                return string.Empty;
            }
        }

        public string ReadLine( bool output ) {

            cCommand.Clear(); // make sure to flush the buffer.

            ConsoleKeyInfo keyInfo;
            while ( (keyInfo = ReadKey( true )).Key != ConsoleKey.Enter ) {
                if ( keyInfo.Key == ConsoleKey.UpArrow ) {
                    var res = TryFetchRecentFutureCommand( output );
                    ReplaceCommand( res );
                    AddPastCommand( res );
                }

                if ( keyInfo.Key == ConsoleKey.DownArrow ) {
                    var res = TryFetchRecentPastCommand( output );
                    ReplaceCommand( res );
                    AddFutureCommand( res );
                }

                // ^1 will do cCommand.Length - 1 ?

                if ( keyInfo.Key == ConsoleKey.Backspace ) {
                    cCommand.Remove( cCommand.Length - 1, 1 );
                    
                    if( cCommand.Length != 0 ) {
                        cCommand.Length--;
                    }
                }

                // CTRL+V | Paste

                if ( (keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control
                    && keyInfo.Key == ConsoleKey.V ) {
                    cCommand.Append( Copied );
                    continue;
                }

                // CTRL+X | Copy

                if ( (keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control
                    && keyInfo.Key == ConsoleKey.X ) {

                    Clipboard.SetText( Text );
                    continue;
                }

                // CTRL+Q | Quit

                if ( (keyInfo.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control
                    && keyInfo.Key == ConsoleKey.Q ) {
                    Environment.Exit( 0 );
                }

                // INS | Show Cursor

                if ( keyInfo.Key == ConsoleKey.Insert ) {
                    Console.CursorVisible = !CursorVisible;
                }

                if ( keyInfo.Key == ConsoleKey.LeftArrow ) {

                    if ( CursorLeft is 0 ) {
                        continue;
                    }

                    CursorLeft--;
                }

                if ( keyInfo.Key == ConsoleKey.RightArrow ) {

                    if ( CursorLeft == Console.BufferWidth ) {
                        continue;
                    }

                    CursorLeft++;
                }

                if ( output ) {
                    Write( keyInfo.KeyChar );
                }

                pChars.Push( keyInfo.KeyChar );
                cCommand.Append( keyInfo.KeyChar );
            }

            pPastCommands.Push( cCommand.ToString() );
            return cCommand.ToString();
        }
    }

}

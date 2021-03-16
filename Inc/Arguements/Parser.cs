using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    public struct ErrorInformation
    {
        public string _message;
        public string _title;
    }

    public class Parser
    {
        private string[] _args { get; set; }
        private bool exit_on_error = false;
        private IDictionary<string, ErrorInformation> ErrorInfo;

        public Parser(string[] args, bool exit = true)
        {
            ErrorInfo = new Dictionary<string, ErrorInformation>();
            _args = args;
        }

        ~Parser() { }

        /// <summary>
        /// Checks the command-line for a single word, sets a referenced bool to specified value.
        /// </summary>
        /// <param name="arg">The string value to search for.</param>
        /// <param name="decl">The referenced bool, will be set on found.</param>
        /// <param name="toSetTo">The value to set referenced bool to.</param>
        /// <returns><c>this</c> to allow nicer looking code.</returns>
        public Parser add_optional(string arg, ref bool decl, bool toSetTo)
        {
            G.L.OG( $"[parser] parsing optional argument ({arg})" );

            if (_args.Length == 0)
                return this;

            foreach (string q in _args)
            {
                if (q == arg)
                {
                    decl = toSetTo;
                    return this;
                }
            }

            return this;
        }

        /// <param name="err">The exit code.</param>
        /// <returns><c>this</c>, no clue why.</returns>
        private Parser Exit(int err)
        {
            if (ErrorInfo.ContainsKey("true"))
            {
                string text = ErrorInfo["true"]._message;
                string title = ErrorInfo["true"]._title;

                if(String.IsNullOrEmpty(text) || String.IsNullOrEmpty(title))
                {
                    // if either fail, we can't trust what is in them.

                    text = "Missing a required arguement.";
                    title = "Error.";                   
                }

                System.Windows.Forms.MessageBox.Show(text, title);
            }

            if (exit_on_error)
            {
                Environment.Exit(err);
            }

            return this; // never reached if exit on error is on.
        }

        /// <summary>
        /// Sets the error message on failure to find required argument.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="title">The title of the messagebox</param>
        /// <returns><c>this</c>, to allow cleaner looking code.</returns>
        public Parser set_error_on_required(string message, string title = "Error!")
        {
            G.L.OG( $"[parser] setting error on required failure ({title}, {message})" );

            if (ErrorInfo.Count != 0)
            {
                ErrorInfo["true"] = new ErrorInformation() { _message = message, _title = title };
                return this;
            }

            ErrorInfo.Add("true", new ErrorInformation() { _message = message, _title = title });
            return this;
        }

        /// <summary>
        /// Will search the command line for formatted string with this format
        /// <code>
        /// KEY=VALUE
        /// </code>
        /// If found, the passed delegate is executed with the <c>VALUE</c> passed as the parameter.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="fn"></param>
        /// <returns></returns>
        public Parser add_required_with_input( string arg, Func<string, RetType> fn ) {

            if ( _args.Length == 0 ) {
                return this;
            }

            string[] _work = _args;

            for(int i = 0; i < _work.Length; i++ ) {
                if ( !_work[i].Contains(arg) || !_work.Contains("=" ) ) {
                    continue;
                }

                var nw = _work[i].Split( '=' );

                if ( !(nw.Length == 2) ) {
                    continue;
                }

                if ( !(nw[0] == arg) ) {
                    continue;
                }

                G.L.OG( $"[args] found {arg}. executing partner function." );

                fn( nw[1] );
            }

            return this;
        }

        /// <summary>
        /// Checks the command-line for a single word, sets a referenced bool to specified value.
        /// </summary>
        /// <param name="arg">The string value to search for.</param>
        /// <param name="decl">The referenced bool, will be set on found.</param>
        /// <param name="toSetTo">The value to set referenced bool to.</param>
        /// <returns><c>this</c> to allow nicer looking code.</returns>
        public Parser add_required(string arg, ref bool decl, bool toSetTo)
        {
            G.L.OG( $"[parser] parsing required argument ({arg})" );

            foreach (string _arg in _args)
            {
                if(_arg == arg)
                {
                    decl = toSetTo;
                    return this;
                }
            }

            return Exit(-1); // If error isn't set we just return exit.
        }
    }
}

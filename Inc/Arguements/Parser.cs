using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    struct ErrorInformation
    {
        public string _message;
        public string _title;
    }

    class Parser
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

        public Parser add_optional(string arg, ref bool decl, bool toSetTo)
        {
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
                Environment.Exit(0);
            }

            return this; // never reached if exit on error is on.
        }

        public Parser set_error_on_required(string message, string title = "Error!")
        {
            if (ErrorInfo.Count != 0)
            {
                ErrorInfo["true"] = new ErrorInformation() { _message = message, _title = title };
                return this;
            }

            ErrorInfo.Add("true", new ErrorInformation() { _message = message, _title = title });
            return this;
        }

        public Parser add_required_with_answer(string arg, ref string[] decl)
        {
            // we check for the arguement then check the two arguements after that.
            // if arg2 is an = we keep going.
            // arg3 will then be the data they're passing.
            // we search for a comma after the data to allow us to know where it stops.

            string[] _work = _args;

            for(int i = 0; i < _work.Length; i++)
            {
                if (i >= _work.Length || i+2 >= _work.Length) // make sure index isn't out of range.
                    break;

                if(_work[i] == arg && _work[i+1] == "=") // arg1 is the keyword && arg2 is '='
                {
                    for(int q = i; _work[q] == ","; q++) // loop from the current index until _work[q] is a comma
                    {
                        decl.Append(_work[q]); // append the data to our result.
                    }

                    return this; // return the instance for pretty looks :)
                }
            }

            return Exit(-1); // all failed, return null & let the program crash.
        }

        public Parser add_required(string arg, ref bool decl, bool toSetTo)
        {
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

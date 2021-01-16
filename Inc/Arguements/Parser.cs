using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    struct ErrorInformation
    {
        public string _message;
        public string _title;
    }

    class Parser
    {
        private string[] _args { get; set; }

        private IDictionary<string, ErrorInformation> ErrorInfo;

        public Parser(string[] args)
        {
            #region FileOG
            G.L.OG("Parser begun, arguements below.");

            for(int i = 0; i < args.Length; i++)
            {
                G.L.OG($"arg{i} : {args[i]}");
            }
            #endregion

            ErrorInfo = new Dictionary<string, ErrorInformation>();
            _args = args;
        }

        ~Parser() { }

        public Parser add_optional(string arg, ref bool decl, bool toSetTo)
        {
            G.L.OG("Parsing optional arguement : " + arg);

            if (_args.Length == 0)
                return this;

            foreach (string q in _args)
            {
                if(q == arg)
                {
                    decl = toSetTo;
                    return this;
                }
            }

            return this;
        }

        public Parser Exit(int err)
        {
            G.L.OG("Parser.Exit began, required arguement failed.");

            if (ErrorInfo.ContainsKey("true"))
            {
                string text = ErrorInfo["true"]._message;
                string title = ErrorInfo["true"]._title;

                if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(title))
                {
                    // if either fail, we can't trust what is in them.

                    text = "Missing a required arguement.";
                    title = "Error.";
                }

                System.Windows.Forms.MessageBox.Show(text, title);
                return Exit(-1);
            }

            Environment.Exit(err);
            return this;
        }
            return this;
        }

        public Parser Exit(int err)
        {
            G.L.OG("Parser.Exit began, required arguement failed.");

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
                return Exit(-1);
            }

            Environment.Exit(err);
            return this;
        }

        public Parser set_error_on_required(string message, string title = "Error!")
        {
            G.L.OG($"ArgsParser->OnError: message={message}, title={title}");

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
                    G.L.OG("Parsing data for command-line arguement : " + arg);

                    for(int q = i; _work[q] == ","; q++) // loop from the current index until _work[q] is a comma
                    {
                        G.L.OG("Appending data : " + _work[q]);
                        decl.Append(_work[q]); // append the data to our result.
                    }

                    return this; // return the instance for pretty looks :)
                }
            }

            return Exit(-1); // all failed, return null & let the program crash.
        }

        public Parser add_required(string arg, ref bool decl, bool toSetTo)
        {
            G.L.OG("Parsing required arguement : " + arg);

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

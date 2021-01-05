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
                if(q == arg)
                {
                    decl = toSetTo;
                    return this;
                }
            }

            return null;
        }

        public void set_error_on_required(string message, string title = "Error!")
        {
            if (ErrorInfo.Count != 0)
            {
                ErrorInfo["true"] = new ErrorInformation() { _message = message, _title = title };
            }

            ErrorInfo.Add("true", new ErrorInformation() { _message = message, _title = title });
        }

        public Parser add_required(string arg, ref bool decl, bool toSetTo)
        {
            foreach(string _arg in _args)
            {
                if(_arg == arg)
                {
                    decl = toSetTo;
                    return this;
                }
            }

            return null;
        }
    }
}

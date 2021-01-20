using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    class ErrorTranslator
    {
        private IDictionary<RetType, string> TranslationUnit;

        public ErrorTranslator(RetType error)
        {
            TranslationUnit = new Dictionary<RetType, string>();

            #region UnitSetup

            TranslationUnit.Add(RetType._C_ACCESSVIOLATION, "There was an access violation.");
            TranslationUnit.Add(RetType._C_FAILURE, "The operation failed.");
            TranslationUnit.Add(RetType._C_IOERROR, "There was an IO error.");
            TranslationUnit.Add(RetType._C_SUCCESS, "The operation completed sucessfully.");
            TranslationUnit.Add(RetType._C_UNKNOWN_ERROR, "There was an unknown error.");
            TranslationUnit.Add(RetType._C_DISABLED, "Command has been disabled by the command line.");
            TranslationUnit.Add(RetType._C_SYSTEM_ERROR, "There was an error performing a system command. (syscall)");

            #endregion

            Error = TranslationUnit[error];
            Original = error;
            TranslationTime = DateTime.Now;
        }

        ~ErrorTranslator() { }

        public string Error
        {
            get;
            private set;
        }

        public RetType Original
        {
            get;
            private set;
        }

        public DateTime TranslationTime
        {
            get;
            private set;
        }
    }
}

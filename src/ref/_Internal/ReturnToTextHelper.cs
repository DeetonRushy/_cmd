using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class ErrorTranslator
    {
        private IDictionary<_cmdReturn, string> TranslationUnit;

        public ErrorTranslator(_cmdReturn error)
        {
            TranslationUnit = new Dictionary<_cmdReturn, string>();

            #region UnitSetup

            TranslationUnit.Add(_cmdReturn._C_ACCESSVIOLATION, "There was an access violation.");
            TranslationUnit.Add(_cmdReturn._C_FAILURE, "The operation failed.");
            TranslationUnit.Add(_cmdReturn._C_IOERROR, "There was an IO error.");
            TranslationUnit.Add(_cmdReturn._C_SUCCESS, "The operation completed sucessfully.");
            TranslationUnit.Add(_cmdReturn._C_UNKNOWN_ERROR, "There was an unknown error.");

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

        public _cmdReturn Original
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

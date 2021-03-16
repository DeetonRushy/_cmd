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

            TranslationUnit.Add(RetType._C_ACCESSVIOLATION, G.cfg["Mappings"]["ACCESS_VIO"]);
            TranslationUnit.Add(RetType._C_FAILURE, G.cfg["Mappings"]["FAIL"] );
            TranslationUnit.Add(RetType._C_IOERROR, G.cfg["Mappings"]["IOERR"] );
            TranslationUnit.Add(RetType._C_SUCCESS, G.cfg["Mappings"]["SUCCESS"] );
            TranslationUnit.Add(RetType._C_UNKNOWN_ERROR, G.cfg["Mappings"]["UNKNOWN_ERROR"] );
            TranslationUnit.Add(RetType._C_DISABLED, G.cfg["Mappings"]["DISABLED"] );
            TranslationUnit.Add(RetType._C_SYSTEM_ERROR, G.cfg["Mappings"]["SYSTEM_ERROR"] );
            TranslationUnit.Add(RetType._C_RESOURCE_NOT_EXIST, G.cfg["Mappings"]["RES_NOT_EXIST"] );
            TranslationUnit.Add( RetType._C_INVALID_PARAMS, G.cfg["Mappings"]["INVAL_PARAMS"] );
            TranslationUnit.Add( RetType._C_DUMMY_VL, G.cfg["Mappings"]["DUMMY"] );

            #endregion

            Error = TranslationUnit[error];
            Original = error;
            TranslationTime = DateTime.Now;
        }

        ~ErrorTranslator() { }

        /// <summary>
        /// The error converted into a string.
        /// </summary>
        public string Error
        {
            get;
            private set;
        }

        /// <summary>
        /// The original, <c>RetType</c> form.
        /// </summary>
        public RetType Original
        {
            get;
            private set;
        }

        /// <summary>
        /// The time stamp at which the error was translated.
        /// </summary>
        public DateTime TranslationTime
        {
            get;
            private set;
        }
    }
}

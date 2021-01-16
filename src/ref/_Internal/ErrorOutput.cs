using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class ErrorOutputHandler
    {
        public static void Out(_cmdReturn status, string additional = "")
        {
            G.L.OG("Outputing information on last error & additional information.");

            string err = new ErrorTranslator(status).Error;
            string time = DateTime.Now.ToShortTimeString();

            if (additional == "")
                additional = "No additional information provided.";

            #region out

            Console.WriteLine();
            Console.WriteLine($"{err} at {time} | Additional Info: {additional}");
            Console.WriteLine();

            #endregion
        }
    }
}

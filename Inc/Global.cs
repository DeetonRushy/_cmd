using System;

namespace cmd
{
    class G
    {
        #region LoggerInstance

        // public static GOG L = new GOG("data\\logs");

        #endregion

        #region IsNull

        public static bool IsNull<T>(T var)
        {
            return (var == null);
        }

        #endregion

        #region Compare

        public static bool Compare<T1, T2>(T1 one, T2 two)
        {
            return one.Equals(two);
        }

        #endregion

        #region IndexAt

        public static T IndexAt<T>(ref T[] arr, int index)
        {
            return arr[index];
        }

        #endregion

        #region Version

        public static string __version__ = "v1.0.1";

        #endregion

        #region RemoveAt

        public static T[] RemoveAt<T>(T[] source, int index) // Quite expensive
        {
            T[] dest = new T[source.Length - 1];

            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if(index < source.Length - 1)
            {
                Array.Copy(source, (index + 1), dest, index, (source.Length - index - 1));
            }

            return dest;
        }

        #endregion

        #region Instance

        public static Start _main = new Start();

        #endregion

        #region Panic

        public static bool restart_needed = false;

        public static void Panic()
        {
            // panic basically resets the entire running instance, seen above.
            Console.Clear();
            restart_needed = true;
        }

        #endregion

        #region PointerText

        public static string line_pointer = $"{Environment.UserName}@{Environment.MachineName}~$ ";

        #endregion

        #region Directory

        public static string current_directory = System.IO.Directory.GetCurrentDirectory();

        #endregion

        #region ErrorOutput

        public static void Out(RetType status, string additional = "")
        {
            ErrorTranslator unit = new ErrorTranslator(status);

            string err = unit.Error;
            string time = unit.TranslationTime.ToShortTimeString();

            if (additional == "")
                additional = "No additional information provided.";

            #region out

            Console.WriteLine();
            Console.WriteLine($"{err} at {time} | Additional Info: {additional}");
            Console.WriteLine();

            #endregion
        }

        #endregion

        #region CaseSensitive

        public static bool _case_sensitive = true;

        #endregion
    }
}

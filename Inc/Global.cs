
namespace cmd
{
    class G
    {
        #region LoggerInstance

        public static GOG L = new GOG("data\\logs");

        #endregion

        #region IsNull

        public static bool IsNull<T>(T var)
        {
            return (var == null);
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
    }
}

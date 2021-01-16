using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// namespace for G L object.

namespace _cmd
{
    class G
    {
        public static cmd_NonGit.L.GOG L = new cmd_NonGit.L.GOG();
    }
}

namespace cmd_NonGit.L
{
    class GOG
    {
        private string I_SessionId = string.Empty;
        private string I_FilePath = string.Empty;
        private bool I_Initialized = false;

        private class GOGIndex
        {
            public GOGIndex(string name, string ident, string Time, string path, string init)
            {
                Name = name;
                SessionIdentifier = ident;
                this.Time = Time;
                FilePath = path;
                WasInitialized = init;
            }

            string Name;
            string SessionIdentifier;
            string Time;
            string FilePath;
            string WasInitialized;
        }

        public GOG()
        {
            I_SessionId = new Random().Next(0, int.MaxValue).ToString();

            if (!Directory.Exists("OGging"))
            {
                Directory.CreateDirectory("OGs");
            }

            Directory.CreateDirectory("OGs\\.data");
            I_FilePath = "OGs";

            Directory.CreateDirectory($"OGs\\{I_SessionId}");
            FileStream iDis = File.Create($"OGs\\{I_SessionId}\\Current.log");

            I_FilePath = $"OGs\\{I_SessionId}\\Current.OG";
            iDis.Dispose();

            GOGIndex ind = new GOGIndex(I_SessionId, new Random().Next().ToString(), DateTime.Now.ToLongTimeString(), I_FilePath, I_Initialized.ToString());
            string SerializedStringOuter = JsonConvert.SerializeObject(ind);


            if (!File.Exists("OGs\\.data\\index.json"))
            {
                FileStream fs = File.Create("OGs\\.data\\index.json");
                fs.Dispose();
            }

            I_Initialized = true;
        }

        public GOG(string FilePath, int SessionIdLength)
        {
            /* Auto Generate A Session Id Here */

            I_FilePath = FilePath;
            I_SessionId = $"{new Random().Next(0, int.MaxValue).ToString()}-{Environment.UserName}";

            if (!Directory.Exists(I_FilePath))
            {
                Directory.CreateDirectory(I_FilePath);
                Directory.CreateDirectory(I_FilePath + "\\.data");
            }

            Directory.CreateDirectory(I_FilePath + "\\" + I_SessionId);
            FileStream Dispose = File.Create(I_FilePath + "\\Current.OG");
            I_FilePath = $"{I_FilePath}\\{I_SessionId}\\Current.OG";

            Dispose.Dispose();
        }

        public bool OG(params string[] Information)
        {
            if (!I_Initialized)
            {
                Console.WriteLine("[GOG] -> We Didn't Initialize Properly In The Constructor, Sorry!");
                System.Threading.Thread.Sleep(4000);
            }

            foreach (string GOG in Information)
            {
                try
                {
                    File.AppendAllText(I_FilePath, $"[GOG][{DateTime.Now.ToLongDateString()}][{DateTime.Now.ToLongTimeString()}] -> {GOG}\n");
                }
                catch (Exception FileError)
                {
#if DEBUG
                    Console.WriteLine("Internal GOG error : " + FileError.Message);
#endif
                    return false;
                }
            }

            return true;
        }
    }
}


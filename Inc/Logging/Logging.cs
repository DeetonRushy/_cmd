using System;
using System.IO;
using System.Linq;

namespace cmd
{
    public class GOG
    {

        string root = string.Empty; // root file path. I.E The folder when the logs will be created.
        bool success = false; // construction success
        string session = string.Empty; // session folder

        public GOG(string root_dir)
        {
            // set success at beginning to make it easier.

            success = true;
            root = root_dir;

            // create all directorys & initial files.
            // also set all class local variables
            // to the chosen path.

            if (!Directory.Exists(root_dir))
            {
                Directory.CreateDirectory(root_dir);
            }

        // generate session identifier

        RestartCreation:

            Random rand = new Random(); // for random selection
            const string alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string identifier = new string(Enumerable.Repeat(alpha, 4) // generate random 4 char string /
                .Select(s => s[rand.Next(s.Length)]).ToArray());            // from the alpha string we have /

            if (!Directory.Exists(root + "\\" + identifier))
            {
                Directory.CreateDirectory(root + "\\" + identifier);
            }
            else
            {
                goto RestartCreation;
            }

            session = root + "\\" + identifier + "\\current.log";

            #region FileCreation

            try
            {
                FileStream log_stream = File.Create(session);
                log_stream.Dispose(); // automatically closes the stream.
            }
            catch
            {
                success = false;
            }

            #endregion

        }

        public bool OG(params string[] Information)
        {
            if (!success) // failed to init
            {
                return success; // cancel log.
            }

            if (File.Exists(session)) // make sure file exists
            {
                foreach (string log in Information) // iterate params
                {
                    string fmt = string.Format( "{0}", DateTime.Now.ToShortDateString() );
                    File.WriteAllText( this.session, fmt + " " + log + "\n" );
                }
            }
            else
            {
                success = false; // avoid coming here again.
                return false;
            }

            return true;
        }
    }
}


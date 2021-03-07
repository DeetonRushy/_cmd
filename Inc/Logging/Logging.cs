using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace cmd
{
    public class GOG
    {

        string root = string.Empty; // root file path. I.E The folder when the logs will be created.
        bool success = false; // construction success
        string session = string.Empty; // session folder


        // PLAN: We launch a seperate thread that will handle actually writing the data to the file.
        // We do this because the external calls to AppendAllText takes up nearly 30% of our entire
        // program CPU usage. This is not acceptable for something that IS NOT essensial.

        public string[] work = { }; // storage for our log worker thread to access logs & delete them.
        Thread worker_thread = null; // our thread, to keep track of it.

        public GOG(string root_dir)
        {
            // set success at beginning to make it easier.

            work.Append("LoggingWorkerThread - init");
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
                .Select(s => s[rand.Next(s.Length)]).ToArray());       // from the alpha string we have /

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

            #region ThreadCreation

            Thread log_worker = new Thread((obj) =>
            {
                // We simply want to access to work variable, see what is currently in it,
                // then we grab the data, log it & delete it from the programs memory space.

                // We quickly copy the logs into the thread,
                // so we aren't accessing the array while 
                // logs are being written to it.

                GOG _instance = (GOG)obj;
                string[] _job = _instance.work;
                

                for(int i = 0; i < _job.Length; i++)
                {
                    for(int q = 0; q < 5; q++) // We retry 5 times if it fails.
                    {
                        try
                        {
                            string fmt = string.Format("[{0}] {1} {2}", DateTime.Now.ToShortTimeString(), _job[i], "\n");
                            File.AppendAllText(session, fmt); // log the data.
                        }
                        catch
                        {
                            continue; // Keep going for specified amount of retries.
                        }

                        break; // Succeeded, we break and continue logging others.
                    }
                }

                // We assume we successfully logged all of the information.
                // We now clear our work load.

                Array.Resize(ref work, 0);

                // We now sleep, this helps the logs accumulate & gives this thread
                // More work to do once it wakes up.

                Thread.Sleep(300);

            });

            worker_thread = log_worker;

            log_worker.IsBackground = true;
            log_worker.Name = "LoggingThread";
            log_worker.Start(this); // pass this into the thread so it can get the logs.

            #endregion
        }

        public bool OG(params string[] Information)
        {
            if (!success) // failed to init
            {
                return success; // cancel log.
            }

            if(work.Length >= 128)
            {
                WaitForThread(worker_thread);
            }

            if (File.Exists(session)) // make sure file exists
            {
                foreach (string log in Information) // iterate params
                {
                    File.WriteAllText( this.session, log + "\n" );
                }
            }
            else
            {
                success = false; // avoid coming here again.
                return false;
            }

            return true;
        }

        private void WaitForThread(Thread ToWaitFor)
        {
            while(ToWaitFor.ThreadState != ThreadState.Background)
            {
                Thread.Sleep(10); // wait for 10ms and check again. 
            }

            // this is basically a spinlock, it spins until the thread comes to life.
        }
    }
}


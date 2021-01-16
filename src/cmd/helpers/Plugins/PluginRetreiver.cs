using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    class PluginRetreiver
    {
        public PluginRetreiver(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            if (!System.IO.Directory.Exists("data\\plugins\\exec"))
            {
                System.IO.Directory.CreateDirectory("data\\plugins\\exec");
            }

            _direc = directory;
        }

        ~PluginRetreiver() { }

        private string _direc;

        public string[] get()
        {
            string[] _plgs = System.IO.Directory.GetFiles(_direc, "*.*", System.IO.SearchOption.AllDirectories);

            G.L.OG("Amount of plugins found : " + _plgs.Length);

            #region FileWork

            foreach(string plugin in _plgs)
            {
                G.L.OG("Discovered plugin : " + plugin);
            }

            #endregion

            return _plgs;
        }
    }
}

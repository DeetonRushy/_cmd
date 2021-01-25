using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    // super messy class, no comments until it's cleaned up.

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

            return _plgs;
        }
    }
}

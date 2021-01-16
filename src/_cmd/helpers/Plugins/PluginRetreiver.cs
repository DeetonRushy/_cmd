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

            _direc = directory;
        }

        ~PluginRetreiver() { }

        private string _direc;
        
        // VERY unsafe right now, will be sorted vsoon

        public string[] get()
        {
            string[] _plgs = System.IO.Directory.GetFiles(_direc, "*.*", System.IO.SearchOption.AllDirectories);

            return _plgs;
        }
    }
}

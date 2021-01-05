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

            foreach(string _dir in System.IO.Directory.GetFiles(directory, "*.*", System.IO.SearchOption.AllDirectories))
            {
                files.Append(_dir);
            }
        }

        ~PluginRetreiver() { }

        private string[] files = { };

        public string[] get()
        {
            return files;
        }
    }
}

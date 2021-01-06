using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _cmd
{
    interface IPlugin
    {
        string Name { get; }
        string Explaination { get; }
        void _Go();
    }
}

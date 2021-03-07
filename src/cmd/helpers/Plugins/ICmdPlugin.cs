using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd {

    public interface ICmdPlugin {

        void OnPluginLoad( CommandExecutor ctx );

        void OnPluginUnload(  /* __cdecl */ CommandExecutor ctx );

        string AuthorName { get; set; }

        string PluginName { get; set; }

    }
}

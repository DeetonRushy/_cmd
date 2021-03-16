using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd {

    public interface ICmdPlugin {

        /// <summary>
        /// The function to be called immediately after the plugin has been loaded.
        /// </summary>
        /// <param name="ctx"> The <c>CommandExecutor</c> context. </param>
        /// <param name="cvctx"> The <c>CvarHost</c> context.</param>
        void OnPluginLoad( CommandExecutor ctx, CVarHost cvctx );

        /// <summary>
        /// The function to be called immediately before the plugin is loaded.
        /// </summary>
        /// <param name="ctx"> The <c>CommandExecutor</c> context.</param>
        void OnPluginUnload(  /* __cdecl */ CommandExecutor ctx );

        /// <summary>
        /// Name of the plugin author.
        /// </summary>
        string AuthorName { get; set; }

        /// <summary>
        /// The name of the plugin.
        /// </summary>
        string PluginName { get; set; }

    }
}

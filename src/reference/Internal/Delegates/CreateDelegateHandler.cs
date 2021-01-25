using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd
{
    class DelegateHandler
    {
        private delegate RetType Del(string[] param);
        private string[] _method_params;
        Func<string[], RetType> _method;

        public DelegateHandler(Func<string[], RetType> method, ref string[] _params)
        {
            _method_params = _params;

            if (G.IsNull(method))
                throw new ArgumentNullException("method");

            _method = method;
        }

        public void Execute(out RetType result)
        {
            result = _method.Invoke(_method_params);
        }
    }
}

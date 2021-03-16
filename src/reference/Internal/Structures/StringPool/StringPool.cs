using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd {
    class StringPool<T> : Dictionary<T, string> {

        public StringPool() {

        }

        public string Invalid;

        public string this[T key] {

            get {
                if ( G.IsNull(key) ) {
                    return Invalid;
                }

                return base[key];
            }

            set {
                if ( G.IsNull(key) ) {
                    Invalid = value;
                }
                else {
                    base[key] = value;
                }
            }
        }
    }
}

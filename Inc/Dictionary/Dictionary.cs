using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd_NonGit
{
    class _Dictionary<TKey, TValue>
    {
        TKey[] _keys;
        TValue[] _values;

        public _Dictionary(int size)
        {
            _keys   = new TKey[size];
            _values = new TValue[size];

            for(int i = 0; i < size; i++)
            {
                _keys[i]    =  default;
                _values[i]  =  default;
            }

            OnUpdate();
        }

        private void ShiftArrayDown<T>(ref T[] Arr)
        {
            Array.Sort(Arr);
        }

        private void OnUpdate()
        {
            // called in every function to check the integrity of the Dictionary.
            // If the dictionary is unstable ( for whatever reason ) we attempt to fix it.

            if(_keys.Length > _values.Length)
            {
                for(int i = 0; i == _keys.Length; i++)
                {
                    // they have a fixed size, so we don't worry about
                    // the index not existing.

                    if (!EqualityComparer<TKey>.Default.Equals(_keys[i], default(TKey)))
                    {
                        _keys[i] = default;
                        ShiftArrayDown(ref _keys);
                    }
                }
            }
        }
    }
}

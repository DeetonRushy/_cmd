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

        private bool CompareT<T, T2>( T f, T2 s ) {
            var a = ( object ) f;
            return a.Equals( s );
        }

        private void ShiftArrayDown<T>(ref T[] Arr)
        {
            Array.Sort(Arr);
        }

        private TValue Find( TKey v ) {
            for(int i = 0; i < _keys.Length; i++ ) {
                if( CompareT( _keys[i], v ) ) {
                    return _values[i];
                }
            }

            throw new KeyNotFoundException( $"the key \"{v}\" does not exist." );
        }

        private void Set( TKey key, TValue value ) {
            var index = FindIndex( key );
            _values[index] = value;
            OnUpdate();
        }

        private int FindIndex( TKey v ) {
            for ( int i = 0; i < _keys.Length; i++ ) {
                if ( CompareT( _keys[i], v ) ) {
                    return i;
                }
            }

            throw new KeyNotFoundException( $"the key \"{v}\" does not exist." );
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

        public TValue this[TKey k] {
            get => Find( k );
            set => Set( k, value );
        }

        public void Add( TKey k, TValue v ) => Set( k, v );

        public TValue At( TKey k ) => this[k];

        public static void RunTest( ) {
            var dict = new _Dictionary<string, int>(12);

            dict.Add( "Age", 18 );

            Console.WriteLine( $"_DICTIONARY: Value For 'Age' is {dict.At( "age" )}." );
        }
    }
}

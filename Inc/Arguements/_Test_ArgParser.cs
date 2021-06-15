using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd.Tests {

    public enum AType : int {
        DEFAULT = 0,
        REQUIRED,
        OPTIONAL,
    }

    public class ParsableArgument {
        public ParsableArgument( char dash_flag,
            string dd_help,
            string word_flag,
            AType flag_type,
            Func<string, int> _OnFlagFound) {

            if ( 
                dd_help == null ||
                word_flag == null ) {
                throw new ArgumentException( "One of these three was null when creating a ParsableArgument. [dunder_help, word_flag, OnFlagFound]" );
            }

            Flag = dash_flag;
            Help = dd_help;
            WordFlag = word_flag;
            Descriptor = flag_type;
            OnFlagFound = _OnFlagFound;
            HasChecked = false;
        }

        public char Flag { get; set; }

        public string Help { get; set; }

        public string WordFlag { get; set; }

        public Func<string, int> OnFlagFound { get; set; }

        public AType Descriptor { get; set; }

        public bool HasChecked { get; set; }
    }

    public class Parser {

        private List<ParsableArgument> m_KnownArguments;
        private string[] m_ProgramArguments;

        public Parser( string[] args ) {
            m_KnownArguments = new List<ParsableArgument>();
            m_ProgramArguments = args;
        }

        public void AddArgument( char CharFlag,
            string HelpText,
            string WordFlag,
            AType ArgumentDescriptor,
            Func<string, int> OnFlagFound) {

            var NewArg = new ParsableArgument( CharFlag,
                HelpText,
                WordFlag,
                ArgumentDescriptor,
                OnFlagFound );

            this.m_KnownArguments.Add( NewArg );

        }

        public void AddArgument( ParsableArgument argument ) {
            if(argument != null ) {
                this.m_KnownArguments.Add( argument );
            }
            else {
                throw new ArgumentException( "argument cannot be null." );
            }
        }

        public void Info(string it ) {
            // Console.WriteLine( it );
        }

        public void Parse( ) {
            for(int i = 0; i < m_ProgramArguments.Length; i++ ) {
                Info( $"Currently parsing {m_ProgramArguments[i]}." );
                for(int q = 0; q < m_KnownArguments.Count; q++ ) {
                    Info( $"Currently parsing {m_KnownArguments[q].WordFlag}" );
                    if(m_ProgramArguments[i].ToLower() == "-" + m_KnownArguments[q].Flag || 
                        m_ProgramArguments[i].ToLower() == "-" + m_KnownArguments[q].WordFlag ) {
                        i++;

                        if(m_ProgramArguments[i] == "--help" ) {
                            Console.WriteLine( $"-- HELP FOR {m_KnownArguments[q].WordFlag} --" );
                            Console.WriteLine( m_KnownArguments[q].Help );
                        }

                        m_KnownArguments[q].OnFlagFound( m_ProgramArguments[i] );
                        m_KnownArguments[q].HasChecked = true;
                    }
                }
            }

            foreach(var arg in m_KnownArguments ) {
                Info( $"{arg.HasChecked}, {arg.Descriptor}" );
                if(arg.HasChecked != true && arg.Descriptor == AType.REQUIRED ) {
                    Console.WriteLine( $"Required argument was not supplied. ({arg.WordFlag})" );
                }
            }
        }
    }
}

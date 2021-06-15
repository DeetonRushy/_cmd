using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd {
    class CommunicationCallback {

        public CommunicationCallback( object sender, string message ) {

            if ( sender == null || message == null ) {
                throw new ArgumentNullException( "neither parameter can be null.", "sender | message" );
            }

            _sender = sender;
            _message = message;
        }

        private object _sender;

        public object Sender {
            get {
                return _sender;
            }
        }

        private string _message;
        public string Message {
            get {
                return _message;
            }
        }

    }
}

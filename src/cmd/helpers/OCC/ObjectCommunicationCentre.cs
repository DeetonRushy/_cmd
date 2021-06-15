using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cmd {
    class ObjectCommunicationCentre {

        public IDictionary<string, CommunicationObject> Centre;

        public ObjectCommunicationCentre( ) {
            Centre = new Dictionary<string, CommunicationObject>();
        }

        public CommunicationCallback SendMessage( string receiver, object caller, string msg ) {

            if ( !Centre.ContainsKey(receiver) ) {
                throw new ArgumentException( "Receiver does not exist.", "receiver" );
            }

            (object _caller, string response) = Centre[receiver].OnMessageReceived( caller, msg );

            return new CommunicationCallback( _caller, response ); 
        }
    }
}

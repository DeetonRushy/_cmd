using System;

namespace cmd {
    class CommunicationObject {

        public string LastMessage { get; private set; }

        public CommunicationObject( Func<object, string, (object, string)> _OnMessageReceived ) {
            OnMessageReceived = ( obj, msg ) => {
                return _OnMessageReceived( obj, msg );
            };
        }

        public Func<object, string, (object, string)> OnMessageReceived;
    }
}

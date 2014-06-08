using System;
using GeoClients.GeoServiceReference;

namespace GeoPlugIn
{
    class GeoServiceCallbackHandler :
        IGeoServiceCallback
    {
        public GeoServiceCallbackHandler()
        {

        }

        public void ConsumeResult(GeoTicketOut composite)
        {

        }

        public IAsyncResult BeginConsumeResult(GeoTicketOut composite, AsyncCallback callback, object asyncState)
        {
            return null;
        }

        public void EndConsumeResult(IAsyncResult result)
        {

        }
    }
}

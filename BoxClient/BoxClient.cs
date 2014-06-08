using System.ServiceModel;
using BoxClient.BoxServiceReference;

namespace BoxClient
{
    public class BoxClient<TCallbackHandler>
        where TCallbackHandler : IBoxServiceCallback, new()
    {
        InstanceContext _context;
        TCallbackHandler _callbackHandler;

        public DuplexChannelFactory<IBoxServiceChannel> GetFactory()
        {
            _callbackHandler = new TCallbackHandler();
            _context = new InstanceContext(_callbackHandler);
            return new DuplexChannelFactory<IBoxServiceChannel>(_context, "NetPipeEndPoint");
        }
    }
}

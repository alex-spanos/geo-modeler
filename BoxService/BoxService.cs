using System.ServiceModel;
using BoxController;

namespace BoxService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                     ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BoxService :
        IBoxService
    {
        public void Calculate(GeoTicketIn ticketIn)
        {
            Controller.I.EnQueueJob(this, ticketIn);
        }

        public IBoxServiceCallBack Callback
        {
            get { return OperationContext.Current.GetCallbackChannel<IBoxServiceCallBack>(); }
        }
    }
}

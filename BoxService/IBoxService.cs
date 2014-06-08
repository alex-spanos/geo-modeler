using System.Runtime.Serialization;
using System.ServiceModel;

namespace BoxService
{
    [ServiceContract(Namespace = "http://Geo.Service.Model",
        SessionMode = SessionMode.Required,
        CallbackContract = typeof(IBoxServiceCallBack))]
    public interface IBoxService
    {
        [OperationContract(IsOneWay = true)]
        void Calculate(GeoTicketIn ticketIn);
    }

    [DataContract]
    public class GeoTicketIn
    {
        int ticketId, flowId;
        string memFileName;

        [DataMember]
        public int TicketId
        {
            get { return ticketId; }
            set { ticketId = value; }
        }

        [DataMember]
        public int FlowId
        {
            get { return flowId; }
            set { flowId = value; }
        }

        [DataMember]
        public string MemFileName
        {
            get { return memFileName; }
            set { memFileName = value; }
        }
    }
}

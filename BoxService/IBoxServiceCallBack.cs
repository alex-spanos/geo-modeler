using System.Runtime.Serialization;
using System.ServiceModel;

namespace BoxService
{
    [ServiceContract]
    public interface IBoxServiceCallBack
    {
        [OperationContract(IsOneWay = true)]
        void ConsumeResult(GeoTicketOut composite);
    }

    [DataContract]
    public class GeoTicketOut
    {
        int ticketId;
        string memFileName;

        [DataMember]
        public int TicketId
        {
            get { return ticketId; }
            set { ticketId = value; }
        }

        [DataMember]
        public string MemFileName
        {
            get { return memFileName; }
            set { memFileName = value; }
        }
    }
}

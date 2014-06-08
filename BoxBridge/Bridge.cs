namespace BoxBridge
{
    public class BoxTicketIn
    {
        public int JobId, FlowId;
        public string MemFileName;
    }

    public class BoxTicketOut
    {
        public int JobId;
        public string MemFileName;
    }

    public interface IBoxHandler
    {
        void CalculationComplete(BoxTicketOut ticketOut);
    }
}

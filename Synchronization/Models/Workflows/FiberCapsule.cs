using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataStructures.Graphs;
using Synchronization.Core;

namespace Synchronization.Models.Workflows
{
    public class FiberCapsule<TUnit, TTicketIn, TTicketOut> :
        Capsule<IFeedable<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>>, WorkflowEdge>,
        IWakeable
        where TUnit : class, IWorker<TTicketIn, TTicketOut>, new()
        where TTicketIn : class
        where TTicketOut : class
    {
        #region Initialization

        public void Init(int inputPortsNum, int outputPortsNum,
            IFeedable<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>> feedable)
        {
            Data = feedable;
            InternalInit(inputPortsNum, outputPortsNum);
        }

        public void Init(int inputPortsNum, int outputPortsNum, int numberOfWorkers)
        {
            if (numberOfWorkers <= 0) return;

            if (numberOfWorkers == 1)
                Data = new FlowFiber<TUnit, FlowFiberParameters<TUnit, TTicketIn, TTicketOut>, TTicketIn, TTicketOut>
                                    (new TsQueue<Tuple<JobTicket<TTicketIn>, FiberCapsule<TUnit, TTicketIn, TTicketOut>>>());
            else
                Data = new FlowFiberArray<
                           FlowFiber<TUnit, FlowFiberParameters<TUnit, TTicketIn, TTicketOut>, TTicketIn, TTicketOut>,
                                     TUnit, FlowFiberParameters<TUnit, TTicketIn, TTicketOut>, TTicketIn, TTicketOut>(numberOfWorkers);

            InternalInit(inputPortsNum, outputPortsNum);
        }

        void InternalInit(int inputPortsNum, int outputPortsNum)
        {
            int i;

            if (inputPortsNum > 0)
                for (i = 0; i < inputPortsNum; i++)
                    AddLastPort(ref InPorts, new WorkflowEdge(typeof(TTicketOut), this));

            if (outputPortsNum > 0)
                for (i = 0; i < outputPortsNum; i++)
                    AddLastPort(ref OutPorts);
        }

        #endregion // End of Initialization

        public void WakeUp()
        {
            
        }

        public void Forward(JobTicket<TTicketOut> ticketOut)
        {
            
        }
    }

    public class WorkflowEdge
    {
        public dynamic Queue;

        public WorkflowEdge(Type tTicket, IWakeable capsule)
        {
            Queue = Rhm.CreateTsQueueInstanse(Rhm.CreateTsQueueType(Rhm.CreateJobTicketType(tTicket)), capsule);
        }
    }

    public class JobTicketQueuesFactory
    {
        private readonly Dictionary<string, Type> _queuesTypes = new Dictionary<string, Type>();

        public Type GetQueue(string ticketTypeName)
        {
            if (_queuesTypes.ContainsKey(ticketTypeName))
                return _queuesTypes[ticketTypeName];

            string[] ticketTypeNames = ticketTypeName.Split('|');

            return _queuesTypes[ticketTypeName];
        }
    }

    public class ModuleAnalyzer
    {
        private readonly List<ModuleInfo> _moduleInfos = new List<ModuleInfo>();

        public void LoadModules(string path)
        {
            AnalyzeModules(Assembly.LoadFrom(path));
        }

        public void AnalyzeModules(Assembly assembly)
        {
            _moduleInfos.AddRange(from tUnit in assembly.GetTypes()
                                  let tTickets = tUnit.AnalyseIsWorker()
                                  where tTickets.Any()
                                  select new ModuleInfo {UnitType = tUnit, TicketTypes = tTickets});
        }

        private class ModuleInfo
        {
            public Type UnitType { get; set; }

            public List<Tuple<FieldInfo[], FieldInfo[]>> TicketTypes { get; set; }
        }
    }
}

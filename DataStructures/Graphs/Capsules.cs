using System.Collections.Generic;
using DataStructures.Rings;
using DataStructures.Rings.Multidimentional.D2;

namespace DataStructures.Graphs
{
    /// <summary>
    /// This class encapsulates a set of subgraphs (connected or not) of some job-tree in the sense
    /// that it contains the modules and lower level capsules (nodes) and their interdependecies (edges) and
    /// also groups together the remaining free ports.
    /// </summary>
    /// <typeparam name="TContent">The capsule's content type.</typeparam>
    /// <typeparam name="TPort">The port's data type.</typeparam>
    /// <typeparam name="TSubContent"> </typeparam>
    /// <typeparam name="TSubPort"> </typeparam>
    public class CapsuleTop<TContent, TPort, TSubContent, TSubPort> :
        Capsule<TContent, TPort>
        where TContent : class
        where TPort : class
        where TSubContent : class
        where TSubPort : class
    {
        #region Fields

        /// <summary>
        /// The ring of subcapsules.
        /// </summary>
        public ExtendedDataRing<Capsule<TContent, TPort>, HeadCapsule<TContent, TPort>, TContent, Capsule<TContent, TPort>> SubCapsRing;

        #endregion // End of Fields
    }

    /// <summary>
    /// This class encapsulates a set of subgraphs (connected or not) of some job-tree in the sense
    /// that it contains the modules and lower level capsules (nodes) and their interdependecies (edges) and
    /// also groups together the remaining free ports.
    /// </summary>
    /// <typeparam name="TContent">The capsule's content type.</typeparam>
    /// <typeparam name="TPort">The port's data type.</typeparam>
    /// <typeparam name="TSouperContent"> </typeparam>
    /// <typeparam name="TSouperPort"> </typeparam>
    /// <typeparam name="TSubContent"> </typeparam>
    /// <typeparam name="TSubPort"> </typeparam>
    public class CapsuleMiddle<TContent, TPort, TSouperContent, TSouperPort, TSubContent, TSubPort> :
        ExtDataNode<Capsule<TContent, TPort>, TContent, Capsule<TContent, TPort>>
        where TContent : class
        where TPort : class
        where TSouperContent : class
        where TSouperPort : class
        where TSubContent : class
        where TSubPort : class
    {
        #region Fields

        /// <summary>
        /// The ring of subcapsules.
        /// </summary>
        public ExtendedDataRing<Capsule<TContent, TPort>, HeadCapsule<TContent, TPort>, TContent, Capsule<TContent, TPort>> SubCapsRing;

        #endregion // End of Fields
    }

    /// <summary>
    /// This class encapsulates a set of subgraphs (connected or not) of some job-tree in the sense
    /// that it contains the modules and lower level capsules (nodes) and their interdependecies (edges) and
    /// also groups together the remaining free ports.
    /// </summary>
    /// <typeparam name="TContent">The capsule's content type.</typeparam>
    /// <typeparam name="TPort">The port's data type.</typeparam>
    /// <typeparam name="TBack"> </typeparam>
    public class CapsuleBottom<TContent, TPort, TBack> :
        ExtDataNode<CapsuleBottom<TContent, TPort, TBack>, TContent, TBack>
        where TContent : class
        where TPort : class
        where TBack : class
    {
        
    }

    /// <summary>
    /// The head capsule in the rings of capsules.
    /// </summary>
    /// <typeparam name="TContent">The capsule's content type.</typeparam>
    /// <typeparam name="TPort">The port's data type.</typeparam>
    public class HeadCapsule<TContent, TPort> :
        Capsule<TContent, TPort>,
        IExtNodeHead<Capsule<TContent, TPort>>
        where TContent : class
        where TPort : class
    {
        /// <summary>
        /// The container object.
        /// </summary>
        public Capsule<TContent, TPort> BackLink { get; set; }

        #region Initialization

        public HeadCapsule() { }

        public HeadCapsule(Capsule<TContent, TPort> back)
        {
            BackLink = back;
        }

        public HeadCapsule(Capsule<TContent, TPort> back, TContent data)
            : base(data)
        {
            BackLink = back;
        }

        public HeadCapsule(Capsule<TContent, TPort> back, Capsule<TContent, TPort> node)
            : this(back, node.Data)
        {
            Next = node.Next;
            Prev = node.Prev;
        }

        #endregion // End of Initialization

        /// <summary>
        /// Specifies wheather the node is the head of the ring.
        /// </summary>
        /// <returns>True for head elements and false for all others.</returns>
        protected sealed override bool IsHead() { return true; }
    }

    /// <summary>
    /// This class encapsulates a set of subgraphs (connected or not) of some job-tree in the sense
    /// that it contains the modules and lower level capsules (nodes) and their interdependecies (edges) and
    /// also groups together the remaining free ports.
    /// </summary>
    /// <typeparam name="TContent">The capsule's content type.</typeparam>
    /// <typeparam name="TPort">The port's data type.</typeparam>
    public class Capsule<TContent, TPort> :
        ExtDataNode<Capsule<TContent, TPort>, TContent, Capsule<TContent, TPort>>
        where TContent : class
        where TPort : class
    {
        #region Fields

        /// <summary>
        /// The ring of subcapsules.
        /// </summary>
        public ExtendedDataRing<Capsule<TContent, TPort>, HeadCapsule<TContent, TPort>, TContent, Capsule<TContent, TPort>> SubCapsRing;

        /// <summary>
        /// The input ring of free ports of the capsule.
        /// </summary>
        public LinkRingX<TContent, TPort> InPorts;

        /// <summary>
        /// The output ring of free ports of the capsule.
        /// </summary>
        public LinkRingX<TContent, TPort> OutPorts;

        #endregion // End of Fields

        #region Initialization

        public Capsule() {}

        protected Capsule(TContent content)
        {
            Data = content;
        }

        #endregion // End of Initialization

        #region METHODS

        #region ADD METHODS

        /// <summary>
        /// Add an empty subcapsule at the final position in the ring of subcapsules.
        /// </summary>
        public void AddLastSubcapsule()
        {
            if (SubCapsRing == null)
                SubCapsRing = new ExtendedDataRing<Capsule<TContent, TPort>, HeadCapsule<TContent, TPort>, TContent, Capsule<TContent, TPort>>(this);
            else SubCapsRing.AddDataToEnd(null);
        }

        /// <summary>
        /// Add a port at the final position in the ring of input or output ports
        /// (Only the content should be allowed to use this function).
        /// </summary>
        /// <param name="ring">The ring of ports.</param>
        /// <param name="data">The data going through the port.</param>
        public void AddLastPort(ref LinkRingX<TContent, TPort> ring, TPort data = null)
        {
            var port = new Port<TContent, TPort>(data);
            Link<TContent, TPort> node = port.Links.GetNode(0);
            if (ring == null) ring = new LinkRingX<TContent, TPort>(this, node);
            else ring.AddNodeToEnd(node);
        }

        #endregion // End of ADD METHODS

        #region GET METHODS

        /// <summary>
        /// Finds whether the capsule contains sub-capsules.
        /// </summary>
        /// <returns>Whether the capsule contains sub-capsules.</returns>
        public bool Empty()
        {
            return SubCapsRing == null || SubCapsRing.Size == 0;
        }

        /// <summary>
        /// Gives the level of the capsule.
        /// </summary>
        /// <returns>The level of the capsule.</returns>
        public int GetLevel()
        {
            int n = 0;
            Capsule<TContent, TPort> cap = GetContainer();

            while (cap != null)
            {
                cap = cap.GetContainer();
                n++;
            }
            return n;
        }

        /// <summary>
        /// Gives the coordinates of some machine or sub-...-subcapsule i.g..
        /// </summary>
        /// <param name="id">The capsule's id.</param>
        /// <param name="capCoords">The capsule's coordinates.</param>
        /// <returns>Whether the capsule is found.</returns>
        public bool GetCoordinates(string id, out List<int> capCoords)
        {
            int i = 1;
            bool found = false;

            if (Data != null && Data.ToString() == id)
            {
                capCoords = null;
                found = true;
            }
            else
            {
                capCoords = null;
                if (SubCapsRing != null)
                {
                    while (!found && i <= SubCapsRing.Size)
                    {
                        List<int> coords;
                        if (SubCapsRing.GetNode(i).GetCoordinates(id, out coords))
                        {
                            if (coords != null)
                            {
                                coords.Add(i);
                                capCoords = coords;
                            }
                            else capCoords = new List<int>(1) {i};
                            found = true;
                        }
                        i++;
                    }
                }
            }
            return found;
        }

        public TPort GetInputPortData(int pos)
        {
            return GetPort(InPorts, pos).Data;
        }

        public TPort GetOutputPortData(int pos)
        {
            return GetPort(OutPorts, pos).Data;
        }

        /// <summary>
        /// Get the port at position <param name="portNum"></param> of input or output ring.
        /// </summary>
        /// <param name="ring">the ring of ports</param>
        /// <param name="portNum">he port position inside the ring of ports</param>
        /// <param name="level">the capsule's level relative to port</param>
        /// <returns>the port</returns>
        private static Port<TContent, TPort> GetPort(LinkRingX<TContent, TPort> ring, int portNum, out int level)
        {
            return ring.GetNode(portNum).GetContainerY(out level);
        }

        /// <summary>
        /// Get the port at position <param name="portNum"></param> of input or output ring.
        /// </summary>
        /// <param name="ring">the ring of ports</param>
        /// <param name="portNum">he port position inside the ring of ports</param>
        /// <returns>the port</returns>
        private static Port<TContent, TPort> GetPort(LinkRingX<TContent, TPort> ring, int portNum)
        {
            return ring.GetNode(portNum).GetContainerY();
        }

        /// <summary>
        /// Finds the capsule of some level, that some port (I or O) points to.
        /// </summary>
        /// <param name="ring">The list of ports.</param>
        /// <param name="portNum">The port number.</param>
        /// <param name="capLevel">The level of the capsule to find.</param>
        /// <returns>A reference to the pointed capsule.</returns>
        public Capsule<TContent, TPort> GetPointedCapsule(LinkRingX<TContent, TPort> ring, int portNum, int capLevel)
        {
            if (portNum > ring.Size) return null;
            var port = GetPort(ring, portNum).Points;
            if (port == null || capLevel > port.Links.Size) return null;
            return port.Links.GetNode(capLevel).GetContainerX();
        }

        #endregion // End of GET METHODS

        #region EN/UNcapsulate

        /// <summary>
        /// Encapsulates a subcapsule into another subcapsule.
        /// </summary>
        /// <param name="capNum">The capsule's position.</param>
        /// <param name="capContNum">The capsule's intended container position.</param>
        public void Encapsulate(int capNum, int capContNum)
        {
            // Detatch the subcapsule.
            Capsule<TContent, TPort> cap = SubCapsRing.ExtractNode(capNum);

            // Insert it into the specified subcapsule at the last position.
            if (capContNum > capNum) capContNum--;
            Capsule<TContent, TPort> capCont = SubCapsRing.GetNode(capContNum);
            if (capCont.SubCapsRing == null)
                capCont.SubCapsRing = new ExtendedDataRing<Capsule<TContent, TPort>, HeadCapsule<TContent, TPort>, TContent, Capsule<TContent, TPort>>(capCont, cap);
            else capCont.SubCapsRing.AddNodeToEnd(cap);
            cap = capCont.SubCapsRing.GetNode(capCont.SubCapsRing.Size);

            // Arrange the external ports input & output.
            EnCapPorts(cap.InPorts, capCont, ref capCont.InPorts, capCont.OutPorts);
            EnCapPorts(cap.OutPorts, capCont, ref capCont.OutPorts, capCont.InPorts);
        }

        /// <summary>
        /// Removes the containment of a subcapsule from a sub-subcapsule.
        /// </summary>
        /// <param name="capContNum">The capsule's current container position.</param>
        /// <param name="capNum">The capsule's position inside the container.</param>
        public void Uncapsulate(int capContNum, int capNum)
        {
            // Detach the sub-subcapsule.
            Capsule<TContent, TPort> capCont = SubCapsRing.GetNode(capContNum),
                                     cap = capCont.SubCapsRing.ExtractNode(capNum);

            // Encapsulate it into this capsule.
            SubCapsRing.AddNodeToEnd(cap);

            // Arrange the external ports input & output.
            UnCapPorts(cap.InPorts, capCont, ref capCont.OutPorts, capCont.InPorts);
            UnCapPorts(cap.OutPorts, capCont, ref capCont.InPorts, capCont.OutPorts);
        }

        private static void EnCapPorts(LinkRingX<TContent, TPort> capRing, Capsule<TContent, TPort> capCont, ref LinkRingX<TContent, TPort> associateRing, LinkRingX<TContent, TPort> disassociateRing)
        {
            CapPorts(capRing, capCont, ref  associateRing, disassociateRing, EnCapPortDel);
        }

        private static void UnCapPorts(LinkRingX<TContent, TPort> capRing, Capsule<TContent, TPort> capCont, ref LinkRingX<TContent, TPort> associateRing, LinkRingX<TContent, TPort> disassociateRing)
        {
            CapPorts(capRing, capCont, ref  associateRing, disassociateRing, UnCapPortDel);
        }

        private static void CapPorts(LinkRingX<TContent, TPort> capRing, Capsule<TContent, TPort> capCont, ref LinkRingX<TContent, TPort> associateRing, LinkRingX<TContent, TPort> disassociateRing, CapPort capAction)
        {
            if (capRing == null) return;
            for (int j = 1; j <= capRing.Size; j++)
            {
                int level;
                Port<TContent, TPort> maxCapPort, capPort = GetPort(capRing, j, out level);
                if ((maxCapPort = capPort.Points) == null) continue;
                Capsule<TContent, TPort> maxCap = maxCapPort.Links.GetNode(maxCapPort.Links.Size).GetContainerX();
                capAction(maxCap, capCont, maxCapPort, level, capPort, ref associateRing, disassociateRing);
            }
        }

        private delegate void CapPort(Capsule<TContent, TPort> maxCap, Capsule<TContent, TPort> capCont, Port<TContent, TPort> maxCapPort, int level, Port<TContent, TPort> capPort, ref LinkRingX<TContent, TPort> associateRing, LinkRingX<TContent, TPort> disassociateRing);

        private static readonly CapPort EnCapPortDel = EnCupPort, UnCapPortDel = UnCupPort;

        private static void EnCupPort(Capsule<TContent, TPort> maxCap, Capsule<TContent, TPort> capCont, Port<TContent, TPort> maxCapPort, int level, Port<TContent, TPort> capPort, ref LinkRingX<TContent, TPort> associateRing, LinkRingX<TContent, TPort> disassociateRing)
        {
            if (maxCap.GetContainer() == capCont)
                DisassociatePort(maxCapPort, disassociateRing);
            else AssociatePort(capPort, level + 1, ref associateRing, capCont);
        }

        private static void UnCupPort(Capsule<TContent, TPort> maxCap, Capsule<TContent, TPort> capCont, Port<TContent, TPort> maxCapPort, int level, Port<TContent, TPort> capPort, ref LinkRingX<TContent, TPort> associateRing, LinkRingX<TContent, TPort> disassociateRing)
        {
            if (maxCap.GetContainer() == capCont)
                AssociatePort(maxCapPort, level + 1, ref associateRing, capCont);
            else DisassociatePort(capPort, disassociateRing);
        }

        /// <summary>
        /// Weaves a port throuth some ring of ports of some capsule.
        /// </summary>
        /// <param name="port">the port</param>
        /// <param name="level">the capsule's level</param>
        /// <param name="ring">the ring</param>
        /// <param name="cap">the capsule which contains the ring</param>
        private static void AssociatePort(Port<TContent, TPort> port, int level, ref LinkRingX<TContent, TPort> ring, Capsule<TContent, TPort> cap)
        {
            if (ring == null) ring = new LinkRingX<TContent, TPort>(cap);
            else ring.AddNodeToEnd(new Link<TContent, TPort>());
            Link<TContent, TPort> ringLink = ring.GetNode(ring.Size);
            port.Links.InsertNode(ringLink, level);
        }

        /// <summary>
        /// Removes a port from some ring of ports of some capsule.
        /// </summary>
        /// <param name="port">the port</param>
        /// <param name="ring">the ring</param>
        private static void DisassociatePort(Port<TContent, TPort> port, LinkRingX<TContent, TPort> ring)
        {
            int position;
            port.Links.ExtractNode(port.Links.Size).GetContainerX(out position);
            ring.DeleteNode(position);
        }

        #endregion // End of EN/UNcapsulate

        #region Overrides

        public override void InitFrom(Capsule<TContent, TPort> cap)
        {
            base.InitFrom(cap);
            SubCapsRing = cap.SubCapsRing;
            SubCapsRing.Container = this;
            InPorts = cap.InPorts;
            InPorts.Container = this;
            OutPorts = cap.OutPorts;
            OutPorts.Container = this;
        }

        public override Capsule<TContent, TPort> Clone()
        {
            Capsule<TContent, TPort> clone = base.Clone();
            clone.SubCapsRing = SubCapsRing;
            clone.SubCapsRing.Container = clone;
            clone.InPorts = InPorts;
            clone.InPorts.Container = clone;
            clone.OutPorts = OutPorts;
            clone.OutPorts.Container = clone;
            return clone;
        }

        #endregion // End of Overrides

        /// <summary>
        /// Produces a deep-copy of the capsule without the external ports.
        /// </summary>
        /// <returns>A copy of the capsule.</returns>
        public Capsule<TContent, TPort> DeepClone()
        {
            Capsule<TContent, TPort> clone = base.Clone();
            int i;

            // Create the input and output ports collections of the clone as of equal length to the prototypes.
            if (InPorts != null && InPorts.Size != 0)
            {
                clone.InPorts = new LinkRingX<TContent, TPort>(clone);
                for (i = 2; i <= InPorts.Size; i++)
                    clone.InPorts.AddNodeToEnd(new Link<TContent, TPort>());
            }
            if (OutPorts != null && OutPorts.Size != 0)
            {
                clone.OutPorts = new LinkRingX<TContent, TPort>(clone);
                for (i = 2; i <= OutPorts.Size; i++)
                    clone.OutPorts.AddNodeToEnd(new Link<TContent, TPort>());
            }
            // Copy the inner structure of the capsule if any.
            if (Empty()) return clone;

            // Clone the head of the ring of subcapsules.
            clone.SubCapsRing = new ExtendedDataRing<Capsule<TContent, TPort>, HeadCapsule<TContent, TPort>, TContent, Capsule<TContent, TPort>>(clone, SubCapsRing.GetNode(1).DeepClone());

            // Clone the rest of the subcapsules.
            for (i = 2; i <= SubCapsRing.Size; i++)
                clone.SubCapsRing.InsertNode(SubCapsRing.GetNode(i).DeepClone(), i);

            // Copy the connections between the subcapsules,
            for (i = 1; i <= SubCapsRing.Size; i++)
            {
                // for each subcapsule,
                Capsule<TContent, TPort> cap1 = SubCapsRing.GetNode(i),
                                         cap2 = clone.SubCapsRing.GetNode(i);

                // for the input & output ports of it respectively,
                LinkRingX<TContent, TPort> ring1 = cap1.InPorts,
                                           ring2 = cap2.InPorts,
                                           ring3 = clone.InPorts;
                /*
                for (int m = 0; m <= 1; m++)
                {
                    // for each port,
                    for (int j = 1; j <= ring1.Size; j++)
                    {
                        IVLink link1 = ring1.GetNode(j),
                               link4 = link1;
                        do link1 = link1.Up;
                        while (link1.GetType() != typeof(Port<Link<TContent, TData>, TData>));
                        var port1 = (Port<Link<TContent, TData>, TData>)link1;

                        // if the port points to another port,
                        if (port1.Points == null) continue;
                        port1 = port1.Points;
                        IVLink link2 = port1.Up;
                        int l;
                        Capsule<TContent, TData> cap3 = ((Link<TContent, TData>)link2).GetContainer(out l);
                        link1 = ring2.GetNode(j);

                        // check whether the capsule, that the port points to, belongs to the subcapsules
                        // of the containing capsule, that has called 'clone' last at the recursion hierarchy,
                        IVLink link5;
                        int k;
                        if (cap3.GetContainer() != this)
                        {
                            // if not, add this port to the external ports of the containing capsule,
                            link4 = link4.Down;
                            ((Link<TContent, TData>)link4).GetContainer(out k);
                            link5 = ring3.GetNode(k);
                            link5.Up = link1;
                            link1.Down = link5;
                        }
                        else
                        {
                            // else create a new port for each of the two subcapsules and link them,
                            cap3.GetContainer(out k);
                            cap3 = clone.SubCapsRing.GetNode(k);
                            link5 = cap3.OutPorts.GetNode(l);

                            // check if this pair of ports has been fixed allready.
                            if (link5.Down != null) continue;
                            var port2 = new Port<Link<TContent, TData>, TData>(port1.Data);
                            link5.Down = port2;
                            while (link5.Up != null) link5 = link5.Up;
                            link5.Up = port2;
                            port2.Down = link5;
                            var port3 = new Port<Link<TContent, TData>, TData>(port1.Data);
                            link1.Down = port3;
                            while (link1.Up != null) link1 = link1.Up;
                            link1.Up = port3;
                            port3.Down = link1;
                            port2.Points = port3;
                        }
                    }
                    ring1 = cap1.OutPorts;
                    ring2 = cap2.OutPorts;
                    ring3 = clone.OutPorts;
                }*/
            }
            return clone;
        }

        /// <summary>
        /// Replaces some subcapsule with another capsule (presumes that the number of input and output ports
        /// of each capsule is the same accordingly. The correspondance of ports is given by the array parameter).
        /// </summary>
        /// <param name="oldCapNum">The position of the capsule which is replaced.</param>
        /// <param name="newCap">The new capsule.</param>
        /// <param name="cor">The correspondance of port numbers.</param>
        public void ReplaceCapsule(int oldCapNum, Capsule<TContent, TPort> newCap, int[] cor)
        {
            Capsule<TContent, TPort> cap = SubCapsRing.GetNode(oldCapNum);

            // Glue the new port structure into place.
            LinkRingX<TContent, TPort> ring1 = cap.InPorts,
                                       ring2 = newCap.InPorts;
            /*
            for (int j = 0; j <= 1; j++)
            {
                for (int i = 1; i <= ring1.Size; i++)
                {
                    IVLink link1 = ring1.GetNode(i);
                    IVLink link2 = link1.Down;
                    link1.Down = null;
                    link2.Up = null;
                    do link1 = link1.Up;
                    while (link1.GetType() != typeof(Port<Link<TContent, TData>, TData>));
                    link1.Down.Up = null;
                    link1.Down = null;
                    IVLink link3 = ring2.GetNode(cor[i]);
                    IVLink link4 = link3;
                    while (link3.Up != null) link3 = link3.Up;
                    link1.Down = link3;
                    link3.Up = link1;
                    link4.Down = link2;
                    link2.Up = link4;
                }
                ring1 = cap.OutPorts;
                ring2 = newCap.OutPorts;
            }
            */
            // Discart the old capsule structure.
            if (!cap.Empty()) cap.SubCapsRing.Container = null;
            cap.SubCapsRing = newCap.SubCapsRing;
            if (!newCap.Empty()) newCap.SubCapsRing.Container = cap;
            cap.Data = newCap.Data;
        }

        #endregion // End of METHODS
    }

    /// <summary>
    /// Contains the information about the expected data for the content. It is accessed
    /// through the vertical connection of the Links and controled by the capsule class.
    /// </summary>
    /// <typeparam name="TContent">conntent of the capsules</typeparam>
    /// <typeparam name="TPort">the port's data type</typeparam>
    public class Port<TContent, TPort>
        where TContent : class
        where TPort : class
    {
        public TPort Data { get; set; }

        public LinkRingY<TContent, TPort> Links { get; set; }

        public Port<TContent, TPort> Points { get; set; }

        public Port()
        {
            Links = new LinkRingY<TContent, TPort>(this);
        }

        public Port(TPort data) : this()
        {
            Data = data;
        }

        public void ConnectTo(Port<TContent, TPort> toConnect)
        {
            Points = toConnect;
            toConnect.Points = this;
        }
    }

    public class LinkRingX<TContent, TPort> :
        ExtendedMultiRingX<Link<TContent, TPort>,
            LinkHeadXY<TContent, TPort>, LinkHeadX<TContent, TPort>, LinkHeadY<TContent, TPort>,
            Capsule<TContent, TPort>, Port<TContent, TPort>>
        where TContent : class
        where TPort : class
    {
        public LinkRingX() {}

        public LinkRingX(Capsule<TContent, TPort> backLink) : base(backLink) {}

        public LinkRingX(Capsule<TContent, TPort> backLink, Link<TContent, TPort> node) : base(backLink, node) {}
    }

    public class LinkRingY<TContent, TPort> :
        ExtendedMultiRingY<Link<TContent, TPort>,
            LinkHeadXY<TContent, TPort>, LinkHeadX<TContent, TPort>, LinkHeadY<TContent, TPort>,
            Capsule<TContent, TPort>, Port<TContent, TPort>>
        where TContent : class
        where TPort : class
    {
        public LinkRingY() {}

        public LinkRingY(Port<TContent, TPort> backLink) : base(backLink) {}

        public LinkRingY(Port<TContent, TPort> backLink, Link<TContent, TPort> node) : base(backLink, node) { }
    }

    public sealed class LinkHeadXY<TContent, TPort> :
        Link<TContent, TPort>,
        IExtNodeHeadX<Capsule<TContent, TPort>>,
        IExtNodeHeadY<Port<TContent, TPort>>
        where TContent : class
        where TPort : class
    {
        /// <summary>
        /// Container object at the horizontal dimention.
        /// </summary>
        public Capsule<TContent, TPort> BackLinkX { get; set; }

        /// <summary>
        /// Container object at the vertical dimention.
        /// </summary>
        public Port<TContent, TPort> BackLinkY { get; set; }

        #region Initialization

        public LinkHeadXY() { }

        public LinkHeadXY(Capsule<TContent, TPort> backX, Port<TContent, TPort> backY)
        {
            BackLinkX = backX;
            BackLinkY = backY;
        }

        public LinkHeadXY(Link<TContent, TPort> node, Capsule<TContent, TPort> backX, Port<TContent, TPort> backY)
            : this(backX, backY)
        {
            InitFrom(node);
        }

        #endregion // End of Initialization

        /// <summary>
        /// Specifies whether the node is the head of the ring.
        /// </summary>
        /// <param name="dimention">the dimention of the ring</param>
        /// <returns>True for head elements and false for all others.</returns>
        public override bool IsHead(int dimention)
        {
            return true;
        }
    }

    public sealed class LinkHeadX<TContent, TPort> :
        Link<TContent, TPort>,
        IExtNodeHeadX<Capsule<TContent, TPort>>
        where TContent : class
        where TPort : class
    {
        /// <summary>
        /// Container object at the horizontal dimention.
        /// </summary>
        public Capsule<TContent, TPort> BackLinkX { get; set; }

        #region Initialization

        public LinkHeadX() {}

        public LinkHeadX(Capsule<TContent, TPort> back)
        {
            BackLinkX = back;
        }

        public LinkHeadX(Link<TContent, TPort> node, Capsule<TContent, TPort> back)
            : this(back)
        {
            InitFrom(node);
        }

        #endregion // End of Initialization

        /// <summary>
        /// Specifies whether the node is the head of the ring.
        /// </summary>
        /// <param name="dimention">the dimention of the ring</param>
        /// <returns>True for head elements and false for all others.</returns>
        public override bool IsHead(int dimention)
        {
            switch (dimention)
            {
                case 0: return true;
                case 1: return false;
                default: return false;
            }
        }
    }

    public sealed class LinkHeadY<TContent, TPort> :
        Link<TContent, TPort>,
        IExtNodeHeadY<Port<TContent, TPort>>
        where TContent : class
        where TPort : class
    {
        /// <summary>
        /// Container object at the vertical dimention.
        /// </summary>
        public Port<TContent, TPort> BackLinkY { get; set; }

        #region Initialization

        public LinkHeadY() { }

        public LinkHeadY(Port<TContent, TPort> back)
        {
            BackLinkY = back;
        }

        public LinkHeadY(Link<TContent, TPort> node, Port<TContent, TPort> back)
            : this(back)
        {
            InitFrom(node);
        }

        #endregion // End of Initialization

        /// <summary>
        /// Specifies whether the node is the head of the ring.
        /// </summary>
        /// <param name="dimention">the dimention of the ring</param>
        /// <returns>True for head elements and false for all others.</returns>
        public override bool IsHead(int dimention)
        {
            switch (dimention)
            {
                case 0: return false;
                case 1: return true;
                default: return false;
            }
        }
    }

    /// <summary>
    /// This class are the connection points between the horizontal rings (using next-prev direction), grouping
    /// free ports at a level of encapsulation, and the vertical (using up-down direction), attached to every port.
    /// </summary>
    /// <typeparam name="TContent">The capsule's content type.</typeparam>
    /// <typeparam name="TPort">The port's data type.</typeparam>
    public class Link<TContent, TPort> :
        ExtMultiNode2D<Link<TContent, TPort>, Capsule<TContent, TPort>, Port<TContent, TPort>>
        where TContent : class
        where TPort : class { }
}

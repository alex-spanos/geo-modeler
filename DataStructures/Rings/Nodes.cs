namespace DataStructures.Rings
{
    /// <summary>
    /// Implementation of extended head data node.
    /// </summary>
    /// <typeparam name="TData">data type</typeparam>
    /// <typeparam name="TBack">container type</typeparam>
    public sealed class ExtDataNodeHead<TData, TBack> :
        ExtDataNode<TData, TBack>,
        IExtNodeHead<TBack>
        where TData : class
        where TBack : class
    {
        /// <summary>
        /// The container object.
        /// </summary>
        public TBack BackLink { get; set; }

        #region Initialization

        public ExtDataNodeHead() {}

        public ExtDataNodeHead(TBack back)
        {
            BackLink = back;
        }

        public ExtDataNodeHead(TBack back, TData data)
            : base(data)
        {
            BackLink = back;
        }

        public ExtDataNodeHead(TBack back, ExtDataNode<TData, TBack> node)
            : this(back)
        {
            InitFrom(node);
        }

        #endregion // End of Initialization

        /// <summary>
        /// Specifies wheather the node is the head of the ring.
        /// </summary>
        /// <returns>true for head elements and false for all others</returns>
        protected override bool IsHead() { return true; }
    }

    /// <summary>
    /// An augmentation of the data node with a link back to the object that contains the ring.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    /// <typeparam name="TData">data type</typeparam>
    /// <typeparam name="TBack">container type</typeparam>
    public sealed class ExtDataNodeHead<TNode, TData, TBack> :
        ExtDataNode<TNode, TData, TBack>,
        IExtNodeHead<TBack>
        where TNode : ExtDataNode<TNode, TData, TBack>, new()
        where TData : class
        where TBack : class
    {
        /// <summary>
        /// The container object.
        /// </summary>
        public TBack BackLink { get; set; }

        #region Initialization

        public ExtDataNodeHead() {}

        public ExtDataNodeHead(TBack back)
        {
            BackLink = back;
        }

        public ExtDataNodeHead(TBack back, TData data)
            : base(data)
        {
            BackLink = back;
        }

        public ExtDataNodeHead(TNode node, TBack back)
            : this(back)
        {
            InitFrom(node);
        }

        #endregion // End of Initialization

        /// <summary>
        /// Specifies wheather the node is the head of the ring.
        /// </summary>
        /// <returns>True for head elements and false for all others.</returns>
        protected override bool IsHead() { return true; }
    }

    /// <summary>
    /// Implementation of extended data node.
    /// </summary>
    /// <typeparam name="TData">data type</typeparam>
    /// <typeparam name="TBack">back type</typeparam>
    public class ExtDataNode<TData, TBack> :
        ExtDataNode<ExtDataNode<TData, TBack>, TData, TBack>
        where TData : class
        where TBack : class
    {
        #region Initialization

        public ExtDataNode() {}

        public ExtDataNode(TData data)
            : base(data) {}

        #endregion // End of Initialization
    }

    /// <summary>
    /// Extended wrapper class for the elements of the ring providing node functionality
    /// plus track back reference capability throught the head element of the ring.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    /// <typeparam name="TData">linked objects type</typeparam>
    /// <typeparam name="TBack">container type</typeparam>
    public class ExtDataNode<TNode, TData, TBack> :
        ExtNode<TNode, TBack>
        where TNode : ExtDataNode<TNode, TData, TBack>, new()
        where TData : class
        where TBack : class
    {
        /// <summary>
        /// The content of the node.
        /// </summary>
        public TData Data { get; set; }

        #region Initialization

        public ExtDataNode() {}

        public ExtDataNode(TData data)
        {
            Data = data;
        }

        #endregion // End of Initialization

        /// <summary>
        /// Initializes the instance from an other one.
        /// </summary>
        /// <param name="node">node to copy</param>
        public override void InitFrom(TNode node)
        {
            base.InitFrom(node);
            Data = node.Data;
        }

        /// <summary>
        /// Initializes the extended data node copying another one.
        /// </summary>
        public override TNode Clone()
        {
            TNode clone = base.Clone();
            clone.Data = Data;
            return clone;
        }
    }

    /// <summary>
    /// An augmentation of the common node with a link back to the object that contains the ring.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    /// <typeparam name="TBack">container type</typeparam>
    public sealed class ExtNodeHead<TNode, TBack> :
        ExtNode<TNode, TBack>,
        IExtNodeHead<TBack>
        where TNode : ExtNode<TNode, TBack>, new()
        where TBack : class
    {
        /// <summary>
        /// Container object.
        /// </summary>
        public TBack BackLink { get; set; }

        #region Initialization

        public ExtNodeHead() {}

        public ExtNodeHead(TBack back)
        {
            BackLink = back;
        }

        public ExtNodeHead(TNode node, TBack back)
            : this(back)
        {
            InitFrom(node);
        }

        #endregion // End of Initialization

        /// <summary>
        /// Specifies wheather the node is the head of the ring.
        /// </summary>
        /// <returns>true for head elements and false for all others</returns>
        protected override bool IsHead() { return true; }
    }

    /// <summary>
    /// Extended base class for the elements of the ring providing node functionality
    /// plus track back reference capability throught the head element of the ring.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    /// <typeparam name="TBack">container type</typeparam>
    public class ExtNode<TNode, TBack> :
        Node<TNode>
        where TNode : ExtNode<TNode, TBack>, new()
        where TBack : class
    {
        /// <summary>
        /// Specifies whether the node is the head of the ring.
        /// </summary>
        /// <returns>True for head elements and false for all others.</returns>
        protected virtual bool IsHead() { return false; }

        /// <summary>
        /// Initializes the instance from an other one.
        /// </summary>
        /// <param name="node">node to copy</param>
        public virtual void InitFrom(TNode node)
        {
            Next = node.Next;
            Prev = node.Prev;
        }

        /// <summary>
        /// Initializes the basic extended node copying another one.
        /// </summary>
        public virtual TNode Clone()
        {
            return new TNode
            {
                Next = Next,
                Prev = Prev
            };
        }

        /// <summary>
        /// Travels back to the container object throught the ring.
        /// </summary>
        /// <returns>the container of the ring</returns>
        public TBack GetContainer()
        {
            int pos;
            return GetContainer(out pos);
        }

        /// <summary>
        /// Travels back to the container object throught the ring.
        /// </summary>
        /// <param name="pos">position of the node in the ring</param>
        /// <returns>the container of the ring</returns>
        public TBack GetContainer(out int pos)
        {
            ExtNode<TNode, TBack> node = this;
            pos = 1;
            while (!node.IsHead())
            {
                node = node.Next;
                pos++;
            }
            return ((IExtNodeHead<TBack>)node).BackLink;
        }
    }

    /// <summary>
    /// The interface that all head node classes must implement.
    /// </summary>
    /// <typeparam name="TBack">container type</typeparam>
    public interface IExtNodeHead<TBack>
        where TBack : class
    {
        /// <summary>
        /// Container object.
        /// </summary>
        TBack BackLink { get; set; }
    }

    /// <summary>
    /// Implementation of data node.
    /// </summary>
    /// <typeparam name="TData">data type</typeparam>
    public class DataNode<TData> :
        DataNode<DataNode<TData>, TData>
        where TData : class
    {
        #region Initialization

        public DataNode() {}

        public DataNode(TData data)
            : base(data) {}

        #endregion // End of Initialization
    }

    /// <summary>
    /// Wrapper class for the elements of the ring providing node functionality.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    /// <typeparam name="TData">linked objects type</typeparam>
    public class DataNode<TNode, TData> :
        Node<TNode>
        where TNode : class
        where TData : class
    {
        /// <summary>
        /// The content of the node.
        /// </summary>
        public TData Data { get; set; }

        #region Initialization

        public DataNode() {}

        public DataNode(TData data)
        {
            Data = data;
        }

        #endregion // End of Initialization
    }

    /// <summary>
    /// Base class for the elements of the ring providing node functionality.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    public class Node<TNode>
        where TNode : class
    {
        /// <summary>
        /// The next node in the ring.
        /// </summary>
        public TNode Next { get; set; }

        /// <summary>
        /// The previous node in the ring.
        /// </summary>
        public TNode Prev { get; set; }
    }
}

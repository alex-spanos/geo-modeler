namespace DataStructures.Rings.Multidimentional
{
    /// <summary>
    /// Implementation of extended data node.
    /// </summary>
    /// <typeparam name="TData">data type</typeparam>
    /// <typeparam name="TBack">back type</typeparam>
    public class ExtDataMultiNode<TData> :
        ExtDataMultiNode<ExtDataMultiNode<TData>, TData>
        where TData : class
    {
        #region Initialization

        public ExtDataMultiNode() { }

        public ExtDataMultiNode(int space, TData data)
            : base(space, data) { }

        #endregion // End of Initialization
    }

    /// <summary>
    /// Extended wrapper class for the elements of the ring providing node functionality
    /// plus track back reference capability throught the head element of the ring.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    /// <typeparam name="TData">linked objects type</typeparam>
    /// <typeparam name="TBack">container type</typeparam>
    public class ExtDataMultiNode<TNode, TData> :
        ExtMultiNode<TNode>
        where TNode : ExtDataMultiNode<TNode, TData>, new()
        where TData : class
    {
        /// <summary>
        /// The content of the node.
        /// </summary>
        public TData Data { get; set; }

        #region Initialization

        public ExtDataMultiNode() {}

        protected ExtDataMultiNode(int space) : base(space) {}

        public ExtDataMultiNode(int space, TData data)
            : base(space)
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

    public abstract class ExtMultiNode<TNode> :
        MultiNode<TNode>
        where TNode : ExtMultiNode<TNode>, new()
    {
        protected ExtMultiNode() {}

        protected ExtMultiNode(int space) : base(space) {}

        /// <summary>
        /// Specifies whether the node is the head of the ring.
        /// </summary>
        /// <param name="dimention">the dimention of the ring</param>
        /// <returns>True for head elements and false for all others.</returns>
        public virtual bool IsHead(int dimention)
        {
            return false;
        }

        /// <summary>
        /// Initializes the instance from an other one.
        /// </summary>
        /// <param name="node">node to copy</param>
        public virtual void InitFrom(TNode node)
        {
            node.Next.CopyTo(Next, 0);
            node.Prev.CopyTo(Prev, 0);
        }

        /// <summary>
        /// Initializes the basic extended node copying another one.
        /// </summary>
        public virtual TNode Clone()
        {
            var clone = new TNode();
            Next.CopyTo(clone.Next, 0);
            Prev.CopyTo(clone.Prev, 0);
            return clone;
        }

        /// <summary>
        /// Travels back to the horizontal container object throught the ring.
        /// </summary>
        /// <param name="dim">dimention of container</param>
        /// <param name="pos">position of the node in the ring</param>
        /// <returns>the head of the ring in some dimention</returns>
        protected ExtMultiNode<TNode> GetContainer(int dim, out int pos)
        {
            ExtMultiNode<TNode> node = this;
            pos = 1;
            while (!node.IsHead(dim))
            {
                node = node.Next[dim];
                pos++;
            }
            return node;
        }
    }

    /// <summary>
    /// Implementation of data node.
    /// </summary>
    /// <typeparam name="TData">data type</typeparam>
    public class DataMultiNode<TData> :
        DataMultiNode<DataMultiNode<TData>, TData>
        where TData : class
    {
        #region Initialization

        public DataMultiNode() {}

        public DataMultiNode(TData data)
            : base(data) {}

        #endregion // End of Initialization
    }

    /// <summary>
    /// Wrapper class for the elements of the ring providing node functionality.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    /// <typeparam name="TData">linked objects type</typeparam>
    public class DataMultiNode<TNode, TData> :
        MultiNode<TNode>
        where TNode : class
        where TData : class
    {
        /// <summary>
        /// The content of the node.
        /// </summary>
        public TData Data { get; set; }

        #region Initialization

        public DataMultiNode() { }

        public DataMultiNode(TData data)
        {
            Data = data;
        }

        #endregion // End of Initialization
    }

    /// <summary>
    /// Base class for the elements of the torus providing node functionality.
    /// </summary>
    /// <typeparam name="TNode">node type</typeparam>
    public class MultiNode<TNode>
        where TNode : class
    {
        /// <summary>
        /// The next node in the torus in some dimention.
        /// </summary>
        public TNode[] Next { get; set; }

        /// <summary>
        /// The previous node in the torus in some dimention.
        /// </summary>
        public TNode[] Prev { get; set; }

        public MultiNode() {}

        public MultiNode(int space)
        {
            Next = new TNode[space];
            Prev = new TNode[space];
        }
    }
}

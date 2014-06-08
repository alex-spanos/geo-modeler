namespace DataStructures.Rings.Multidimentional
{
    public class DataMultiRing<TData> :
        DataMultiRing<DataMultiNode<TData>, TData>
        where TData : class
    {
        #region Initialization

        public DataMultiRing() : base(0) { }

        protected DataMultiRing(int dimention) : base(dimention) { }

        #endregion // End of Initialization
    }

    /// <summary>
    /// This class manipulates rings (doubly-connected, closed-loop dynamic lists)
    /// of containers of objects of type <see cref="TData"/>.
    /// </summary>
    /// <typeparam name="TData">The linked objects type.</typeparam>
    /// <typeparam name="TNode">The node wrapping class.</typeparam>
    public class DataMultiRing<TNode, TData> :
        MultiRing<TNode>
        where TNode : DataMultiNode<TNode, TData>, new()
        where TData : class
    {
        #region Initialization

        public DataMultiRing() : base(0) {}

        protected DataMultiRing(int dimention) : base(dimention) { }

        #endregion // End of Initialization

        #region Data manipulation

        /// <summary>
        /// Get a reference to the data of the node at some position.
        /// </summary>
        /// <param name="pos">The position of the node whose data are requested.</param>
        /// <returns>The node at position <paramref name="pos"/>.</returns>
        public TData GetData(int pos)
        {
            return GetNode(pos).Data;
        }

        /// <summary>
        /// Adds a node at the last position of the ring.
        /// </summary>
        /// <param name="data">The content of the node.</param>
        public void AddDataToEnd(TData data)
        {
            AddNodeToEnd(new TNode { Data = data });
        }

        /// <summary>
        /// Inserts a node at some position.
        /// </summary>
        /// <param name="data">The content of the node.</param>
        /// <param name="pos">The insertion position.</param>
        public void InsertData(TData data, int pos)
        {
            InsertNode(new TNode { Data = data }, pos);
        }

        /// <summary>
        /// Extracts the node from some position.
        /// </summary>
        /// <param name="pos">The position of the node to extract.</param>
        /// <returns>The extracted node.</returns>
        public TData ExtractData(int pos)
        {
            return ExtractNode(pos).Data;
        }

        #endregion // End of Data manipulation

        public TData this[int i]
        {
            get { return GetData(i); }
            set { GetNode(i).Data = value; }
        }
    }

    /// <summary>
    /// This class manipulates rings (doubly-connected, closed-loop dynamic lists)
    /// of wrapper objects of type <see cref="TNode"/>, participating in a multidementional system.
    /// </summary>
    /// <typeparam name="TNode">The node base class.</typeparam>
    public class MultiRing<TNode> :
        RingBase<TNode>
        where TNode : MultiNode<TNode>
    {
        #region Fields

        /// <summary>
        /// Dimention of the ring.
        /// </summary>
        protected readonly int Dim;

        #endregion // End of Fields

        #region Initialization

        public MultiRing()
        {
            Dim = 0;
        }

        protected MultiRing(int dimention)
        {
            Dim = dimention;
        }

        #endregion // End of Initialization

        #region Positioning system

        /// <summary>
        /// Moves the main 'pointer' at some position.
        /// </summary>
        /// <param name="pos">The position inside the ring to move to.</param>
        protected override void MoveCur(int pos)
        {
            TNode p = Cur;

            if (Size == 0) return;
            int d1 = NormalizeIndex(pos),
                d2 = d1 - Pos;

            if (d2 == 0) return;
            int i;
            if (d2 > 0)
            {
                if (d2 > N) for (i = 0; i < Size - d2; i++) p = p.Prev[Dim];
                else for (i = 0; i < d2; i++) p = p.Next[Dim];
            }
            else
            {
                int j = Size + d2;
                if (j > N) for (i = 0; i < -d2; i++) p = p.Prev[Dim];
                else for (i = 0; i < j; i++) p = p.Next[Dim];
            }
            Cur = p;
            Pos = d1;
        }

        #endregion // End of Positioning system

        #region Node manipulation

        /// <summary>
        /// Common insert pattern in certain insert methods.
        /// </summary>
        /// <param name="node">The node to insert.</param>
        /// <param name="pos">The insertion position.</param>
        protected override void InsertPattern(TNode node, int pos)
        {
            MoveCur(pos);
            TNode p = Cur.Prev[Dim];
            node.Next[Dim] = Cur;
            node.Prev[Dim] = p;
            p.Next[Dim] = node;
            Cur.Prev[Dim] = node;
            IncreaseSize();
        }

        /// <summary>
        /// Adds a node to an empty ring.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void AddNodeToEmptyRing(TNode node)
        {
            node.Next[Dim] = node;
            node.Prev[Dim] = node;
            Cur = node;
            Pos = 1;
            IncreaseSize();
        }

        /// <summary>
        /// Extracts the node from some position.
        /// </summary>
        /// <param name="pos">The position of the node to extract.</param>
        /// <returns>The extracted node.</returns>
        public override TNode ExtractNode(int pos)
        {
            if (Size == 0) return null;
            MoveCur(pos);
            TNode p = Cur;
            p.Next[Dim].Prev[Dim] = p.Prev[Dim];
            p.Prev[Dim].Next[Dim] = p.Next[Dim];
            if (Size == 1)
            {
                Cur = null;
                Pos = 0;
            }
            else
            {
                Cur = p.Next[Dim];
                if (Size == Pos) Pos = 1;
            }
            DecreaseSize();
            p.Next = null;
            p.Prev = null;
            return p;
        }

        #endregion // End of Node manipulation
    }
}

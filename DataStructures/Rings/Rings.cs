namespace DataStructures.Rings
{
    /// <summary>
    /// This class manipulates rings (doubly-connected, closed-loop dynamic lists)
    /// of objects of type <see cref="TData"/> and provides a link to the container object.
    /// </summary>
    /// <typeparam name="TData">The linked objects type.</typeparam>
    /// <typeparam name="TBack">The container object type.</typeparam>
    public class ExtendedDataRing<TData, TBack> :
        ExtendedDataRing<ExtDataNode<TData, TBack>, ExtDataNodeHead<TData, TBack>, TData, TBack>
        where TData : class
        where TBack : class
    {
        #region Initialization

        public ExtendedDataRing() { }

        public ExtendedDataRing(TBack backLink)
            : base(backLink) { }

        public ExtendedDataRing(TBack backLink, TData data)
            : base(backLink, data) { }

        #endregion // End of Initialization
    }

    /// <summary>
    /// This class manipulates rings (doubly-connected, closed-loop dynamic lists)
    /// of objects data containers of type <see cref="TData"/>.
    /// </summary>
    /// <typeparam name="TData">The linked objects type.</typeparam>
    public class DataRing<TData> :
        DataRing<DataNode<TData>, TData>
        where TData : class
    {
        #region Initialization

        public DataRing() { }

        public DataRing(TData data)
            : base(data) { }

        #endregion // End of Initialization
    }

    /// <summary>
    /// This class manipulates rings (doubly-connected, closed-loop dynamic lists)
    /// of objects of type <see cref="TData"/> and provides a link to the container object.
    /// </summary>
    /// <typeparam name="TData">The linked objects type.</typeparam>
    /// <typeparam name="TBack">The container object type.</typeparam>
    /// <typeparam name="TNode">The node wrapping class.</typeparam>
    /// <typeparam name="THeadNode">The head node wrapping type.</typeparam>
    public class ExtendedDataRing<TNode, THeadNode, TData, TBack> :
        ExtendedRing<TNode, THeadNode, TBack>
        where TNode : ExtDataNode<TNode, TData, TBack>, new()
        where THeadNode : TNode, IExtNodeHead<TBack>, new()
        where TData : class
        where TBack : class
    {
        #region Initialization

        public ExtendedDataRing() { }

        public ExtendedDataRing(TBack backLink)
            : base(backLink) {}

        public ExtendedDataRing(TBack backLink, TData data)
            : this(backLink)
        {
            GetNode(1).Data = data;
        }

        public ExtendedDataRing(TBack backLink, TNode node)
            : base(backLink, node) {}

        public ExtendedDataRing(THeadNode head)
            : base(head) { }

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
    /// of containers of objects of type <see cref="TData"/>.
    /// </summary>
    /// <typeparam name="TNode">The node wrapping class.</typeparam>
    /// <typeparam name="TData">The linked objects type.</typeparam>
    public class DataRing<TNode, TData> :
        Ring<TNode>
        where TNode : DataNode<TNode, TData>, new()
        where TData : class
    {
        #region Initialization

        public DataRing() { }

        public DataRing(TData data)
        {
            AddDataToEnd(data);
        }

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
    /// of wrapper objects of type <see cref="TNode"/> and provides a link to the container object.
    /// </summary>
    /// <typeparam name="TBack">The container object type.</typeparam>
    /// <typeparam name="TNode">The node wrapping class.</typeparam>
    /// <typeparam name="THeadNode">The head node wrapping type.</typeparam>
    public class ExtendedRing<TNode, THeadNode, TBack> :
        Ring<TNode>
        where TNode : ExtNode<TNode, TBack>, new()
        where THeadNode : TNode, IExtNodeHead<TBack>, new()
        where TBack : class
    {
        /// <summary>
        /// The ring's container object.
        /// </summary>
        public TBack Container
        {
            get
            {
                var head = (IExtNodeHead<TBack>) GetNode(1);
                return head != null ? head.BackLink : null;
            }
            set
            {
                var head = (IExtNodeHead<TBack>) GetNode(1);
                if (head != null) head.BackLink = value;
            }
        }

        #region Initialization

        public ExtendedRing() { }

        public ExtendedRing(TBack backLink)
        {
            base.AddNodeToEnd(new THeadNode {BackLink = backLink});
        }

        public ExtendedRing(TBack backLink, TNode node)
        {
            AddNodeToEnd(node);
            ((THeadNode) GetNode(1)).BackLink = backLink;
        }

        public ExtendedRing(THeadNode head)
        {
            base.AddNodeToEnd(head);
        }

        #endregion // End of Initialization

        #region Node manipulation overrides

        /// <summary>
        /// Adds a node at the last position of the ring.
        /// </summary>
        /// <param name="node">The content of the node.</param>
        public sealed override void AddNodeToEnd(TNode node)
        {
            if (Size == 0)
            {
                var head = new THeadNode();
                head.InitFrom(node);
                base.AddNodeToEnd(head);
            }
            else base.AddNodeToEnd(node);
        }

        /// <summary>
        /// Inserts a node at some position.
        /// </summary>
        /// <param name="node">The content of the node.</param>
        /// <param name="pos">The insertion position.</param>
        public sealed override void InsertNode(TNode node, int pos)
        {
            if ((pos = NormalizeIndex(pos)) == 1)
            {
                var head = new THeadNode();
                head.InitFrom(node);
                base.InsertNode(head, 1);
                if (Size == 1) return;
                var oldHead = (THeadNode) base.ExtractNode(2);
                head.BackLink = oldHead.BackLink;
                base.InsertNode(oldHead.Clone(), 2);
            }
            else base.InsertNode(node, pos);
        }

        /// <summary>
        /// Extracts the node from some position.
        /// </summary>
        /// <param name="pos">The position of the node to extract.</param>
        /// <returns>The extracted node's content.</returns>
        public sealed override TNode ExtractNode(int pos)
        {
            if ((pos = NormalizeIndex(pos)) == 1 && Size != 0)
            {
                TNode oldHead = base.ExtractNode(1);
                if (Size == 0) return oldHead;
                TNode node = base.ExtractNode(1);
                var head = new THeadNode {BackLink = ((IExtNodeHead<TBack>) oldHead).BackLink};
                head.InitFrom(node);
                base.InsertNode(head, 1);
                return oldHead;
            }
            return base.ExtractNode(pos);
        }

        #endregion // End of Node manipulation overrides
    }

    /// <summary>
    /// This class manipulates rings (doubly-connected, closed-loop dynamic lists)
    /// of wrapper objects of type <see cref="TNode"/>.
    /// </summary>
    /// <typeparam name="TNode">The node base class.</typeparam>
    public class Ring<TNode> :
        RingBase<TNode>
        where TNode : Node<TNode>
    {
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
                if (d2 > N) for (i = 0; i < Size - d2; i++) p = p.Prev;
                else for (i = 0; i < d2; i++) p = p.Next;
            }
            else
            {
                int j = Size + d2;
                if (j > N) for (i = 0; i < -d2; i++) p = p.Prev;
                else for (i = 0; i < j; i++) p = p.Next;
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
            TNode p = Cur.Prev;
            node.Next = Cur;
            node.Prev = p;
            p.Next = node;
            Cur.Prev = node;
            IncreaseSize();
        }

        /// <summary>
        /// Adds a node to an empty ring.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void AddNodeToEmptyRing(TNode node)
        {
            node.Next = node;
            node.Prev = node;
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
            p.Next.Prev = p.Prev;
            p.Prev.Next = p.Next;
            if (Size == 1)
            {
                Cur = null;
                Pos = 0;
            }
            else
            {
                Cur = p.Next;
                if (Size == Pos) Pos = 1;
            }
            DecreaseSize();
            p.Next = null;
            p.Prev = null;
            return p;
        }

        #endregion // End of Node manipulation
    }

    public abstract class RingBase<TNode>
        where TNode : class
    {
        #region Fields

        /// <summary>
        /// The pointer to the current node at position <see cref="Pos"/>.
        /// </summary>
        protected TNode Cur;

        /// <summary>
        /// The position of the current pointer <see cref="Cur"/>.
        /// </summary>
        protected int Pos;

        ///<summary>
        /// The n in: <see cref="Size"/> = 2n or <see cref="Size"/> = 2n + 1.
        /// </summary>
        protected int N;

        /// <summary>
        /// Value is zero if the size is even and one if odd.
        /// </summary>
        private byte _parity;

        /// <summary>
        /// The length of the ring.
        /// </summary>
        public int Size { get; set; }

        #endregion // End of Fields

        #region Positioning system

        /// <summary>
        /// Increases the size counter by one.
        /// </summary>
        protected void IncreaseSize()
        {
            Size++;
            if (_parity == 0) _parity++;
            else
            {
                _parity--;
                N++;
            }
        }

        /// <summary>
        /// Decreases the size counter by one.
        /// </summary>
        protected void DecreaseSize()
        {
            Size--;
            if (_parity == 0)
            {
                _parity++;
                N--;
            }
            else _parity--;
        }

        /// <summary>
        /// Moves the main 'pointer' at some position.
        /// </summary>
        /// <param name="pos">The position inside the ring to move to.</param>
        protected abstract void MoveCur(int pos);

        /// <summary>
        /// Normalises an arbitrary integer as a ring index.
        /// </summary>
        /// <param name="index">The arbitrary number.</param>
        /// <returns>The index position inside the ring.</returns>
        protected int NormalizeIndex(int index)
        {
            int rem;

            if (index == 0) return Size;
            if (index > 0)
            {
                if (index <= Size) return index;
                rem = index % Size;
                return rem == 0 ? Size : rem;
            }
            if (index <= -Size) return Size - index;
            rem = index % Size;
            return rem == 0 ? Size : Size - rem;
        }

        #endregion // End of Positioning system

        #region Node manipulation

        /// <summary>
        /// Get a reference to the node at some position.
        /// </summary>
        /// <param name="pos">The position of the node to get.</param>
        /// <returns>The node at position <paramref name="pos"/>.</returns>
        public TNode GetNode(int pos)
        {
            MoveCur(pos);
            return Cur;
        }

        /// <summary>
        /// Adds a node of any derived type as the last element of the ring.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public virtual void AddNodeToEnd(TNode node)
        {
            if (Size == 0) AddNodeToEmptyRing(node);
            else InsertPattern(node, 1);
        }

        /// <summary>
        /// Inserts a node of any derived type to the ring.
        /// </summary>
        /// <param name="node">The node to insert.</param>
        /// <param name="pos">The insertion position.</param>
        public virtual void InsertNode(TNode node, int pos)
        {
            if (Size == 0) AddNodeToEmptyRing(node);
            else
            {
                InsertPattern(node, pos);
                Pos = NormalizeIndex(Pos + 1);
            }
        }

        /// <summary>
        /// Common insert pattern in certain insert methods.
        /// </summary>
        /// <param name="node">The node to insert.</param>
        /// <param name="pos">The insertion position.</param>
        protected abstract void InsertPattern(TNode node, int pos);

        /// <summary>
        /// Adds a node to an empty ring.
        /// </summary>
        /// <param name="node">The node.</param>
        protected abstract void AddNodeToEmptyRing(TNode node);

        /// <summary>
        /// Extracts the node from some position.
        /// </summary>
        /// <param name="pos">The position of the node to extract.</param>
        /// <returns>The extracted node.</returns>
        public abstract TNode ExtractNode(int pos);

        /// <summary>
        /// Deletes the node at some position.
        /// </summary>
        /// <param name="pos">The position of the node to delete.</param>
        public void DeleteNode(int pos)
        {
            ExtractNode(pos);
        }

        #endregion // End of Node manipulation
    }
}

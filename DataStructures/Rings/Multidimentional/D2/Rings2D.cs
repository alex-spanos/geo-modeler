namespace DataStructures.Rings.Multidimentional.D2
{
    public class ExtendedDataMultiRingX<TNode, TData, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY> :
        ExtendedDataMultiRing2D<TNode, TData, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY>
        where TNode : ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>, new()
        where TData : class
        where THeadNodeXY : TNode, IExtNodeHeadX<TBackX>, IExtNodeHeadY<TBackY>, new()
        where THeadNodeX : TNode, IExtNodeHeadX<TBackX>, new()
        where THeadNodeY : TNode, IExtNodeHeadY<TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// The ring's container object in the first dimention.
        /// </summary>
        public TBackX Container
        {
            get
            {
                var head = (IExtNodeHeadX<TBackX>)GetNode(1);
                return head != null ? head.BackLinkX : null;
            }
            set
            {
                var head = (IExtNodeHeadX<TBackX>)GetNode(1);
                if (head != null) head.BackLinkX = value;
            }
        }

        #region Initialization

        public ExtendedDataMultiRingX() : base(0) { }

        public ExtendedDataMultiRingX(TBackX backLink)
            : this()
        {
            var head = new THeadNodeX { BackLinkX = backLink };
            AddNodeToEnd(head);
        }

        public ExtendedDataMultiRingX(TBackX backLink, TNode node)
            : this()
        {
            AddNodeToEnd(node);
            ((THeadNodeX)GetNode(1)).BackLinkX = backLink;
        }

        public ExtendedDataMultiRingX(THeadNodeX head)
            : this()
        {
            AddNodeToEnd(head);
        }

        #endregion // End of Initialization

        #region Heads implementations

        protected override THeadNodeXY CreateDoubleHead(TNode node)
        {
            return new THeadNodeXY { BackLinkY = ((IExtNodeHeadY<TBackY>)node).BackLinkY };
        }

        protected override TNode CreatePrimaryHead()
        {
            return CreateHeadX();
        }

        protected override TNode CreatePrimaryHead(TNode node)
        {
            return CreateHeadX(node);
        }

        protected override TNode CreateSecondaryHead(TNode node)
        {
            return CreateHeadY(node);
        }

        protected override void InitHead(THeadNodeXY head, TNode node)
        {
            head.BackLinkX = ((IExtNodeHeadX<TBackX>)node).BackLinkX;
        }

        protected override void InitHead(TNode head, TNode node)
        {
            ((IExtNodeHeadX<TBackX>)head).BackLinkX = ((IExtNodeHeadX<TBackX>)node).BackLinkX;
        }

        #endregion // End of Heads implementations
    }

    public class ExtendedDataMultiRingY<TNode, TData, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY> :
        ExtendedDataMultiRing2D<TNode, TData, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY>
        where TNode : ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>, new()
        where TData : class
        where THeadNodeXY : TNode, IExtNodeHeadX<TBackX>, IExtNodeHeadY<TBackY>, new()
        where THeadNodeX : TNode, IExtNodeHeadX<TBackX>, new()
        where THeadNodeY : TNode, IExtNodeHeadY<TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// The ring's container object in the second dimention.
        /// </summary>
        public TBackY Container
        {
            get
            {
                var head = (IExtNodeHeadY<TBackY>)GetNode(1);
                return head != null ? head.BackLinkY : null;
            }
            set
            {
                var head = (IExtNodeHeadY<TBackY>)GetNode(1);
                if (head != null) head.BackLinkY = value;
            }
        }

        #region Initialization

        public ExtendedDataMultiRingY() : base(1) { }

        public ExtendedDataMultiRingY(TBackY backLink)
            : this()
        {
            var head = new THeadNodeY { BackLinkY = backLink };
            AddNodeToEnd(head);
        }

        public ExtendedDataMultiRingY(TBackY backLink, TNode node)
            : this()
        {
            AddNodeToEnd(node);
            ((THeadNodeY)GetNode(1)).BackLinkY = backLink;
        }

        public ExtendedDataMultiRingY(THeadNodeY head)
            : this()
        {
            AddNodeToEnd(head);
        }

        #endregion // End of Initialization

        #region Heads implementations

        protected override THeadNodeXY CreateDoubleHead(TNode node)
        {
            return new THeadNodeXY { BackLinkX = ((IExtNodeHeadX<TBackX>)node).BackLinkX };
        }

        protected override TNode CreatePrimaryHead()
        {
            return CreateHeadY();
        }

        protected override TNode CreatePrimaryHead(TNode node)
        {
            return CreateHeadY(node);
        }

        protected override TNode CreateSecondaryHead(TNode node)
        {
            return CreateHeadX(node);
        }

        protected override void InitHead(THeadNodeXY head, TNode node)
        {
            head.BackLinkY = ((IExtNodeHeadY<TBackY>)node).BackLinkY;
        }

        protected override void InitHead(TNode head, TNode node)
        {
            ((IExtNodeHeadY<TBackY>)head).BackLinkY = ((IExtNodeHeadY<TBackY>)node).BackLinkY;
        }

        #endregion // End of Heads implementations
    }

    public class ExtendedMultiRingX<TNode, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY> :
        ExtendedMultiRing2D<TNode, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY>
        where TNode : ExtMultiNode2D<TNode, TBackX, TBackY>, new()
        where THeadNodeXY : TNode, IExtNodeHeadX<TBackX>, IExtNodeHeadY<TBackY>, new()
        where THeadNodeX : TNode, IExtNodeHeadX<TBackX>, new()
        where THeadNodeY : TNode, IExtNodeHeadY<TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// The ring's container object in the first dimention.
        /// </summary>
        public TBackX Container
        {
            get
            {
                var head = (IExtNodeHeadX<TBackX>)GetNode(1);
                return head != null ? head.BackLinkX : null;
            }
            set
            {
                var head = (IExtNodeHeadX<TBackX>)GetNode(1);
                if (head != null) head.BackLinkX = value;
            }
        }

        #region Initialization

        public ExtendedMultiRingX() : base(0) {}

        public ExtendedMultiRingX(TBackX backLink) : this()
        {
            var head = new THeadNodeX {BackLinkX = backLink};
            AddNodeToEnd(head);
        }

        public ExtendedMultiRingX(TBackX backLink, TNode node) : this()
        {
            AddNodeToEnd(node);
            ((IExtNodeHeadX<TBackX>) GetNode(1)).BackLinkX = backLink;
        }

        public ExtendedMultiRingX(THeadNodeX head) : this()
        {
            AddNodeToEnd(head);
        }

        #endregion // End of Initialization

        #region Heads implementations

        protected override THeadNodeXY CreateDoubleHead(TNode node)
        {
            return new THeadNodeXY {BackLinkY = ((IExtNodeHeadY<TBackY>) node).BackLinkY};
        }

        protected override TNode CreatePrimaryHead()
        {
            return CreateHeadX();
        }

        protected override TNode CreatePrimaryHead(TNode node)
        {
            return CreateHeadX(node);
        }

        protected override TNode CreateSecondaryHead(TNode node)
        {
            return CreateHeadY(node);
        }

        protected override void InitHead(THeadNodeXY head, TNode node)
        {
            head.BackLinkX = ((IExtNodeHeadX<TBackX>) node).BackLinkX;
        }

        protected override void InitHead(TNode head, TNode node)
        {
            ((IExtNodeHeadX<TBackX>) head).BackLinkX = ((IExtNodeHeadX<TBackX>) node).BackLinkX;
        }

        #endregion // End of Heads implementations
    }

    public class ExtendedMultiRingY<TNode, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY> :
        ExtendedMultiRing2D<TNode, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY>
        where TNode : ExtMultiNode2D<TNode, TBackX, TBackY>, new()
        where THeadNodeXY : TNode, IExtNodeHeadX<TBackX>, IExtNodeHeadY<TBackY>, new()
        where THeadNodeX : TNode, IExtNodeHeadX<TBackX>, new()
        where THeadNodeY : TNode, IExtNodeHeadY<TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// The ring's container object in the second dimention.
        /// </summary>
        public TBackY Container
        {
            get
            {
                var head = (IExtNodeHeadY<TBackY>)GetNode(1);
                return head != null ? head.BackLinkY : null;
            }
            set
            {
                var head = (IExtNodeHeadY<TBackY>)GetNode(1);
                if (head != null) head.BackLinkY = value;
            }
        }

        #region Initialization

        public ExtendedMultiRingY() : base(1) {}

        public ExtendedMultiRingY(TBackY backLink) : this()
        {
            var head = new THeadNodeY {BackLinkY = backLink};
            base.AddNodeToEnd(head);
        }

        public ExtendedMultiRingY(TBackY backLink, TNode node) : this()
        {
            AddNodeToEnd(node);
            ((IExtNodeHeadY<TBackY>) GetNode(1)).BackLinkY = backLink;
        }

        public ExtendedMultiRingY(THeadNodeY head) : this()
        {
            base.AddNodeToEnd(head);
        }

        #endregion // End of Initialization

        #region Heads implementations

        protected override THeadNodeXY CreateDoubleHead(TNode node)
        {
            return new THeadNodeXY {BackLinkX = ((IExtNodeHeadX<TBackX>) node).BackLinkX};
        }

        protected override TNode CreatePrimaryHead()
        {
            return CreateHeadY();
        }

        protected override TNode CreatePrimaryHead(TNode node)
        {
            return CreateHeadY(node);
        }

        protected override TNode CreateSecondaryHead(TNode node)
        {
            return CreateHeadX(node);
        }

        protected override void InitHead(THeadNodeXY head, TNode node)
        {
            head.BackLinkY = ((IExtNodeHeadY<TBackY>) node).BackLinkY;
        }

        protected override void InitHead(TNode head, TNode node)
        {
            ((IExtNodeHeadY<TBackY>) head).BackLinkY = ((IExtNodeHeadY<TBackY>) node).BackLinkY;
        }

        #endregion // End of Heads implementations
    }

    public abstract class ExtendedDataMultiRing2D<TNode, TData, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY> :
        ExtendedMultiRing2D<TNode, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY>
        where TNode : ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>, new()
        where TData : class
        where THeadNodeXY : TNode, IExtNodeHeadX<TBackX>, IExtNodeHeadY<TBackY>, new()
        where THeadNodeX : TNode, IExtNodeHeadX<TBackX>, new()
        where THeadNodeY : TNode, IExtNodeHeadY<TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        protected ExtendedDataMultiRing2D(int dim) : base(dim) {}

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
    }

    public abstract class ExtendedMultiRing2D<TNode, THeadNodeXY, THeadNodeX, THeadNodeY, TBackX, TBackY> :
        MultiRing<TNode>
        where TNode : ExtMultiNode2D<TNode, TBackX, TBackY>, new()
        where THeadNodeXY : TNode, IExtNodeHeadX<TBackX>, IExtNodeHeadY<TBackY>, new()
        where THeadNodeX : TNode, IExtNodeHeadX<TBackX>, new()
        where THeadNodeY : TNode, IExtNodeHeadY<TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        #region Fields

        /// <summary>
        /// the other dimention of the torus.
        /// </summary>
        private readonly int _otherDim;

        #endregion // End of Fields

        #region Initialization

        protected ExtendedMultiRing2D(int dim)
            : base(dim)
        {
            _otherDim = (Dim + 1)%2;
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
                InitAddToEndReLink(node.IsHead(_otherDim)
                    ? CreateDoubleHead(node)
                    : CreatePrimaryHead(), node);
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
                if (node.IsHead(_otherDim))
                {
                    var headXY = CreateDoubleHead(node);
                    if (Size != 0) InitHead(headXY, ReplaceHead());
                    InitInsertReLink(headXY, node);
                }
                else
                {
                    var head = CreatePrimaryHead();
                    if (Size != 0) InitHead(head, ReplaceHead());
                    InitInsertReLink(head, node);
                }
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
                var oldHead = base.ExtractNode(1);
                if (Size == 0) return oldHead;
                TNode node = base.ExtractNode(1);
                InitInsertReLink(CreatePrimaryHead(oldHead), node);
                return oldHead;
            }
            return base.ExtractNode(pos);
        }

        #endregion // End of Node manipulation overrides

        private TNode ReplaceHead()
        {
            var oldHead = base.ExtractNode(1);
            if (oldHead.IsHead(_otherDim))
                InitInsertReLink(CreateSecondaryHead(oldHead), oldHead);
            else base.InsertNode(oldHead.Clone(), 1);
            return oldHead;
        }

        private void InitAddToEndReLink(TNode head, TNode node)
        {
            head.InitFrom(node);
            base.AddNodeToEnd(head);
            ReLink(head);
        }

        private void InitInsertReLink(TNode head, TNode node)
        {
            head.InitFrom(node);
            base.InsertNode(head, 1);
            ReLink(head);
        }

        private static void ReLink(TNode node)
        {
            int i, l = node.Next.Length;

            for (i = 0; i < l; i++)
                if (node.Next[i].Prev[i] != null)
                    node.Next[i].Prev[i] = node;

            for (i = 0; i < l; i++)
                if (node.Prev[i].Next[i] != null)
                    node.Prev[i].Next[i] = node;
        }

        #region Heads abstracts

        protected abstract THeadNodeXY CreateDoubleHead(TNode node);

        protected abstract TNode CreatePrimaryHead();

        protected abstract TNode CreatePrimaryHead(TNode node);

        protected abstract TNode CreateSecondaryHead(TNode node);

        protected abstract void InitHead(THeadNodeXY head, TNode node);

        protected abstract void InitHead(TNode head, TNode node);

        #endregion // End of Heads abstracts

        #region Heads creation

        protected static TNode CreateHeadX()
        {
            return new THeadNodeX();
        }

        protected static TNode CreateHeadX(TNode node)
        {
            return new THeadNodeX {BackLinkX = ((IExtNodeHeadX<TBackX>) node).BackLinkX};
        }

        protected static TNode CreateHeadY()
        {
            return new THeadNodeY();
        }

        protected static TNode CreateHeadY(TNode node)
        {
            return new THeadNodeY {BackLinkY = ((IExtNodeHeadY<TBackY>) node).BackLinkY};
        }

        #endregion End of Heads creation
    }
}

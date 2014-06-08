namespace DataStructures.Rings.Multidimentional.D2
{
    public sealed class ExtDataNodeHeadXY<TNode, TData, TBackX, TBackY> :
        ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>,
        IExtNodeHeadX<TBackX>,
        IExtNodeHeadY<TBackY>
        where TNode : ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>, new()
        where TData : class
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// Container object at the horizontal dimention.
        /// </summary>
        public TBackX BackLinkX { get; set; }

        /// <summary>
        /// Container object at the vertical dimention.
        /// </summary>
        public TBackY BackLinkY { get; set; }

        #region Initialization

        public ExtDataNodeHeadXY() { }

        public ExtDataNodeHeadXY(TBackX backX, TBackY backY)
        {
            BackLinkX = backX;
            BackLinkY = backY;
        }

        public ExtDataNodeHeadXY(TNode node, TBackX backX, TBackY backY)
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

    public sealed class ExtDataNodeHeadX<TNode, TData, TBackX, TBackY> :
        ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>,
        IExtNodeHeadX<TBackX>
        where TNode : ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>, new()
        where TData : class
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// Container object at the horizontal dimention.
        /// </summary>
        public TBackX BackLinkX { get; set; }

        #region Initialization

        public ExtDataNodeHeadX() { }

        public ExtDataNodeHeadX(TBackX back)
        {
            BackLinkX = back;
        }

        public ExtDataNodeHeadX(TNode node, TBackX back)
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

    public sealed class ExtDataNodeHeadY<TNode, TData, TBackX, TBackY> :
        ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>,
        IExtNodeHeadY<TBackY>
        where TNode : ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>, new()
        where TData : class
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// Container object at the vertical dimention.
        /// </summary>
        public TBackY BackLinkY { get; set; }

        #region Initialization

        public ExtDataNodeHeadY() { }

        public ExtDataNodeHeadY(TBackY back)
        {
            BackLinkY = back;
        }

        public ExtDataNodeHeadY(TNode node, TBackY back)
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

    public sealed class ExtNodeHeadXY<TNode, TBackX, TBackY> :
        ExtMultiNode2D<TNode, TBackX, TBackY>,
        IExtNodeHeadX<TBackX>,
        IExtNodeHeadY<TBackY>
        where TNode : ExtMultiNode2D<TNode, TBackX, TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// Container object at the horizontal dimention.
        /// </summary>
        public TBackX BackLinkX { get; set; }

        /// <summary>
        /// Container object at the vertical dimention.
        /// </summary>
        public TBackY BackLinkY { get; set; }

        #region Initialization

        public ExtNodeHeadXY() {}

        public ExtNodeHeadXY(TBackX backX, TBackY backY)
        {
            BackLinkX = backX;
            BackLinkY = backY;
        }

        public ExtNodeHeadXY(TNode node, TBackX backX, TBackY backY)
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

    public sealed class ExtNodeHeadX<TNode, TBackX, TBackY> :
        ExtMultiNode2D<TNode, TBackX, TBackY>,
        IExtNodeHeadX<TBackX>
        where TNode : ExtMultiNode2D<TNode, TBackX, TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// Container object at the horizontal dimention.
        /// </summary>
        public TBackX BackLinkX { get; set; }

        #region Initialization

        public ExtNodeHeadX() {}

        public ExtNodeHeadX(TBackX back)
        {
            BackLinkX = back;
        }

        public ExtNodeHeadX(TNode node, TBackX back)
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

    public sealed class ExtNodeHeadY<TNode, TBackX, TBackY> :
        ExtMultiNode2D<TNode, TBackX, TBackY>,
        IExtNodeHeadY<TBackY>
        where TNode : ExtMultiNode2D<TNode, TBackX, TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// Container object at the vertical dimention.
        /// </summary>
        public TBackY BackLinkY { get; set; }

        #region Initialization

        public ExtNodeHeadY() {}

        public ExtNodeHeadY(TBackY back)
        {
            BackLinkY = back;
        }

        public ExtNodeHeadY(TNode node, TBackY back)
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

    public class ExtDataMultiNode2D<TNode, TData, TBackX, TBackY> :
        ExtMultiNode2D<TNode, TBackX, TBackY>
        where TNode : ExtDataMultiNode2D<TNode, TData, TBackX, TBackY>, new()
        where TData : class
        where TBackX : class
        where TBackY : class
    {
        /// <summary>
        /// The content of the node.
        /// </summary>
        public TData Data { get; set; }

        #region Initialization

        public ExtDataMultiNode2D() {}

        protected ExtDataMultiNode2D(int space) : base(space) {}

        public ExtDataMultiNode2D(int space, TData data)
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

    public class ExtMultiNode2D<TNode, TBackX, TBackY> :
        ExtMultiNode<TNode>
        where TNode : ExtMultiNode2D<TNode, TBackX, TBackY>, new()
        where TBackX : class
        where TBackY : class
    {
        public ExtMultiNode2D() {} 

        public ExtMultiNode2D(int space) : base(space) {}

        /// <summary>
        /// Travels back to the horizontal container object throught the ring.
        /// </summary>
        /// <returns>the container of the ring</returns>
        public TBackX GetContainerX()
        {
            int pos;
            return GetContainerX(out pos);
        }

        /// <summary>
        /// Travels back to the horizontal container object throught the ring.
        /// </summary>
        /// <param name="pos">position of the node in the ring</param>
        /// <returns>the container of the ring</returns>
        public TBackX GetContainerX(out int pos)
        {
            return ((IExtNodeHeadX<TBackX>)GetContainer(1, out pos)).BackLinkX;
        }

        /// <summary>
        /// Travels back to the horizontal container object throught the ring.
        /// </summary>
        /// <returns>the container of the ring</returns>
        public TBackY GetContainerY()
        {
            int pos;
            return GetContainerY(out pos);
        }

        /// <summary>
        /// Travels back to the horizontal container object throught the ring.
        /// </summary>
        /// <param name="pos">position of the node in the ring</param>
        /// <returns>the container of the ring</returns>
        public TBackY GetContainerY(out int pos)
        {
            return ((IExtNodeHeadY<TBackY>)GetContainer(2, out pos)).BackLinkY;
        }
    }

    /// <summary>
    /// implemented by the heads in the first dimention.
    /// </summary>
    /// <typeparam name="TBack">the type of the container in the first dimention.</typeparam>
    public interface IExtNodeHeadX<TBack>
        where TBack : class
    {
        /// <summary>
        /// Container object at the horizontal dimention.
        /// </summary>
        TBack BackLinkX { get; set; }
    }

    /// <summary>
    /// implemented by the heads in the second dimention.
    /// </summary>
    /// <typeparam name="TBack">the type of the container in the second dimention.</typeparam>
    public interface IExtNodeHeadY<TBack>
        where TBack : class
    {
        /// <summary>
        /// Container object at the vertical dimention.
        /// </summary>
        TBack BackLinkY { get; set; }
    }
}

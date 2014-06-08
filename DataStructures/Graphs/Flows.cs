using System;
using System.Collections.Generic;
using System.Xml;
using DataStructures.Rings;

namespace DataStructures.Graphs
{
    /*
    /// <summary>
    /// This class is used to load a list of jobs or machines from an XML file.
    /// </summary>
    public class FlowList
    {
        /// <summary>The XML document.</summary>
        XmlDocument doc;

        /// <summary>The root element.</summary>
        XmlElement root;

        /// <summary>The list of jobs to be solved or of machines flows.</summary>
        List<JobFlow> jobFlows;

        /// <summary>The length of the list.</summary>
        public ushort Count { get { return (ushort)jobFlows.Count; } }

        /// <summary>Initializes from an XML file.</summary>
        /// <param name="xmlFile">The XML file.</param>
        public FlowList(string xmlFile)
        {
            ushort j = 0;

            doc = new XmlDocument();
            doc.Load(xmlFile);
            root = doc.DocumentElement;
            XmlNodeList theXMLList = root.ChildNodes;
            foreach (XmlNode FlowNode in root.ChildNodes)
                if (FlowNode.NodeType == XmlNodeType.Element) j++;
            jobFlows = new List<JobFlow>(j);
            foreach (XmlNode FlowNode in root.ChildNodes)
                if (FlowNode.NodeType == XmlNodeType.Element)
                    jobFlows.Add(new JobFlow((XmlElement)FlowNode));
            root = null;
            doc = null;
        }

        public Capsule<Machine, Material> this[ushort index] { get { return jobFlows[index].GlobalCap; } }
    }
    
    /// <summary>This class can be used for the description of a job,
    /// a job to be solved request or the capabilities of a flow of
    /// machines. (supposes the XML has been validated against the XSD).</summary>
    public class JobFlow
    {
        /// <summary>The global capsule (maximum of lattice).</summary>
        public HeadCapsule<Machine, Material> GlobalCap;

        /// <summary>Material builder delegate.</summary>
        MatDel maT;

        /// <summary>The definition of a material builder.</summary>
        /// <param name="material">The material portion of the xml.</param>
        /// <returns>The constructed material.</returns>
        delegate Material MatDel(XmlElement variables);

        /// <summary>Material builder.</summary>
        /// <param name="parameters">The material's parameters.</param>
        /// <returns>The constructed material.</returns>
        Material MatBuilder(XmlElement parameters) { return new Material(parameters); }

        /// <summary>Material constrains builder.</summary>
        /// <param name="constrains">The material's constrains.</param>
        /// <returns>The constructed material constrains.</returns>
        Material MatConBuilder(XmlElement constrains) { return new MaterialConstraints(constrains); }

        /// <summary>Machine builder delegate.</summary>
        MacDel maC;

        /// <summary>The definition of a machine builder.</summary>
        /// <param name="machine">The machine portion of the xml.</param>
        /// <returns>The constructed machine.</returns>
        delegate Machine MacDel(XmlElement machine);

        /// <summary>Machine builder.</summary>
        /// <param name="parameters">The machine's parameters.</param>
        /// <returns>The constructed machine.</returns>
        Machine MacBuilder(XmlElement parameters) { return new Machine(parameters); }

        /// <summary>Empty machine builder.</summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        Machine EmptyMacBuilder(XmlElement machine)
        {
            Machine temp = new Machine();
            temp.Name = machine.Attributes[0].Value;
            return temp;
        }

        /// <summary>Initialize from XML.</summary>
        /// <param name="portion">Job or MachinesGroup.</param>
        public JobFlow(XmlElement portion)
        {
            Link<Machine, Material> l1;
            Port<Link<Machine, Material>, Material> p1;
            Material m1;
            Capsule<Machine, Material> c1;
            List<int> coo;
            int k;

            // Set the global capsule.
            GlobalCap = new HeadCapsule<Machine, Material>(null, new Machine());
            GlobalCap.Next = GlobalCap;
            GlobalCap.Prev = GlobalCap;
            GlobalCap.Data.Name = portion.Attributes[0].Value;

            // Set the 'function pointer'.
            if (portion.Name == "Job")
            { // Case it's parsing a job.
                maT = MatBuilder;
                maC = EmptyMacBuilder;
            }
            else
            { // Case it's parsing a machine group.
                maT = MatConBuilder;
                maC = MacBuilder;
            }

            // Load the flows of a job or group of machines.
            foreach (XmlNode FlowXML in portion.ChildNodes[0].ChildNodes)
            {
                if (FlowXML.NodeType == XmlNodeType.Element)
                {
                    if (GlobalCap.OutPorts == null)
                        GlobalCap.OutPorts = new ExtendedRing<Link<Machine,Material>, HeadLink<Machine,Material>, Capsule<Machine,Material>>(GlobalCap);
                    else GlobalCap.OutPorts.AddNodeToEnd(new Link<Machine, Material>());
                    l1 = GlobalCap.OutPorts.GetNode(GlobalCap.OutPorts.Size);
                    m1 = BuildFlow((XmlElement)FlowXML, out p1);
                    l1.Down = p1;
                    p1.Up = l1;
                    l1.Up = p1.Down;
                    p1.Down.Down = l1;
                }
            }

            // Load the capsule structure.
            foreach (XmlNode CapXML in portion.ChildNodes[1].ChildNodes)
            {
                if (CapXML.NodeType == XmlNodeType.Element)
                {
                    GlobalCap.AddLastSubcapsule();
                    k = GlobalCap.SubCapsRing.Size;
                    coo = new List<int>(1);
                    coo.Add(k);
                    c1 = GlobalCap.SubCapsRing.GetNode(k);
                    c1.Data = new Machine();
                    c1.Data.Name = CapXML.Attributes[0].Value;
                    BuildCap((XmlElement)CapXML, c1, coo);
                }
            }
        }

        /// <summary>Recursive function that builds the flow from XML portions.</summary>
        /// <param name="mainFlowXML">The flow XML.</param>
        /// <param name="toConnect">The flow output.</param>
        /// <returns>The output material of the flow.</returns>
        Material BuildFlow(XmlElement mainFlowXML, out Port<Link<Machine, Material>, Material> toConnect)
        {
            Capsule<Machine, Material> c1;
            Port<Link<Machine, Material>, Material> p1;
            Material m1;

            GlobalCap.AddLastSubcapsule();
            c1 = GlobalCap.SubCapsRing.GetNode(GlobalCap.SubCapsRing.Size);
            m1 = maT((XmlElement)mainFlowXML.FirstChild);
            //c1.AddLastPort(ref c1.OutPorts, m1);
            toConnect = (Port<Link<Machine, Material>, Material>)c1.OutPorts.GetNode(c1.OutPorts.Size).Up;
            c1.Data = maC((XmlElement)mainFlowXML.ChildNodes[1]);
            if (mainFlowXML.ChildNodes.Count == 3)
            {
                foreach (XmlNode subFlowXML in mainFlowXML.ChildNodes[2].ChildNodes)
                {
                    //c1.AddLastPort(ref c1.InPorts, BuildFlow((XmlElement)subFlowXML, out p1));
                    p1 = null; p1.ConnectTo((Port<Link<Machine, Material>, Material>)c1.InPorts.GetNode(c1.InPorts.Size).Up);
                }
            }
            return m1;
        }

        /// <summary>Recursive function that builds the capsule structure from XML portions.</summary>
        /// <param name="mainCapXML">The capsule XML.</param>
        /// <param name="cap">The super capsule.</param>
        /// <param name="coo">The coordinates of the capsule.</param>
        void BuildCap(XmlElement mainCapXML, Capsule<Machine, Material> cap, List<int> coo)
        {
            int i;
            Capsule<Machine, Material> c1;
            List<int> Coords;

            foreach (XmlNode subCapXML in mainCapXML.ChildNodes)
            {
                if (subCapXML.NodeType == XmlNodeType.Element)
                {
                    if (subCapXML.Name == "Capsule")
                    {// Case it's parsing a capsule:
                        cap.AddLastSubcapsule();
                        i = cap.SubCapsRing.Size;
                        coo.Add((ushort)i);
                        c1 = cap.SubCapsRing.GetNode((ushort)i);
                        c1.Data = new Machine();
                        c1.Data.Name = subCapXML.Attributes[0].Value;
                        BuildCap((XmlElement)subCapXML, c1, coo);
                    }
                    else
                    {// Case it's parsing a machine:
                        if (GlobalCap.GetCoordinates(subCapXML.Attributes[0].Value, out Coords))
                        {
                            GlobalCap.Encapsulate(Coords[0], coo[0]);
                            coo[0] = (ushort)(coo[0] - 1);
                            c1 = GlobalCap.SubCapsRing.GetNode(coo[0]);
                            for (i = 1; i < coo.Count; i++)
                            {
                                c1.Encapsulate(c1.SubCapsRing.Size, coo[i]);
                                coo[i] = (ushort)(coo[i] - 1);
                                c1 = c1.SubCapsRing.GetNode(coo[i]);
                            }
                        }
                    }
                }
            }
        }
    }
    
    public class Machine
    {
        string _Name;
        /// <summary>The name of the machine.</summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        float InnerProcTime;
        /// <summary>The inner processing time.</summary>
        public float Time
        {
            get { return InnerProcTime; }
            set { InnerProcTime = value; }
        }

        /// <summary>The properties of the machine.</summary>
        protected int[] IntVars1;

        /// <summary>Gets a property of the machine.</summary>
        /// <param name="a">Property index</param>
        /// <returns>The property value.</returns>
        public int Get1(int a) { return IntVars1[a]; }

        /// <summary>Sets a property of the machine.</summary>
        /// <param name="a">Property index</param>
        /// <param name="v">The property value.</param>
        public void Set1(int a, int v) { IntVars1[a] = v; }

        /// <summary>The properties' list size.</summary>
        /// <returns>The list's length.</returns>
        public ushort PropListSize()
        {
            if (IntVars1 != null) return (ushort)IntVars1.Length;
            else return 0;
        }

        /// <summary>Format the machine's property values to string lines.</summary>
        public string[] FormatProperties
        {
            get
            {
                ushort i, k;
                string[] info;

                k = (ushort)(PropListSize() + 2);
                info = new string[k];
                info[0] = "Name:\t" + _Name;
                info[1] = "I.P.Time:\t" + InnerProcTime.ToString();
                for (i = 2; i < k; i++) info[i] = i - 1 + ")\t" + IntVars1[i - 2].ToString();
                return info;
            }
        }

        public Machine() { }

        /// <summary>Construct a machine instance from an XML element.</summary>
        /// <param name="elem">The XML element.</param>
        public Machine(XmlElement elem)
        {
            ushort i = 0, j = 0;

            _Name = elem.Attributes[0].Value;
            InnerProcTime = Convert.ToSingle(elem["IPT"].InnerText);
            XmlNodeList PropList = elem.GetElementsByTagName("Property");
            j = (ushort)PropList.Count;
            if (j != 0)
            {
                IntVars1 = new int[j];
                for (i = 0; i < j; i++) IntVars1[i] = Convert.ToInt32(PropList[i].InnerText);
            }
        }
    }

    public class Material
    {
        protected string _Name;
        /// <summary>The name of the type of the material.</summary>
        public string Name { get { return _Name; } }

        protected float _Rate1;
        /// <summary>The transfer rate of the material through the
        /// corresponting port or the minimum rate of a machine.</summary>
        public float Rate1
        {
            get { return _Rate1; }
            set { _Rate1 = value; }
        }

        /// <summary>The properties of the material or the minimum constraints.</summary>
        protected int[] IntVars1;

        /// <summary>Gets a property of the material.</summary>
        /// <param name="a">Property index</param>
        /// <returns>The property value.</returns>
        public int Get1(int a) { return IntVars1[a]; }

        /// <summary>Sets a property of the material.</summary>
        /// <param name="a">Property index</param>
        /// <param name="v">The property value.</param>
        public void Set1(int a, int v) { IntVars1[a] = v; }

        /// <summary>The properties' list size.</summary>
        /// <returns>The list's length.</returns>
        public ushort PropListSize()
        {
            if (IntVars1 != null) return (ushort)IntVars1.Length;
            else return 0;
        }

        /// <summary>Format the material's property values to string lines.</summary>
        public virtual string[] FormatProperties
        {
            get
            {
                ushort i, k;
                string[] info;

                k = (ushort)(PropListSize() + 2);
                info = new string[k];
                info[0] = "Name:\t" + _Name;
                info[1] = "Rate:\t" + _Rate1.ToString();
                for (i = 2; i < k; i++) info[i] = i - 1 + ")\t" + IntVars1[i - 2].ToString();
                return info;
            }
        }

        public Material() { }

        /// <summary>Construct specific material.</summary>
        /// <param name="name">The name of the material.</param>
        /// <param name="rate">The rate value.</param>
        /// <param name="info">The parameters.</param>
        public Material(string name, float rate, int[] info)
        {
            _Name = name;
            IntVars1 = new int[info.Length];
            info.CopyTo(IntVars1, 0);
            _Rate1 = rate;
        }

        /// <summary>Construct a material instance from an XML element.</summary>
        /// <param name="elem">The XML element.</param>
        public Material(XmlElement elem)
        {
            ushort i = 0, j = 0;

            _Name = elem.Attributes[0].Value;
            _Rate1 = Convert.ToSingle(elem["Rate"].InnerText);
            XmlNodeList PropList = elem.GetElementsByTagName("Property");
            j = (ushort)PropList.Count;
            if (j != 0)
            {
                IntVars1 = new int[j];
                for (i = 0; i < j; i++) IntVars1[i] = Convert.ToInt32(PropList[i].InnerText);
            }
        }
    }

    class MaterialConstraints : Material
    {
        // The maximum transfer rate of the material through the corresponting port.
        float _Rate2;
        public float Rate2
        {
            get { return _Rate2; }
            set { _Rate2 = value; }
        }

        // The maximum constraints on the properties of the material.
        int[] IntVars2;
        public int Get2(int a) { return IntVars2[a]; }
        public void Set2(int a, int v) { IntVars2[a] = v; }

        /// <summary>The material's property constraints values to string lines.</summary>
        public override string[] FormatProperties
        {
            get
            {
                ushort i, k;
                string[] info;

                k = (ushort)(PropListSize() + 2);
                info = new string[k];
                info[0] = "Name:\t" + _Name;
                info[1] = "Rate:\t" + _Rate1.ToString() + "\t" + _Rate2.ToString();
                for (i = 2; i < k; i++) info[i] = i - 1 + ")\t" + IntVars1[i - 2].ToString() + "\t" + IntVars2[i - 2].ToString();
                return info;
            }
        }

        public MaterialConstraints() { }

        /// <summary>Construct a holder for the material constraints 
        /// (the two arrays are supposed to have the same size).</summary>
        /// <param name="materialName"></param>
        /// <param name="rateMin"></param>
        /// <param name="rateMax"></param>
        /// <param name="infoMin"></param>
        /// <param name="infoMax"></param>
        public MaterialConstraints(string materialName, float rateMin, float rateMax, int[] infoMin, int[] infoMax)
        {
            int ParamListLen = infoMin.Length;

            _Name = materialName;
            IntVars1 = new int[ParamListLen];
            infoMin.CopyTo(IntVars1, 0);
            _Rate1 = rateMin;
            IntVars2 = new int[ParamListLen];
            infoMax.CopyTo(IntVars2, 0);
            _Rate2 = rateMax;
        }

        /// <summary>Construct a material constraints holder instance from an XML element.</summary>
        /// <param name="elem">The XML element.</param>
        public MaterialConstraints(XmlElement elem)
        {
            ushort i = 0, j = 0;

            _Name = elem.Attributes[0].Value;
            _Rate1 = Convert.ToSingle(elem["Rate"]["Min"].InnerText);
            _Rate2 = Convert.ToSingle(elem["Rate"]["Max"].InnerText);
            XmlNodeList PropList = elem.GetElementsByTagName("Property");
            j = (ushort)PropList.Count;
            if (j != 0)
            {
                IntVars1 = new int[j];
                IntVars2 = new int[j];
                for (i = 0; i < j; i++)
                {
                    IntVars1[i] = Convert.ToInt32(PropList[i]["Min"].InnerText);
                    IntVars2[i] = Convert.ToInt32(PropList[i]["Max"].InnerText);
                }
            }
        }
    }
    */
}

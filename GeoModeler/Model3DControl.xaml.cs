using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace GeoModeler
{
    /// <summary>
    /// Interaction logic for Project3DModel.xaml
    /// </summary>
    public partial class Model3DControl : UserControl
    {
        public Model3DControl()
        {
            InitializeComponent();
            cameraControl.Camera = (PerspectiveCamera)projectView.Camera;
        }
    }
}
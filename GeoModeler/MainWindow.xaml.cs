using System.Windows;
using System.Windows.Media.Media3D;

namespace GeoModeler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            projectControl.InitFromFile("C:\\DATA_Local\\Test\\triangles.txt");
            projectControl.Show3DModel();
        }
    }
}
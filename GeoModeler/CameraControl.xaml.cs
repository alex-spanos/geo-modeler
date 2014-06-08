using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace GeoModeler
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : UserControl
    {
        public PerspectiveCamera Camera { get; set; }

        #region Constants

        #region Arithmetic

        const double halfPI = Math.PI / 2,
                     twoPI = 2 * Math.PI;
        #endregion

        #region Steps

        const double moveStep = 10,
                     rotateStep = 1 / twoPI,
                     zoomStep = 10 / twoPI;
        #endregion

        #region Move vectors

        Vector3D For = new Vector3D(moveStep, 0, 0),
                 Back = new Vector3D(-moveStep, 0, 0),
                 Up = new Vector3D(0, 0, moveStep),
                 Down = new Vector3D(0, 0, -moveStep),
                 Left = new Vector3D(0, moveStep, 0),
                 Right = new Vector3D(0, -moveStep, 0);
        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// The angle between the x+ axis and the xy projection of the look direction.
        /// </summary>
        double Theta;

        /// <summary>
        /// The angle between the x+ axis and the xz projection of the look direction.
        /// </summary>
        double Phi;

        /// <summary>
        /// True for absolute navigation, false for relative.
        /// </summary>
        bool NavigationMode;

        #endregion

        public CameraControl()
        {
            InitializeComponent();
            NavigationMode = true;
        }

        #region Buttons

        #region Rotate look direction

        private void rotateUpButton_Click(object sender, RoutedEventArgs e) { RotateBy(0, rotateStep); }

        private void rotateDownButton_Click(object sender, RoutedEventArgs e) { RotateBy(0, -rotateStep); }

        private void rotateLeftButton_Click(object sender, RoutedEventArgs e) { RotateBy(rotateStep, 0); }

        private void rotateRightButton_Click(object sender, RoutedEventArgs e) { RotateBy(-rotateStep, 0); }

        #endregion

        #region Field of view

        private void widenButton_Click(object sender, RoutedEventArgs e) { Camera.FieldOfView += zoomStep; }

        private void narrowButton_Click(object sender, RoutedEventArgs e) { Camera.FieldOfView -= zoomStep; }

        #endregion

        #region Move position

        private void moveForButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationMode) MoveBy(For);
            else MoveBy(moveStep * Camera.LookDirection);
        }

        private void moveBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationMode) MoveBy(Back);
            else MoveBy(-moveStep * Camera.LookDirection);
        }

        private void moveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationMode) MoveBy(Left);
            else MoveBy(Vector3D.CrossProduct(Up, Camera.LookDirection));
        }

        private void moveRightButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationMode) MoveBy(Right);
            else MoveBy(Vector3D.CrossProduct(Camera.LookDirection, Up));
        }

        private void moveUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationMode) MoveBy(Up);
            else MoveBy(Vector3D.CrossProduct(Vector3D.CrossProduct(Camera.LookDirection, Up), Camera.LookDirection));
        }

        private void moveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationMode) MoveBy(Down);
            else MoveBy(Vector3D.CrossProduct(Camera.LookDirection, Vector3D.CrossProduct(Camera.LookDirection, Up)));
        }

        #endregion

        private void swapNavButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationMode = !NavigationMode;
            if (NavigationMode)
            {
                swapNavButton.Content = "R";
                moveForButton.Content = "x+";
                moveBackButton.Content = "x-";
                moveLeftButton.Content = "y+";
                moveRightButton.Content = "y-";
                moveUpButton.Content = "z+";
                moveDownButton.Content = "z-";
            }
            else
            {
                swapNavButton.Content = "A";
                moveForButton.Content = "f";
                moveBackButton.Content = "b";
                moveRightButton.Content = "r";
                moveLeftButton.Content = "l";
                moveUpButton.Content = "u";
                moveDownButton.Content = "d";
            }
        }

        #endregion

        #region Move methods

        public void MoveTo(Point3D position)
        { Camera.Position = position; }

        public void MoveTo(double X, double Y, double Z)
        { Camera.Position = new Point3D(X, Y, Z); }

        public void MoveBy(Vector3D displacement)
        { MoveTo(Camera.Position + displacement); }

        public void MoveBy(double dX, double dY, double dZ)
        { MoveTo(Camera.Position.X + dX, Camera.Position.Y + dY, Camera.Position.Z + dZ); }

        #endregion

        #region Rotate methods

        public void RotateBy(double dTheta, double dPhi)
        { RotateTo(Theta + dTheta, Phi + dPhi); }

        public void RotateTo(Vector3D direction)
        {
            double horizontalDis = Math.Sqrt(Math.Pow(direction.X, 2) + Math.Pow(direction.Y, 2)),
                   verticalDis = Math.Sqrt(Math.Pow(direction.X, 2) + Math.Pow(direction.Z, 2));

            RotateTo(Math.Cosh(direction.X / (horizontalDis * twoPI)),
                     Math.Sinh(direction.Z / (verticalDis * twoPI)));
        }

        public void RotateTo(double theta, double phi)
        {
            Theta = theta % twoPI;
            if (phi < -halfPI) Phi = -halfPI;
            else if (phi > halfPI) Phi = halfPI;
            else Phi = phi;
            Camera.LookDirection = new Vector3D(Math.Cos(Theta), Math.Sin(Theta), Math.Sin(Phi));
        }

        #endregion
    }
}
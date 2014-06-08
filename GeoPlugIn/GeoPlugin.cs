using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(GeoPlugIn.GeoPlugin))]

namespace GeoPlugIn
{
    public class GeoPlugin :
        IExtensionApplication
    {
        GeoServiceController _geoServiceController;

        void IExtensionApplication.Initialize()
        {
            _geoServiceController = new GeoServiceController();
            _geoServiceController.LaunchService();
        }

        void IExtensionApplication.Terminate()
        {
            if (_geoServiceController.ServiceRunning)
                _geoServiceController.ShutdownService();
        }
    }
}

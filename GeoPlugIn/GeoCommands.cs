using System;
using System.ServiceModel;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using GeoClients;
using GeoClients.GeoServiceReference;

[assembly: CommandClass(typeof(GeoPlugIn.geoCommands))]

namespace GeoPlugIn
{
    public class geoCommands
    {
        static readonly GeoClient<GeoServiceCallbackHandler> GeoClient;
        readonly DuplexChannelFactory<IGeoServiceChannel> _factory;

        static geoCommands()
        {
            GeoClient = new GeoClient<GeoServiceCallbackHandler>();
        }

        public geoCommands()
        {
            _factory = GeoClient.GetFactory();
        }

        [CommandMethod("GeoGroup", "TRIANGULATE", "Triangulate", CommandFlags.Modal | CommandFlags.UsePickSet)]
        public void Triangulate()
        {
            PromptSelectionResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetSelection();
            if (result.Status == PromptStatus.OK)
            {
                IGeoServiceChannel channel = _factory.CreateChannel();
                channel.Calculate(new GeoTicketIn { TicketId = 1, FlowId = 1, MemFileName = "" });
            }
        }
    }
}

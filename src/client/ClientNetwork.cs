
namespace WaypointTogetherContinued
{
    using System.Linq;
    using System.Reflection;
    using Vintagestory.API.Client;
    using Vintagestory.GameContent;
    public class ClientNetwork
    {
        private readonly ICoreClientAPI api;
        private readonly IClientNetworkChannel channel;

        public ClientNetwork(ICoreClientAPI api)
        {
            this.api = api;

            channel = api.Network.RegisterChannel("malin.waypointtogethercontinued");
            channel.RegisterMessageType<UserToServerShareWaypointMessage>();
            channel.RegisterMessageType<ServerToUserShareWaypointMessage>();
            channel.SetMessageHandler<ServerToUserShareWaypointMessage>(this.HandlePacket);
        }


        public void ShareWaypoint(Waypoint wp)
        {
            channel.SendPacket(new UserToServerShareWaypointMessage() { Waypoint = wp });
        }

        private static readonly MethodInfo RebuildMapComponentsMethod = typeof(WaypointMapLayer).GetMethod("RebuildMapComponents", BindingFlags.NonPublic | BindingFlags.Instance);


        private void HandlePacket(ServerToUserShareWaypointMessage packet)
        {
            var mapManager = api.ModLoader.GetModSystem<WorldMapManager>();
            var maplayers = mapManager.MapLayers;

            var waypointLayer = (maplayers.Find(x => x is WaypointMapLayer) as WaypointMapLayer);
            if (waypointLayer == null) return;
            waypointLayer.ownWaypoints.Add(packet.Waypoint);


            GuiDialogWorldMap mapDialog = api.Gui.OpenedGuis.FirstOrDefault(d => d is GuiDialogWorldMap) as GuiDialogWorldMap;
            if (mapDialog != null)
            {
                mapDialog.Recompose();
                // trigger RebuildMapComponents
                RebuildMapComponentsMethod.Invoke(waypointLayer, null);

            }

        }
    }
}

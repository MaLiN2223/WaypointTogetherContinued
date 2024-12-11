using Vintagestory.API.Server;

namespace WaypointTogetherContinued;

public class ServerNetwork
{
    private readonly IServerNetworkChannel channel;

    public ServerNetwork(ICoreServerAPI api)
    {
        channel = api.Network.RegisterChannel("malin.waypointtogethercontinued");
        channel.RegisterMessageType<ShareWaypointPacket>();
        channel.SetMessageHandler<ShareWaypointPacket>(this.HandlePacket);
    }

    public void ShareWaypoint(string message)
    {
        if (message != null && message != "")
        {
            channel.SendPacket(new ShareWaypointPacket(message));
        }
    }

    private void HandlePacket(IServerPlayer player, ShareWaypointPacket packet)
    {
        channel.BroadcastPacket(packet, player);
    }
}

using Vintagestory.API.Client;

namespace WaypointTogetherContinued;

public class ClientNetwork
{
    private readonly ICoreClientAPI api;
    private readonly IClientNetworkChannel channel;

    string lastMessage = "";

    public ClientNetwork(ICoreClientAPI api)
    {
        this.api = api;

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

    private void HandlePacket(ShareWaypointPacket packet)
    {
        if (lastMessage == packet.Message)
        {
            return;
        }

        api.SendChatMessage(packet.Message);
        lastMessage = packet.Message;
    }
}

using System.Net.Sockets;
using System.Text;

using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using WaypointTogetherContinued.Network.Packets;
namespace WaypointTogetherContinued.Client;
/// <summary>
/// A <see langword="class" /> for handling <see cref="ClientNetwork" /> packets.
/// </summary>
public class ClientNetwork
{
    /// <summary>
    /// Gets the instance of the mod's <see cref="IClientNetworkChannel" />.
    /// </summary>
    private IClientNetworkChannel Channel { get; }

    /// <summary>
    /// Gets the instance of the game's <see cref="ICoreClientAPI" />.
    /// </summary>
    private ICoreClientAPI API { get; }

    /// <summary>
    /// Stores an instance of the last message that was received by the server.
    /// </summary>
    private string lastMessage = "";
  
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientNetwork" /> class.
    /// </summary>
    /// <param name="api">An instance of the game's <see cref="ICoreClientAPI" />.</param>
    public ClientNetwork(ICoreClientAPI api)
    {
        this.API = api;

        this.Channel = api.Network.RegisterChannel("malin.waypointtogethercontinued")
          .RegisterMessageType<ShareWayPointPacket>()
          .SetMessageHandler<ShareWayPointPacket>(this.HandlePacket);
    }

    /// <summary>
    /// A method to share the given <see cref="Waypoint" /> with other clients.
    /// </summary>
    /// <param name="message">The message that the client is sending to the server.</param>
    /// <param name="wayPoint">The instance of the <see cref="Waypoint" /> that was created or modified.</param>
    public void ShareWayPoint(string? message, Waypoint? wayPoint)
    {
        if (!string.IsNullOrEmpty(message))
        {
          this.Channel.SendPacket(new ShareWayPointPacket(message, wayPoint));
        }
    }

    /// <summary>
    /// Handles the packets for the client.
    /// </summary>
    /// <param name="packet">The data information of the packet.</param>
    private void HandlePacket(ShareWayPointPacket packet)
    {
        if (lastMessage == packet.Message)
        {
            return;
        }

        var currentPlayer = this.API.World.Player;
        if (packet.Message.StartsWith("/waypoint modify"))
        {
            // Modify currently is in format of /waypoint modify <id> <color> <icon> <pinned> <name>
            string[] split = packet.Message.Split(' ');
            string id = split[2];
            string color = split[3];
            string icon = split[4];
            string pinned = split[5];
            string name = split[6];

            var maplayers = this.API.ModLoader.GetModSystem<WorldMapManager>().MapLayers;
            var waypointLayer = maplayers.Find(x => x is WaypointMapLayer) as WaypointMapLayer;
            Waypoint? existing = packet.WayPoint;
            if (existing is not null) {
                int myExistingId = -1;
                if (waypointLayer?.ownWaypoints is not null)
                {
                    myExistingId = waypointLayer.ownWaypoints.FindIndex(x => x.Position.X == existing?.Position.X && x.Position.Y == existing.Position.Y && x.Position.Z == existing.Position.Z);
                }
                if (myExistingId != -1)
                {
                    // send modify, just replace id
                    string message = "/waypoint modify " + myExistingId + " " + existing.Color + " " + icon + " " + pinned + " " + name;
                    this.API.SendChatMessage(message);
                }
                else
                {
                    double x = existing.Position.X - (this.API.World.BlockAccessor.MapSizeX / 2);
                    double y = existing.Position.Y - this.API.World.MapSizeY;
                    double z = existing.Position.Z - (this.API.World.BlockAccessor.MapSizeZ / 2);
                    // we want /waypoint addati [icon] [x] [y] [z] [pinned] [color] [title]
                    string message = $"/waypoint addati {icon} {x} {y} {z} {pinned} {color} {name}";
                    this.API.SendChatMessage(message);
                }
            }
        }
        else
        {
            this.API.SendChatMessage(packet.Message);
        }
        lastMessage = packet.Message;
    }
}

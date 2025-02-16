using System.Linq;
using Vintagestory.API.Client;
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
            string name = string.Join(' ', split.Skip(6));

            var maplayers = this.API.ModLoader.GetModSystem<WorldMapManager>().MapLayers;
            var waypointLayer = maplayers.Find(x => x is WaypointMapLayer) as WaypointMapLayer;
            Waypoint? packetWayPoint = packet.WayPoint;
            if (packetWayPoint is not null) {
                int myExistingId = -1;
                if (waypointLayer?.ownWaypoints is not null)
                {
                    myExistingId = waypointLayer.ownWaypoints.FindIndex((Waypoint existingWayPoint) => 
                      existingWayPoint.Position.X == packetWayPoint?.Position.X
                      && existingWayPoint.Position.Y == packetWayPoint.Position.Y
                      && existingWayPoint.Position.Z == packetWayPoint.Position.Z
                    );
                }
                if (myExistingId != -1)
                {
                    // Send modify, just replace id
                    this.API.SendChatMessage($"/waypoint modify {myExistingId} {color} {icon} {pinned} {name}");
                }
                else
                {
                    double x = packetWayPoint.Position.X - (this.API.World.BlockAccessor.MapSizeX / 2);
                    double y = packetWayPoint.Position.Y - this.API.World.MapSizeY;
                    double z = packetWayPoint.Position.Z - (this.API.World.BlockAccessor.MapSizeZ / 2);
                    // We want /waypoint addati [icon] [existingWayPoint] [y] [z] [pinned] [color] [name]
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

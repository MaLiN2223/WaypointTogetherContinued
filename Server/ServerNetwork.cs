using Vintagestory.API.Server;
using Vintagestory.GameContent;
using WaypointTogetherContinued.Network.Packets;
namespace WaypointTogetherContinued.Server;
/// <summary>
/// A <see langword="class" /> for handling <see cref="ServerNetwork" /> packets.
/// </summary>
public class ServerNetwork
{
    /// <summary>
    /// Gets the instance of the mod's <see cref="IServerNetworkChannel" />.
    /// </summary>
    private IServerNetworkChannel Channel { get; }

    /// <summary>
    /// Gets the instance of the game's <see cref="ICoreServerAPI" />.
    /// </summary>
    private ICoreServerAPI API { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerNetwork" /> class.
    /// </summary>
    /// <param name="api">An instance of the game's <see cref="ICoreServerAPI" />.</param>
    public ServerNetwork(ICoreServerAPI api)
    {
        this.API = api;
        Channel = this.API.Network.RegisterChannel("malin.waypointtogethercontinued");
        Channel.RegisterMessageType<ShareWayPointPacket>();
        Channel.SetMessageHandler<ShareWayPointPacket>(this.HandlePacket);
    }

    /// <summary>
    /// Handles the packets for the server.
    /// </summary>
    /// <param name="player">The instance of the player that sent the packet.</param>
    /// <param name="packet">The data information of the packet.</param>
    private void HandlePacket(IServerPlayer player, ShareWayPointPacket packet)
    {
        API.Logger.Event($"Player {player.PlayerName} shared way point {packet.WayPoint?.Guid ?? "null"} with message \"{packet.Message}\"");
        var maplayers = API.ModLoader.GetModSystem<WorldMapManager>().MapLayers;
        var waypointLayer = maplayers.Find(x => x is WaypointMapLayer) as WaypointMapLayer;
        Waypoint? existing = waypointLayer?.Waypoints.Find(x => x.Guid == packet.WayPoint?.Guid);
        API.Logger.Event($"{(existing is not null ? "Found" : "Did not find a")} way point on the server with the shared way point GUID.");
        var newPacket = new ShareWayPointPacket(packet.Message, packet.WayPoint);
        Channel.BroadcastPacket(newPacket, player);
    }
}

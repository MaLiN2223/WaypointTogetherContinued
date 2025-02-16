using ProtoBuf;
using Vintagestory.GameContent;
namespace WaypointTogetherContinued.Network.Packets;
/// <summary>
/// A <see langword="class" /> for sharing the <see langword="Waypoint" /> information.
/// </summary>
[ProtoContract]
internal class ShareWayPointPacket
{
    /// <summary>
    /// Gets the message that was sent with the packet.
    /// </summary>
    [ProtoMember(1)]
    public string Message { get; }

    /// <summary>
    /// Gets the instance of the <see cref="Waypoint" /> that was sent with the packet.
    /// </summary>
    [ProtoMember(2)]
    public Waypoint? WayPoint { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShareWayPointPacket" /> class.
    /// </summary>
    public ShareWayPointPacket() {
      Message = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShareWayPointPacket" /> class.
    /// </summary>
    /// <param name="message">The message that was sent with the packet.</param>
    /// <param name="wayPoint">The instance of the <see cref="Waypoint" /> that was sent with the packet.</param>
    public ShareWayPointPacket(string? message, Waypoint? wayPoint)
    {
        this.Message = message ?? string.Empty;
        this.WayPoint = wayPoint;
    }
}

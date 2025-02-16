using ProtoBuf;
using Vintagestory.GameContent;

[ProtoContract]
class ShareWaypointPacket
{
    [ProtoMember(1)]
    public string Message { get; set; }

    [ProtoMember(2)]
    public Waypoint Waypoint { get; set; }

    public ShareWaypointPacket()
    {
        Message = "";
        Waypoint = null;
    }

    public ShareWaypointPacket(string message, Waypoint waypointGuid)
    {
        Message = message;
        Waypoint = waypointGuid;
    }
}

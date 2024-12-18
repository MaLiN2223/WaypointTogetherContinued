using ProtoBuf;

[ProtoContract]
class ShareWaypointPacket
{
    [ProtoMember(1)]
    public string Message { get; set; }

    [ProtoMember(2)]
    public string WaypointGuid { get; set; }

    public ShareWaypointPacket()
    {
        Message = "";
        WaypointGuid = "";
    }

    public ShareWaypointPacket(string message, string waypointGuid)
    {
        Message = message;
        WaypointGuid = waypointGuid;
    }
}

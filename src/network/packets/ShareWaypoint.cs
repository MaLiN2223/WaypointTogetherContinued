using ProtoBuf;

[ProtoContract]
class ShareWaypointPacket
{
    [ProtoMember(1)]
    public string Message { get; set; }

    public ShareWaypointPacket()
    {
        Message = "";
    }

    public ShareWaypointPacket(string message)
    {
        Message = message;
    }
}

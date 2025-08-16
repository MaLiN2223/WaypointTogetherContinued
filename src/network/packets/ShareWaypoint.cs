using ProtoBuf;
using Vintagestory.GameContent;


public enum CommandType
{
    Add = 1,
    Remove = 2,
    Modify = 3
}

[ProtoContract]
public class UserToServerShareWaypointMessage
{
    [ProtoMember(1)]
    public required Waypoint Waypoint { get; set; }
}

[ProtoContract]
public class ServerToUserShareWaypointMessage
{
    [ProtoMember(1)]
    public required Waypoint Waypoint { get; set; }
}
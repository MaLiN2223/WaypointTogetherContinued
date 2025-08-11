using HarmonyLib;
using ProtoBuf;
using System;
using System.Reflection;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace WaypointTogetherContinued;

public class ServerNetwork
{
    private readonly IServerNetworkChannel channel;
    private readonly ICoreServerAPI api;

    public ServerNetwork(ICoreServerAPI api)
    {
        channel = api.Network.RegisterChannel("malin.waypointtogethercontinued");
        channel.RegisterMessageType<ShareWaypointPacket>();
        channel.RegisterMessageType<ShareWaypointPacketFromServer>();
        channel.SetMessageHandler<ShareWaypointPacket>(this.HandlePacket);
        this.api = api;
    }

    public void ShareWaypoint(string message, string byPlayer)
    {
        if (message != null && message != "")
        {
            channel.SendPacket(new ShareWaypointPacket(message, byPlayer));
        }
    }

    private void HandlePacket(IServerPlayer player, ShareWaypointPacket packet)
    {
        var maplayers = api.ModLoader.GetModSystem<WorldMapManager>().MapLayers;
        var waypointLayer = maplayers.Find(x => x is WaypointMapLayer);
        if (waypointLayer is WaypointMapLayer wml)
        {
            var existing = wml.Waypoints.Find(x => x.Guid == packet.WaypointGuid);
            if (existing is not null)
            {
                var newPacket = new ShareWaypointPacketFromServer
                {
                    Message = packet.Message,
                    ExistingWaypoint = existing,
                };
                channel.BroadcastPacket(newPacket, player);
            }
        }
    }
}

[ProtoContract]
public class ShareWaypointPacketFromServer
{
    [ProtoMember(1)]
    public required string Message { get; set; }

    [ProtoMember(2)]
    public required Waypoint ExistingWaypoint { get; set; }
}


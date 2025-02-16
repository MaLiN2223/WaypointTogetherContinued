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
        channel.SetMessageHandler<ShareWaypointPacket>(this.HandlePacket);
        this.api = api;
    }

    private void HandlePacket(IServerPlayer player, ShareWaypointPacket packet)
    {
        channel.BroadcastPacket(packet, player);
    }
}


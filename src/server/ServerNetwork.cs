using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
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
        channel.RegisterMessageType<UserToServerShareWaypointMessage>();
        channel.RegisterMessageType<ServerToUserShareWaypointMessage>();
        channel.SetMessageHandler<UserToServerShareWaypointMessage>(this.HandlePacket);
        this.api = api;
    }

    private void HandlePacket(IServerPlayer player, UserToServerShareWaypointMessage packet)
    {
        var mapManager = api.ModLoader.GetModSystem<WorldMapManager>();
        var maplayers = mapManager.MapLayers;
        var waypointLayer = maplayers.Find(x => x is WaypointMapLayer);
        if (waypointLayer is WaypointMapLayer wml)
        {
            var sharedWaypoint = packet.Waypoint;
            var allWaypoints = wml.Waypoints;
            foreach (var recipientPlayer in api.World.AllOnlinePlayers)
            {
                if (recipientPlayer.PlayerUID == player.PlayerUID) continue;
                this.ShareWaypointWithPlayer(player, wml, sharedWaypoint, recipientPlayer);
            }
        }
    }

    private void ShareWaypointWithPlayer(IServerPlayer fromPlayer, WaypointMapLayer wml, Waypoint sharedWaypoint, IPlayer recipientPlayer)
    {
        Waypoint waypointCopy = new Waypoint()
        {
            Guid = Guid.NewGuid().ToString(),
            OwningPlayerUid = recipientPlayer.PlayerUID,
            Position = sharedWaypoint.Position,
            Color = sharedWaypoint.Color,
            Icon = sharedWaypoint.Icon,
            Title = sharedWaypoint.Title,
            Pinned = sharedWaypoint.Pinned,
            Temporary = sharedWaypoint.Temporary
        };

        bool alreadyExists = wml.Waypoints.Any(wp => wp.OwningPlayerUid == recipientPlayer.PlayerUID && wp.Position.AsBlockPos.Equals(sharedWaypoint.Position.AsBlockPos));
        if (alreadyExists)
        {
            return;
        }
        api.SendMessage(recipientPlayer, GlobalConstants.GeneralChatGroup, "Waypoint received from " + fromPlayer.PlayerName, EnumChatType.OwnMessage);


        wml.AddWaypoint(waypointCopy, (IServerPlayer)recipientPlayer);
        api.World.Logger.Notification($"Shared waypoint '{waypointCopy.Title}' from {fromPlayer.PlayerName} with {recipientPlayer.PlayerName}.");
        api.World.Logger.Notification($"New waypoint has the following color: {waypointCopy.Color}, taken from list: {wml.Waypoints.Last().Color}");
        channel.SendPacket(new ServerToUserShareWaypointMessage { Waypoint = waypointCopy }, (IServerPlayer)recipientPlayer);
    }
}

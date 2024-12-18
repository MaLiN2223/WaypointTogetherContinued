using HarmonyLib;
using System;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace WaypointTogetherContinued;

public class ClientNetwork
{
    private readonly ICoreClientAPI api;
    private readonly IClientNetworkChannel channel;

    string lastMessage = "";

    public ClientNetwork(ICoreClientAPI api)
    {
        this.api = api;

        channel = api.Network.RegisterChannel("malin.waypointtogethercontinued");
        channel.RegisterMessageType<ShareWaypointPacket>();
        channel.RegisterMessageType<ShareWaypointPacketFromServer>();
        channel.SetMessageHandler<ShareWaypointPacketFromServer>(this.HandlePacket);
    }

    public void ShareWaypoint(string message, string byUser)
    {
        if (message != null && message != "")
        {
            channel.SendPacket(new ShareWaypointPacket(message, byUser));
        }
    }

    private void HandlePacket(ShareWaypointPacketFromServer packet)
    {
        if (lastMessage == packet.Message)
        {
            return;
        }

        var currentPlayer = api.World.Player;

        if (packet.Message.StartsWith("/waypoint modify"))
        {
            // Modify currently is in format of /waypoint modify <id> <color> <icon> <pinned> <name>
            string[] split = packet.Message.Split(' ');
            string id = split[2];
            string color = split[3];
            string icon = split[4];
            string pinned = split[5];
            string name = split[6];

            var maplayers = api.ModLoader.GetModSystem<WorldMapManager>().MapLayers;
            var waypointLayer = (maplayers.Find(x => x is WaypointMapLayer) as WaypointMapLayer);
            Waypoint existing = packet.ExistingWaypoint;
            int myExistingId = waypointLayer.ownWaypoints.FindIndex(x => x.Position.X == existing.Position.X && x.Position.Y == existing.Position.Y && x.Position.Z == existing.Position.Z);
            if (myExistingId != -1)
            {
                // send modify, just replace id
                string message = "/waypoint modify " + myExistingId + " " + color + " " + icon + " " + pinned + " " + name;
                api.SendChatMessage(message);
            }
            else
            {
                // we want /waypoint addati [icon] [x] [y] [z] [pinned] [color] [title]
                string message = "/waypoint addti " + icon + " " + existing.Position.X + " " + existing.Position.Y + " " + existing.Position.Z + " " + pinned + " " + color + " " + name;
                api.SendChatMessage(packet.Message);
            }
        }
        else
        {
            api.SendChatMessage(packet.Message);
        }
        lastMessage = packet.Message;
    }
}

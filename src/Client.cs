using HarmonyLib;
using Vintagestory.API.Client;

namespace WaypointTogetherContinued;

public class Client
{
    public readonly ClientNetwork network;

    public Client(ICoreClientAPI api)
    {
        network = new ClientNetwork(api);
    }
}

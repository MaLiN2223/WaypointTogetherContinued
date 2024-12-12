using Vintagestory.API.Server;

namespace WaypointTogetherContinued;

public class Server
{
    public readonly ServerNetwork network;

    public Server(ICoreServerAPI api)
    {
        network = new ServerNetwork(api);
    }
}

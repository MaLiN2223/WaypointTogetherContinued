using Vintagestory.API.Server;
using WaypointTogetherContinued.Server;

namespace WaypointTogetherContinued;
/// <summary>
/// A <see langword="class" /> to wrap handling <see cref="ServerNetwork" /> packets.
/// </summary>
public class ServerWrapper
{
    /// <summary>
    /// Instance of the <see cref="ServerNetwork" /> packet handle.
    /// </summary>
    public readonly ServerNetwork network;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerWrapper" /> class.
    /// </summary>
    /// <param name="api">An instance of the game's core server API</param>
    public ServerWrapper(ICoreServerAPI api)
    {
        network = new ServerNetwork(api);
    }
}

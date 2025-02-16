using Vintagestory.API.Client;
using WaypointTogetherContinued.Client;
namespace WaypointTogetherContinued;
/// <summary>
/// A <see langword="class" /> to wrap handling <see cref="ClientNetwork" /> packets.
/// </summary>
public class ClientWrapper
{
    /// <summary>
    /// Instance of the <see cref="ClientNetwork" /> packet handler.
    /// </summary>
    public readonly ClientNetwork network;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientWrapper" /> class.
    /// </summary>
    /// <param name="api">An instance of the game's core client API</param>
    public ClientWrapper(ICoreClientAPI api)
    {
        network = new ClientNetwork(api);
    }
}

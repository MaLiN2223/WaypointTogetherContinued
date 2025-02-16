using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
[assembly: ModInfo("WaypointTogetherContinued", Authors = new[] { "nulliel", "malin2223" })]
namespace WaypointTogetherContinued;
/// <summary>
/// The core <see langword="class" /> of the mod.
/// </summary>
public class Core : ModSystem
{
    /// <summary>
    /// Gets the instance of the <see cref="ClientWrapper" />.
    /// </summary>
    public ClientWrapper? Client { get; private set; }

    /// <summary>
    /// Gets the instance of the <see cref="ServerWrapper" />.
    /// </summary>
    public ServerWrapper? Server { get; private set; }

    /// <summary>
    /// Gets the instance of the <see cref="Harmony" /> <see cref="Patcher" />.
    /// </summary>
    public Patcher? Patcher { get; private set; }

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        ModConfig.ReadConfig(api);
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);

        Client = new ClientWrapper(api);
        Patcher = new Patcher(nameof(WaypointTogetherContinued));
        Patcher.PatchAll(api);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);

        Server = new ServerWrapper(api);
    }

    public override void Dispose()
    {
        Patcher?.Dispose();

        base.Dispose();
    }
}

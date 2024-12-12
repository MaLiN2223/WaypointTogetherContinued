using HarmonyLib;
using Vintagestory.API.Common;

public class Patcher
{
    private readonly Harmony instance;

    public Patcher(string id)
    {
        instance = new Harmony(id);
    }

    public void PatchAll(ICoreAPI api)
    {
        ClientWaypointManager.PatchAll(instance, api);
        instance.PatchAll();
    }

    public void Dispose()
    {
        instance.UnpatchAll();
    }
}

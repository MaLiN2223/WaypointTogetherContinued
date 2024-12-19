using HarmonyLib;
using System;
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
        try
        {
            instance.PatchAll();
        }
        catch (Exception e)
        {
            api.Logger.Error(e.ToString());
            api.Logger.Error(e.InnerException.ToString());
            throw;
        }
    }

    public void Dispose()
    {
        instance.UnpatchAll();
    }
}

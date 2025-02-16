using HarmonyLib;
using System;
using Vintagestory.API.Common;
namespace WaypointTogetherContinued;
// Ignore Spelling: Unpatches
/// <summary>
/// An instance to wrap <see cref="Harmony" /> patches.
/// </summary>
public class Patcher : IDisposable
{
    /// <summary>
    /// The instance of the <see cref="Harmony" /> Patcher.
    /// </summary>
    private readonly Harmony instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="Patcher" /> class.
    /// </summary>
    /// <param name="id">The id of the mod that we will use for the <see cref="Patcher" />.</param>
    public Patcher(string id)
    {
        instance = new Harmony(id);
    }

    /// <summary>
    /// Invokes patching the methods and/or classes that we are trying to patch.
    /// </summary>
    /// <param name="api">An instance of the game's core API.</param>
    public void PatchAll(ICoreAPI api)
    {
        try
        {
            instance.PatchAll();
        }
        catch (Exception exception)
        {
            api.Logger.Error(exception.ToString());
            if (exception.InnerException is not null) {
                api.Logger.Error(exception.InnerException.ToString());
            }
            throw;
        }
    }

    /// <summary>
    /// Unpatches all <see cref="Harmony" /> patches from the game.
    /// </summary>
    public void Dispose()
    {
        instance.UnpatchAll();
        GC.SuppressFinalize(this);
    }
}

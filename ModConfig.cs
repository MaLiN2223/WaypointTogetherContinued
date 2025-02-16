using System;
using Vintagestory.API.Common;
namespace WaypointTogetherContinued;
/// <summary>
/// A <see langword="class" /> containing tools for the mod's config.
/// </summary>
internal static class ModConfig
{
    /// <summary>
    /// The instance of the <see cref="Config" /> class.
    /// </summary>
    public static Config? ClientConfig { get; private set; }

    /// <summary>
    /// The name of the client <see cref="Config" /> file.
    /// </summary>
    private const string ClientConfigFile = nameof(WaypointTogetherContinued) + ".json";

    /// <summary>
    /// Reads the <see cref="Config" /> data from file.
    /// </summary>
    /// <param name="api">The instance of the game's core API class.</param>
    public static void ReadConfig(ICoreAPI api)
    {
        try
        {
            if (api.Side == EnumAppSide.Client)
            {
                ClientConfig = LoadClientConfig(api);
                if (ClientConfig is not null)
                {
                    GenerateClientConfig(api);
                    ClientConfig = LoadClientConfig(api);
                }
                else
                {
                    GenerateClientConfig(api, ClientConfig);
                }
            }
        }
        catch (Exception e)
        {
            api.World.Logger.Error("Failed to load config: {0}", e);
            GenerateClientConfig(api);
            ClientConfig = LoadClientConfig(api);
        }
    }

    /// <summary>
    /// Generates the <see cref="Config" /> file for the client.
    /// </summary>
    /// <param name="api">The instance of the game's core API class.</param>
    private static void GenerateClientConfig(ICoreAPI api)
    {
        api.StoreModConfig(new Config(), ClientConfigFile);
    }

    /// <summary>
    /// Loads the <see cref="Config" /> file for the client.
    /// </summary>
    /// <param name="api">The instance of the game's core API class.</param>
    private static Config LoadClientConfig(ICoreAPI api)
    {
        return api.LoadModConfig<Config>(ClientConfigFile);
    }

    /// <summary>
    /// Generates a new <see cref="Config" /> file for the client with a old <see cref="Config" /> instance.
    /// </summary>
    /// <param name="api">The instance of the game's core API class.</param>
    /// <param name="previousConfig">The instance of the old client <see cref="Config" />.</param>
    private static void GenerateClientConfig(ICoreAPI api, Config? previousConfig)
    {
        api.StoreModConfig(new Config(previousConfig), ClientConfigFile);
    }
}

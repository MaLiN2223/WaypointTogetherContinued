namespace WaypointTogetherContinued;
/// <summary>
/// The <see langword="class" /> for the <see cref="Config" /> data.
/// </summary>
internal class Config
{
    /// <summary>
    /// Gets or sets a <see langword="bool" /> that determines if sharing should be enabled by default.
    /// </summary>
    public bool DefaultSharing { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Config" /> class.
    /// </summary>
    public Config() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Config" /> class.
    /// </summary>
    /// <param name="previousConfig">An instance of an old <see cref="Config" /> class.</param>
    public Config(Config? previousConfig)
    {
        DefaultSharing = previousConfig?.DefaultSharing ?? false;
    }
}

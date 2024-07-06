using FChatApi;
using FChatApi.Objects;
using FChatApi.Enums;
using Engine.ModuleHost.Enums;
using Engine.ModuleHost.CommandHandling;

namespace Engine.ModuleHost.Plugins;

/// <summary>
/// Our base plugin for others to derive off of
/// </summary>
public abstract class PluginBase
{
    /// <summary>the type of module this is</summary>
    public BotModule ModuleType { get; protected set; }

    /// <summary>how often this module runs Update()</summary>
    public TimeSpan UpdateInterval { get; }

    /// <summary>how often this module runs Update()</summary>
    public DateTime NextUpdate { get; private set; }

    /// <summary>
    /// construct the plugin base
    /// </summary>
    /// <param name="commandChar">the symbol that wakes the module up</param>
    /// <param name="updateInterval">how often this module runs Update().<br/>defaults to: Never</param>
    public PluginBase(TimeSpan? updateInterval = null) : this(default,updateInterval)
    {
        UpdateInterval = updateInterval ?? Timeout.InfiniteTimeSpan;
    }

    /// <summary>
    /// construct the plugin base
    /// </summary>
    /// <param name="commandChar">the symbol that wakes the module up</param>
    /// <param name="updateInterval">how often this module runs Update().<br/>defaults to: Never</param>
    public PluginBase(BotModule moduleType,TimeSpan? updateInterval = null)
    {
        ModuleType      = moduleType;
        UpdateInterval  = updateInterval ?? Timeout.InfiniteTimeSpan;
    }

    /// <summary>
    /// do periodically executed update stuff here
    /// </summary>
    /// <remarks>
    /// you <b><u>must</u></b> call this base method in the override
    /// </remarks>
    public virtual Task Update() => new(()=>NextUpdate = DateTime.Now + UpdateInterval);

    /// <summary>
    /// shuts down any volatile variables
    /// </summary>
    public virtual void Shutdown() { }
    
    ///////////////////////////////////

    /// <summary>
    /// hidden constructor
    /// </summary>
    private PluginBase() { }
}
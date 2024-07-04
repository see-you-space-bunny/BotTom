using ChatApi;
using ChatApi.Objects;
using ModuleHost.CommandHandling;

namespace ModuleHost;

/// <summary>
/// Our base plugin for others to derive off of
/// </summary>
public class PluginBase
{
    /// <summary>the type of module this is</summary>
    public BotModule ModuleType { get; }

    /// <summary>the symbol that wakes the module up</summary>
    public string CommandChar { get; }

    /// <summary>alternate symbol that wakes the module up<br/>this symbol does not need to be at the beginning of the message</summary>
    public string FloatingCommandChar { get; }

    /// <summary>how often this module runs Update()</summary>
    public TimeSpan UpdateInterval { get; }

    /// <summary>how often this module runs Update()</summary>
    public DateTime NextUpdate { get; private set; }

    /// <summary>does the plugin even look at messages at all</summary>
    public bool UsesMessageContents => !string.IsNullOrWhiteSpace(CommandChar) || !string.IsNullOrWhiteSpace(FloatingCommandChar);

    /// <summary>
    /// construct the plugin base
    /// </summary>
    /// <param name="commandChar">the symbol that wakes the module up</param>
    /// <param name="updateInterval">how often this module runs Update().<br/>defaults to: Never</param>
    public PluginBase(string? commandChar,TimeSpan? updateInterval = null) : this(commandChar,string.Empty,updateInterval) { }

    /// <summary>
    /// construct the plugin base
    /// </summary>
    /// <param name="commandChar">the symbol that wakes the module up</param>
    /// <param name="floatingCommandChar">alternate symbol that wakes the module up<br/>this symbol does not need to be at the beginning of the message</param>
    /// <param name="updateInterval">how often this module runs Update().<br/>defaults to: Never</param>
    public PluginBase(string? commandChar,string? floatingCommandChar,TimeSpan? updateInterval = null)
    {
        CommandChar = commandChar ?? string.Empty;
        FloatingCommandChar = floatingCommandChar ?? string.Empty;
        UpdateInterval = updateInterval ?? Timeout.InfiniteTimeSpan;
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    /// <summary>
    /// do periodically executed update stuff here
    /// </summary>
    /// <remarks>
    /// you <b><u>must</u></b> call this base method in the override
    /// </remarks>
    public async virtual Task Update()
    {
        NextUpdate = DateTime.Now + UpdateInterval;
    }
    
    /// <summary>
    /// shuts down any volatile variables
    /// </summary>
    public async virtual Task Shutdown() { }
#pragma warning restore CS1998
    
    ///////////////////////////////////

#pragma warning disable CS8618  // Non-nullable field must contain a non-null value when exiting constructor.
                                // Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// hidden constructor
    /// </summary>
    private PluginBase() { }
#pragma warning restore CS8618
}
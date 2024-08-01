namespace Plugins.Core;

/// <summary>
/// Our base plugin for others to derive off of
/// </summary>
public abstract class PluginBase
{
	/// <summary>how often this module runs Update()</summary>
	public TimeSpan UpdateInterval { get; }

	/// <summary>how often this module runs Update()</summary>
	public DateTime NextUpdate { get; protected set; }

	/// <summary>
	/// construct the plugin base
	/// </summary>
	/// <param name="commandChar">the symbol that wakes the module up</param>
	/// <param name="updateInterval">how often this module runs Update().<br/>defaults to: Never</param>
	public PluginBase(TimeSpan? updateInterval = null)
	{
		UpdateInterval  = updateInterval ?? Timeout.InfiniteTimeSpan;
	}

	public virtual void Initialize() { }

	/// <summary>
	/// do periodically executed update stuff here
	/// </summary>
	/// <remarks>
	/// you <b><u>must</u></b> call this base method in the override
	/// </remarks>
	public virtual void Update() => NextUpdate = Timeout.InfiniteTimeSpan == UpdateInterval ? DateTime.MaxValue : DateTime.Now + UpdateInterval;

	/// <summary>
	/// shuts down any volatile variables
	/// </summary>
	public virtual void Shutdown() { }
}
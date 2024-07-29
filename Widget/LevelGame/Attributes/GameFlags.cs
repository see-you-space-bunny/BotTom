namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class GameFlagsAttribute : Attribute
{
	// See the attribute guidelines at
	//  http://go.microsoft.com/fwlink/?LinkId=85236
	readonly bool _scalesOnLimitChange;
	readonly bool _refilledByFullRecovery;
	
	// This is a positional argument
	public GameFlagsAttribute(bool scalesOnLimitChange = false, bool refilledByFullRecovery = false)
	{
		this._scalesOnLimitChange = scalesOnLimitChange;
		this._refilledByFullRecovery = refilledByFullRecovery;
		
		// TODO: Implement code here
		/* throw new System.NotImplementedException(); */
	}
	
	public bool ScalesOnLimitChange
	{
		get { return this._scalesOnLimitChange; }
	}
	
	public bool RefilledByFullRecovery
	{
		get { return this._refilledByFullRecovery; }
	}
	
	// This is a named argument
	/* public int NamedInt { get; set; } */
}
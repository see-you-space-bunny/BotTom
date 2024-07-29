namespace RoleplayingGame.Objects;

public class GameObject(int level)
{
	#region Fields(#)
	protected int _level = level;
	#endregion

	#region Properties (+)
	public int Level => _level;
	public ulong Id { get; internal set; }
	#endregion
	
}
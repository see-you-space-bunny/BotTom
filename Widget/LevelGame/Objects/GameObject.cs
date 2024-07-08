namespace Widget.LevelGame;

public class GameObject(int level)
{
	#region Fields(#)
	protected int _level = level;
	#endregion

	#region Properties (+)
	public int Level => _level;
	#endregion
	
}
namespace DiceRoller.GameSystems;

internal static class MathLoader
{
	static MathLoader()
	{
		Envy.Envy.Load(Path.Combine(Environment.CurrentDirectory, ".env"));
	}
}

namespace Charsheet.MorkBorg.Actors;

internal class Resource(int maximum, int? current = null)
{
  internal int Maximum = maximum;
  internal int Current = current ?? maximum;
}

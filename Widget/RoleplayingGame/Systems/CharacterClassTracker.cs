using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Serialization;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Systems;

internal class CharacterClassTracker
{
#region (-) Fields
    private readonly Dictionary<ClassName,CharacterClass> _characterClasses;
#endregion


#region (+) Properties
    internal Dictionary<ClassName,CharacterClass> All => _characterClasses;
#endregion


#region (+) Properties
	internal string SourceFilePath { get; set; }
#endregion


#region (+) InitializeFrom
	internal void InitializeFrom(string filePath)
	{
		foreach (CharacterClass @class in DeserializeKommaVaues.GetClasses(filePath))
		{
			_characterClasses.Add(@class.Name,@class);
		}
	}
#endregion


#region Constructor
	internal CharacterClassTracker()
	{
		SourceFilePath		= "CharacterClasses - Export.csv";
		_characterClasses	= [];
	}
#endregion
}
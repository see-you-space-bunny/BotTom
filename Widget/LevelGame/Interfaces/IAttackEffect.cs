using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LevelGame.SheetComponents;

namespace LevelGame.Interfaces;

public interface IAttackEffect
{
	public bool TryToHit(CharacterResource evasion,CharacterResource health);
	public bool TryToImpact(CharacterResource protection);
	public bool TryToHarm(CharacterResource health);
}
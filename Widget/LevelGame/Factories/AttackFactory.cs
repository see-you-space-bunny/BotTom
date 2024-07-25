using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LevelGame.Effects;
using LevelGame.Enums;
using LevelGame.Objects;

namespace LevelGame.Factories;

public class AttackFactory
{
	public AttackEffect NewAttack<TSource>(AttackType attackType,TSource source,Actor target)
	{
		var aeb = new AttackEffectBuilder();
		switch (attackType)
		{
			case AttackType.Basic:
				break;

			default:
				break;
		}
		return null!;
	}
}
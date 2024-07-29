using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Factories;

public class StatusEffectFactory
{
	

	public ActiveStatusEffect CreateStatusEffect(StatusEffect statusEffect,Actor target,float intensity,Actor? source = null)
	{
		var effectBuilder = new ActiveStatusEffectBuilder().WithEffectType(statusEffect);

		switch (statusEffect)
		{
			case StatusEffect.Defeated:
				AssembleDefeated(effectBuilder);
				break;

			default:
				break;
		}

		return effectBuilder.Build();
	}


	private static void AssembleDefeated(ActiveStatusEffectBuilder effectBuilder)
	{
	}
}
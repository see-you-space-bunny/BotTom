using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LevelGame.Objects;

namespace LevelGame.Effects;

public class AttackEffectBuilder
{
	Actor? _source;
    float _harm;
    float _impact;
    float _accuracy;
    readonly List<ActiveStatusEffectBuilder> _carriedEffects;

    public AttackEffectBuilder WithStatusEffects(ActiveStatusEffectBuilder[] effectBuilders)
    {
        foreach (var effectBuilder in effectBuilders)
        {
            _carriedEffects.Add(effectBuilder);
        }
        return this;
    }

    public AttackEffectBuilder WithStatusEffect(ActiveStatusEffectBuilder effectBuilder)
    {
        _carriedEffects.Add(effectBuilder);
        return this;
    }

    public AttackEffectBuilder WithDamage(float harm,float impact,float accuracy) =>
        this.WithHarm(harm).WithImpact(impact).WithAccuracy(accuracy);

    public AttackEffectBuilder WithHarm(float value)
    {
        _harm = value;
        return this;
    }

    public AttackEffectBuilder WithImpact(float value)
    {
        _impact = value;
        return this;
    }

    public AttackEffectBuilder WithAccuracy(float value)
    {
        _accuracy = value;
        return this;
    }

    public bool IsReadyToBuild() =>
		_accuracy > 0.0f && (_harm > 0.0f || _impact > 0.0f);

    public AttackEffect Build()
    {
        return new AttackEffect(
			_harm,
			_impact,
			_accuracy,
            [.. _carriedEffects],
			_source
        );
    }

    public AttackEffectBuilder()
    {
        _harm			= 0.0f;
        _impact			= 0.0f;
        _accuracy		= 0.0f;
        _carriedEffects	= [];
    }
}
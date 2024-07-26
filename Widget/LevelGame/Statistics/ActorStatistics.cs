using System.Numerics;
using LevelGame.Effects;
using LevelGame.Enums;
using LevelGame.Objects;

namespace LevelGame.Statistics;

public class ActorStatistics
{
	#region Combat Records
	private Dictionary<StatisticsCategory,Dictionary<ulong,ulong>> _combatRecordsPvPIncoming;
	private Dictionary<StatisticsCategory,Dictionary<ulong,ulong>> _combatRecordsPvPOutgoing;
	private Dictionary<StatisticsCategory,Dictionary<EnvironmentSource,ulong>> _combatRecordsPvEIncoming;
	private Dictionary<StatisticsCategory,Dictionary<EnvironmentSource,ulong>> _combatRecordsPvEOutgoing;
	#endregion

	#region Combined Totals
	public BigInteger TotalEvasionLoss =>
		new BigInteger(_combatRecordsPvEIncoming[StatisticsCategory.EvasionLoss].Values.ToList().Sum(uL=>(decimal)uL)) + 
		new BigInteger(_combatRecordsPvPIncoming[StatisticsCategory.EvasionLoss].Values.ToList().Sum(uL=>(decimal)uL));
	public BigInteger TotalProtectionLoss =>
		new BigInteger(_combatRecordsPvEIncoming[StatisticsCategory.ProtectionLoss].Values.ToList().Sum(uL=>(decimal)uL)) + 
		new BigInteger(_combatRecordsPvPIncoming[StatisticsCategory.ProtectionLoss].Values.ToList().Sum(uL=>(decimal)uL));
	public BigInteger TotalHealthLoss =>
		new BigInteger(_combatRecordsPvEIncoming[StatisticsCategory.HealthLoss].Values.ToList().Sum(uL=>(decimal)uL)) + 
		new BigInteger(_combatRecordsPvPIncoming[StatisticsCategory.HealthLoss].Values.ToList().Sum(uL=>(decimal)uL));
	public BigInteger TotalDamageTaken => TotalProtectionLoss + TotalHealthLoss;
	#endregion

	#region Record Events
	public void RecordIncomingAttackResults(EnvironmentSource source,ulong evasionLoss,bool hit,ulong protectionLoss,bool protBreak,
										ulong healthLoss,bool kill,ulong overkill,
										Actor[]? assistingPlayers = null,EnvironmentSource[]? assistingEnvironment = null)
	{
		AttackRecordSanityCheck(source);
		_combatRecordsPvEIncoming[StatisticsCategory.EvasionLoss][source]+=evasionLoss;
		if (hit)
		{
			_combatRecordsPvEIncoming[StatisticsCategory.Hit][source]++;
			_combatRecordsPvEIncoming[StatisticsCategory.ProtectionLoss][source]+=protectionLoss;
			if (protBreak)
			{
				_combatRecordsPvEIncoming[StatisticsCategory.ProtBreak][source]++;
				_combatRecordsPvEIncoming[StatisticsCategory.HealthLoss][source]+=healthLoss;
			}
			if (kill)
			{
				_combatRecordsPvEIncoming[StatisticsCategory.Kill][source]++;
				_combatRecordsPvEIncoming[StatisticsCategory.Overkill][source]+=overkill;
				RecordAssistants(incoming:true,assistingPlayers??[],assistingEnvironment??[]);
			}
		}
		else
		{
			_combatRecordsPvEIncoming[StatisticsCategory.Miss][source]++;
		}
	}

	public void RecordIncomingAttackResults(Actor source,ulong evasionLoss,bool hit,ulong protectionLoss,bool protBreak,
										ulong healthLoss,bool kill,ulong overkill,
										Actor[]? assistingPlayers = null,EnvironmentSource[]? assistingEnvironment = null)
	{
		AttackRecordSanityCheck(source);
		_combatRecordsPvPIncoming[StatisticsCategory.EvasionLoss][source.Id]+=evasionLoss;
		if (hit)
		{
			_combatRecordsPvPIncoming[StatisticsCategory.Hit][source.Id]++;
			_combatRecordsPvPIncoming[StatisticsCategory.ProtectionLoss][source.Id]+=protectionLoss;
			if (protBreak)
			{
				_combatRecordsPvPIncoming[StatisticsCategory.ProtBreak][source.Id]++;
				_combatRecordsPvPIncoming[StatisticsCategory.HealthLoss][source.Id]+=healthLoss;
			}
			if (kill)
			{
				_combatRecordsPvPIncoming[StatisticsCategory.Kill][source.Id]++;
				_combatRecordsPvPIncoming[StatisticsCategory.Overkill][source.Id]+=overkill;
				RecordAssistants(incoming:true,assistingPlayers??[],assistingEnvironment??[]);
			}
		}
		else
		{
			_combatRecordsPvPIncoming[StatisticsCategory.Miss][source.Id]++;
		}
	}

	public void RecordOutgoingAttackResults(EnvironmentSource target,ulong evasionLoss,bool hit,ulong protectionLoss,bool protBreak,
										ulong healthLoss,bool kill,ulong overkill,
										Actor[]? assistingPlayers = null,EnvironmentSource[]? assistingEnvironment = null)
	{
		AttackRecordSanityCheck(target);
		_combatRecordsPvEOutgoing[StatisticsCategory.EvasionLoss][target]+=evasionLoss;
		if (hit)
		{
			_combatRecordsPvEOutgoing[StatisticsCategory.Hit][target]++;
			_combatRecordsPvEOutgoing[StatisticsCategory.ProtectionLoss][target]+=protectionLoss;
			if (protBreak)
			{
				_combatRecordsPvEOutgoing[StatisticsCategory.ProtBreak][target]++;
				_combatRecordsPvEOutgoing[StatisticsCategory.HealthLoss][target]+=healthLoss;
			}
			if (kill)
			{
				_combatRecordsPvEOutgoing[StatisticsCategory.Kill][target]++;
				_combatRecordsPvEOutgoing[StatisticsCategory.Overkill][target]+=overkill;
				RecordAssistants(incoming:false,assistingPlayers??[],assistingEnvironment??[]);
			}
		}
		else
		{
			_combatRecordsPvEOutgoing[StatisticsCategory.Miss][target]++;
		}
	}

	public void RecordOutgoingAttackResults(Actor target,ulong evasionLoss,bool hit,ulong protectionLoss,bool protBreak,
										ulong healthLoss,bool kill,ulong overkill,
										Actor[]? assistingPlayers = null,EnvironmentSource[]? assistingEnvironment = null)
	{
		AttackRecordSanityCheck(target);
		_combatRecordsPvPOutgoing[StatisticsCategory.EvasionLoss][target.Id]+=evasionLoss;
		if (hit)
		{
			_combatRecordsPvPOutgoing[StatisticsCategory.Hit][target.Id]++;
			_combatRecordsPvPOutgoing[StatisticsCategory.ProtectionLoss][target.Id]+=protectionLoss;
			if (protBreak)
			{
				_combatRecordsPvPOutgoing[StatisticsCategory.ProtBreak][target.Id]++;
				_combatRecordsPvPOutgoing[StatisticsCategory.HealthLoss][target.Id]+=healthLoss;
			}
			if (kill)
			{
				_combatRecordsPvPOutgoing[StatisticsCategory.Kill][target.Id]++;
				_combatRecordsPvPOutgoing[StatisticsCategory.Overkill][target.Id]+=overkill;
				RecordAssistants(incoming:false,assistingPlayers??[],assistingEnvironment??[]);
			}
		}
		else
		{
			_combatRecordsPvPOutgoing[StatisticsCategory.Miss][target.Id]++;
		}
	}
	private void RecordAssistants(bool incoming, Actor[] assistingPlayers,EnvironmentSource[] assistingEnvironment)
	{
		if (incoming)
		{
			foreach (Actor player in assistingPlayers)
			{
				_combatRecordsPvPIncoming[StatisticsCategory.Kill][player.Id]++;
			}
			foreach (EnvironmentSource environmentSource in assistingEnvironment)
			{
				_combatRecordsPvEIncoming[StatisticsCategory.Kill][environmentSource]++;
			}
		}
		else
		{
			foreach (Actor player in assistingPlayers)
			{
				_combatRecordsPvPOutgoing[StatisticsCategory.Kill][player.Id]++;
			}
			foreach (EnvironmentSource environmentSource in assistingEnvironment)
			{
				_combatRecordsPvEOutgoing[StatisticsCategory.Kill][environmentSource]++;
			}
		}
	}
	private void AttackRecordSanityCheck(Actor target)
	{
		if (_combatRecordsPvPOutgoing[StatisticsCategory.EvasionLoss].TryAdd(target.Id,0uL))
		{
			_ = _combatRecordsPvPOutgoing[StatisticsCategory.Hit]			.TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPOutgoing[StatisticsCategory.ProtectionLoss].TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPOutgoing[StatisticsCategory.ProtBreak]		.TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPOutgoing[StatisticsCategory.HealthLoss]	.TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPOutgoing[StatisticsCategory.Kill]			.TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPOutgoing[StatisticsCategory.Overkill]		.TryAdd(target.Id,0uL);
		}
		if (_combatRecordsPvPIncoming[StatisticsCategory.EvasionLoss].TryAdd(target.Id,0uL))
		{
			_ = _combatRecordsPvPIncoming[StatisticsCategory.Hit]			.TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPIncoming[StatisticsCategory.ProtectionLoss].TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPIncoming[StatisticsCategory.ProtBreak]		.TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPIncoming[StatisticsCategory.HealthLoss]	.TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPIncoming[StatisticsCategory.Kill]			.TryAdd(target.Id,0uL);
			_ = _combatRecordsPvPIncoming[StatisticsCategory.Overkill]		.TryAdd(target.Id,0uL);
		}
	}
	private void AttackRecordSanityCheck(EnvironmentSource target)
	{
		if (_combatRecordsPvEOutgoing[StatisticsCategory.EvasionLoss].TryAdd(target,0uL))
		{
			_ = _combatRecordsPvEOutgoing[StatisticsCategory.Hit]			.TryAdd(target,0uL);
			_ = _combatRecordsPvEOutgoing[StatisticsCategory.ProtectionLoss].TryAdd(target,0uL);
			_ = _combatRecordsPvEOutgoing[StatisticsCategory.ProtBreak]		.TryAdd(target,0uL);
			_ = _combatRecordsPvEOutgoing[StatisticsCategory.HealthLoss]	.TryAdd(target,0uL);
			_ = _combatRecordsPvEOutgoing[StatisticsCategory.Kill]			.TryAdd(target,0uL);
			_ = _combatRecordsPvEOutgoing[StatisticsCategory.Overkill]		.TryAdd(target,0uL);
		}
		if (_combatRecordsPvEIncoming[StatisticsCategory.EvasionLoss].TryAdd(target,0uL))
		{
			_ = _combatRecordsPvEIncoming[StatisticsCategory.Hit]			.TryAdd(target,0uL);
			_ = _combatRecordsPvEIncoming[StatisticsCategory.ProtectionLoss].TryAdd(target,0uL);
			_ = _combatRecordsPvEIncoming[StatisticsCategory.ProtBreak]		.TryAdd(target,0uL);
			_ = _combatRecordsPvEIncoming[StatisticsCategory.HealthLoss]	.TryAdd(target,0uL);
			_ = _combatRecordsPvEIncoming[StatisticsCategory.Kill]			.TryAdd(target,0uL);
			_ = _combatRecordsPvEIncoming[StatisticsCategory.Overkill]		.TryAdd(target,0uL);
		}
	}
	#endregion

	#region Constructor
	public ActorStatistics()
	{
		_combatRecordsPvPIncoming	= [];
		_combatRecordsPvPOutgoing	= [];
		_combatRecordsPvEIncoming	= [];
		_combatRecordsPvEOutgoing	= [];
	}
	#endregion

	#region Serialization
	public static ActorStatistics Deserialize(BinaryReader reader)
	{
		var statistics = new ActorStatistics();
		for (uint i = 0;i<reader.ReadUInt32();i++)
		{
			var category = (StatisticsCategory)reader.ReadUInt32();
			statistics._combatRecordsPvPIncoming.TryAdd(category,[]);
			for (uint j = 0;j<reader.ReadUInt32();j++)
			{
				statistics._combatRecordsPvPIncoming[category].TryAdd(reader.ReadUInt64(),reader.ReadUInt64());
			}
		}
		for (uint i = 0;i<reader.ReadUInt32();i++)
		{
			var category = (StatisticsCategory)reader.ReadUInt32();
			statistics._combatRecordsPvPOutgoing.TryAdd(category,[]);
			for (uint j = 0;j<reader.ReadUInt32();j++)
			{
				statistics._combatRecordsPvPOutgoing[category].TryAdd(reader.ReadUInt64(),reader.ReadUInt64());
			}
		}
		for (uint i = 0;i<reader.ReadUInt32();i++)
		{
			var category = (StatisticsCategory)reader.ReadUInt32();
			statistics._combatRecordsPvEIncoming.TryAdd(category,[]);
			for (uint j = 0;j<reader.ReadUInt32();j++)
			{
				statistics._combatRecordsPvEIncoming[category].TryAdd((EnvironmentSource)reader.ReadUInt32(),reader.ReadUInt64());
			}
		}
		for (uint i = 0;i<reader.ReadUInt32();i++)
		{
			var category = (StatisticsCategory)reader.ReadUInt32();
			statistics._combatRecordsPvEOutgoing.TryAdd(category,[]);
			for (uint j = 0;j<reader.ReadUInt32();j++)
			{
				statistics._combatRecordsPvEOutgoing[category].TryAdd((EnvironmentSource)reader.ReadUInt32(),reader.ReadUInt64());
			}
		}
		return statistics;
	}
	public void Serialize(BinaryWriter writer)
	{
		writer.Write((uint)	_combatRecordsPvPIncoming.Count);
		foreach ((var category, var pvpRecords) in _combatRecordsPvPIncoming)
		{
			writer.Write((uint)	category);
			writer.Write((uint)	pvpRecords.Count);
			foreach ((ulong id,var pvpRecord) in pvpRecords)
			{
				writer.Write((ulong)	id);
				writer.Write((ulong)	pvpRecord);
			}
		}
		writer.Write((uint)	_combatRecordsPvPOutgoing.Count);
		foreach ((var category, var pvpRecords) in _combatRecordsPvPOutgoing)
		{
			writer.Write((uint)	category);
			writer.Write((uint)	pvpRecords.Count);
			foreach ((ulong id,var pvpRecord) in pvpRecords)
			{
				writer.Write((ulong)	id);
				writer.Write((ulong)	pvpRecord);
			}
		}
		writer.Write((uint)	_combatRecordsPvEIncoming.Count);
		foreach ((var category, var pveRecords) in _combatRecordsPvEIncoming)
		{
			writer.Write((uint)	category);
			writer.Write((uint)	pveRecords.Count);
			foreach ((EnvironmentSource source,var pveRecord) in pveRecords)
			{
				writer.Write((uint)		source);
				writer.Write((ulong)	pveRecord);
			}
		}
		writer.Write((uint)	_combatRecordsPvEOutgoing.Count);
		foreach ((var category, var pveRecords) in _combatRecordsPvEOutgoing)
		{
			writer.Write((uint)	category);
			writer.Write((uint)	pveRecords.Count);
			foreach ((EnvironmentSource source,var pveRecord) in pveRecords)
			{
				writer.Write((uint)		source);
				writer.Write((ulong)	pveRecord);
			}
		}
	}
	#endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Attributes;
using FChatApi.Enums;
using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Systems;

public class DieRoll
{
#region (-) Constants
	private const string SymbolGameDie				=	"🎲";
	private const string FormatDieRoll				=	"{0}{1}";
	private const string FormatDieRollModified		=	"{0}{1}+{2}";
	private const string FormatBBCodeWithArg		=	"[{1}={2}]{0}[/{1}]";
	private const BBCodeColor ColorNegativeValue	=	BBCodeColor.black;
#endregion


#region (+) Enums
	public enum RollType
	{
		Basic,
	}
#endregion


#region (-) Fields
	public readonly RollType	_rollType;
#endregion


#region (+) Fields
	public readonly DateTime				Timestamp;
	public readonly Tuple<int,int>[]		Results;
	public readonly Tuple<Ability,int>[]	Modifiers;
#endregion


#region (+) Properties
	public int	Total	=>	Results.Sum(r=>r.Item2);
#endregion


#region (+) ToString
	public override string ToString()
	{
		string result	=	string.Format(FormatDieRoll,Total,SymbolGameDie);
		foreach (Tuple<Ability,int> modifier in Modifiers)
		{
			string formMod	=	modifier.Item2 >= 0 ? modifier.Item2.ToString() : string.Format(FormatBBCodeWithArg,Math.Abs(modifier.Item2),"color",ColorNegativeValue);
			var deco		=	modifier.Item1.GetEnumAttribute<Ability,StatDecorationAttribute>();
			result			=	string.Format(FormatDieRollModified,formMod,deco?.Emoji ?? string.Empty,result);
		}
		return result;
	}
#endregion


#region (+) Serialization
	public static DieRoll Deserialize(BinaryReader reader)
	{
		var timestamp	=	new DateTime(reader.ReadInt64());
		var results		=	new List<Tuple<int,int>>();
		for (uint i=0;i<reader.ReadUInt16();i++)
		{
			results.Add(new Tuple<int,int>(reader.ReadInt32(),reader.ReadInt32()));
		}
		var modifiers	=	new List<Tuple<Ability,int>>();
		for (uint i=0;i<reader.ReadUInt16();i++)
		{
			modifiers.Add(new Tuple<Ability,int>((Ability)reader.ReadUInt16(),reader.ReadInt32()));
		}
		var rollType	=	(RollType)reader.ReadUInt16();
		return new DieRoll(timestamp,[.. results],[.. modifiers],rollType);
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((long)		Timestamp.Ticks);
		writer.Write((ushort)	Results.Length);
		foreach (Tuple<int,int> result in Results)
		{
			writer.Write((int)	result.Item1);
			writer.Write((int)	result.Item2);
		}
		writer.Write((ushort)	Modifiers.Length);
		foreach (Tuple<Ability,int> modifier in Modifiers)
		{
			writer.Write((ushort)	modifier.Item1);
			writer.Write((int)		modifier.Item2);
		}
		writer.Write((ushort)	_rollType);
	}
#endregion


#region Constructor
	public DieRoll(Tuple<int,int>[] results)
		: this(results,[])
	{ }

	public DieRoll(Tuple<int,int>[] results,Tuple<Ability,int>[] modifiers)
		: this(results,modifiers,RollType.Basic)
	{ }

	public DieRoll(Tuple<int,int>[] results,Tuple<Ability,int>[] modifiers,RollType rollType)
		: this(DateTime.Now,results,modifiers,rollType)
	{ }

	private DieRoll(DateTime timestamp,Tuple<int,int>[] results,Tuple<Ability,int>[] modifiers,RollType rollType)
	{
		Timestamp	=	timestamp;
		Results		=	results;
		Modifiers	=	modifiers;
		_rollType	=	rollType;
	}
#endregion


#region Static Constructor
	static DieRoll()
	{

	}
#endregion
}
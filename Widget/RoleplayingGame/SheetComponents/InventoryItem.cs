using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FChatApi.Attributes;
using RoleplayingGame.Attributes;
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;
using RoleplayingGame.Systems;

namespace RoleplayingGame.SheetComponents;

internal class InventoryItem
{
#region (-) Fields
	private readonly ItemCategory	_itemCategory;
	private readonly SpecificItem	_specificItem;
	private readonly bool			_occupiesSlot;
	private readonly ulong			_taggedById;
#endregion


#region (-) Fields
	private readonly List<ActiveStatusEffect>	_activeStatuses;
#endregion


#region (-) Fields
	private string	_name;
	private string	_description;
	private uint	_amount;
	private float	_bulk;
	private Actor	_taggedBy;
#endregion


#region (-) Properties
	private string DefaultDescription	=>
		_specificItem != SpecificItem.Custom
			?	_itemCategory.GetEnumAttribute<ItemCategory,DescriptionAttribute>().Description		//	TODO: create attribute
			:	_specificItem.GetEnumAttribute<SpecificItem,DescriptionAttribute>().Description;	//	TODO: create attribute
#endregion


#region (~) misc Properties
	internal float			Bulk				=> _bulk * _amount;
	internal ItemCategory	Category			=> _itemCategory;
	internal SpecificItem	SpecificItemType	=> _specificItem;
#endregion


#region (~) bool Properties
	internal bool	IsCustomItem		=>	_specificItem == SpecificItem.Custom;
	internal bool	OccupiesSlot		=> _occupiesSlot && !Equipped;
#endregion


#region (~) [set] Properties
	internal bool	Equipped	{ get; set; }
#endregion


#region (~) string Properties
	internal string	Name		{ get=>_name; set=>_name=value; }
	internal string	Description	{ get=>_description; set=>_description=value; }
#endregion


#region (~) [set] Properties
	internal uint	Amount		{ get=>_amount; set { _amount = Math.Max(_amount+value,0); } }
	internal float	UnitBulk	{ get=>_bulk; set=>_bulk=value; }
#endregion


#region (+) ToString
	public override string ToString()	=>
		_name;

	public string ToString(bool withDescription,bool withTags = true)
	{
		StringBuilder sb	= new (ToString());
		if (withTags)
		{
			sb.Append(TagsString());
		}

		if (withDescription)
		{
			sb.AppendFormat("\n{0}",_description);
		}
		return sb.ToString();
	}

	private string TagsString()
	{
		const string FormatBasic	=	"[sup]({0})[/sup]";
		const string FormatWithTag	=	"[sup]({0},{1})[/sup]";
		if ( _taggedBy is not null  || _activeStatuses.Any(s=>s.Tagged))
		{
			Actor actor	=	_taggedBy ?? _activeStatuses.FirstOrDefault(s=>s.Tagged)?.Source!;

			if (actor is not null)
			{
				return string.Format(FormatWithTag,string.Join(',',[.. _activeStatuses.Select(s=>s.ToString())]),actor.CharacterName);
			}
			else
			{
				return string.Format(FormatBasic,string.Join(',',[.. _activeStatuses.Select(s=>s.ToString())]));
			}
		}
		return string.Empty;
	}
#endregion


#region Initialize
	internal void Initialize(CharacterTracker characterTracker)
	{
		_taggedBy	=	characterTracker.SingleById(_taggedById);
	}
#endregion


#region Serialization
	internal static InventoryItem Deserialize(BinaryReader reader)
	{
		InventoryItem result;
/////	Constructor
		if (reader.ReadBoolean())
		{
			result	=	new InventoryItem(
				(string)		reader.ReadString(),
				(ItemCategory)	reader.ReadUInt16(),
				(ulong)			reader.ReadUInt64(),
				(float)			reader.ReadSingle()
			);
			//_activeStatuses;
		}
		else
		{
			result	=	new InventoryItem(
				(SpecificItem)	reader.ReadUInt16(),
				(float)			reader.ReadSingle()
			);
		}
		result.Amount	=	reader.ReadUInt32();

/////	Description
		if (reader.ReadBoolean())
		{
			result._description	=	reader.ReadString();
		}
		return result;
	}

	internal void Serialize(BinaryWriter writer)
	{
/////	Constructor
		writer.Write((bool)	IsCustomItem);
		if (IsCustomItem)
		{
			writer.Write((string)	_name);
			writer.Write((ushort)	_itemCategory);
			writer.Write((ulong)	_taggedBy.ActorId);
			writer.Write((float)	_bulk);
			//_activeStatuses;
		}
		else
		{
			writer.Write((ushort)	SpecificItemType);
		}
		writer.Write((uint)		_amount);

/////	Description
		if (_description == DefaultDescription)
		{
			writer.Write(true);
			writer.Write((string)	_description);
		}
		else
		{
			writer.Write(false);
		}
	}
#endregion


#region Constructor
	internal InventoryItem(string name,ItemCategory category,ulong taggedById,float bulk = 0.0f,uint amount = 1)
		: this(name,category,taggedById,[],bulk,amount)
	{ }
	
	internal InventoryItem(string name,ItemCategory category,ulong taggedById,ActiveStatusEffect[] activeStatuses,float bulk = 0.0f,uint amount = 1)
		: this(category,bulk,amount)
	{
		_name			=	name;
		_description	=	DefaultDescription;
		_specificItem	=	SpecificItem.Custom;
		_taggedBy		=	null!;
		_taggedById		=	taggedById;
		_activeStatuses	=	[.. activeStatuses];
	}

	internal InventoryItem(string name,ItemCategory category,Actor taggedBy,float bulk = 0.0f,uint amount = 1)
		: this(name,category,taggedBy,[],bulk,amount)
	{ }
	
	internal InventoryItem(string name,ItemCategory category,Actor taggedBy,ActiveStatusEffect[] activeStatuses,float bulk = 0.0f,uint amount = 1)
		: this(category,bulk,amount)
	{
		_name			=	name;
		_description	=	DefaultDescription;
		_specificItem	=	SpecificItem.Custom;
		_taggedBy		=	taggedBy;
		_taggedById		=	_taggedBy.ActorId;
		_activeStatuses	=	[.. activeStatuses];
	}

	internal InventoryItem(SpecificItem specificItem,float bulk = 0.0f,uint amount = 1)
		: this(specificItem.GetEnumAttribute<SpecificItem,SpecificItemCategoryAttribute>().Category,bulk,amount)
	{
		_name			=	specificItem.GetEnumAttribute<SpecificItem,DescriptionAttribute>().Description;	//	TODO: create attribute
		_description	=	DefaultDescription;
		_specificItem	=	specificItem;
		_taggedBy		=	null!;
		_activeStatuses	=	[];	//	TODO: create attribute
	}
#endregion


#region Private Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private InventoryItem(ItemCategory category,float bulk,uint amount)
	{
		_itemCategory	=	category;
		_bulk			=	bulk;
		_amount			=	amount;
	}

	private InventoryItem()
	{
		Equipped	=	false;
	}
#pragma warning restore CS8618
#endregion
}
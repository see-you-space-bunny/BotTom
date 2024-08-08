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
#region (-) Constants
	private const string TagFormatBasic			=	"[sup]({0})[/sup]";
	private const string TagFormatWithSource	=	"[sup]({0},{1})[/sup]";
#endregion


#region (-) Fields
	private readonly ItemCategory		_itemCategory;
	private readonly SpecificItem		_specificItem;
	private readonly EnvironmentSource	_environmentSource;
	private readonly bool				_occupiesSlot;
	private readonly ulong				_taggedById;
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
			?	_specificItem.GetEnumAttribute<SpecificItem,DescriptionAttribute>().Description		//	TODO: create attribute
			:	_itemCategory.GetEnumAttribute<ItemCategory,DescriptionAttribute>().Description;	//	TODO: create attribute
#endregion


#region (~) misc Properties
	internal float			Bulk				=> _bulk * _amount;
	internal ItemCategory	Category			=> _itemCategory;
	internal SpecificItem	SpecificItemType	=> _specificItem;
#endregion


#region (~) bool Properties
	internal bool	IsCustomItem	=>	_specificItem == SpecificItem.Custom;
	internal bool	OccupiesSlot	=> _occupiesSlot && !Equipped;
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
		if ( _taggedBy is not null  || _activeStatuses.Any(s=>s.Tagged))
		{
			Actor actor	=	_taggedBy ?? _activeStatuses.FirstOrDefault(s=>s.Tagged)?.Source!;

			if (actor is not null)
			{
				return string.Format(TagFormatWithSource,string.Join(',',[.. _activeStatuses.Select(s=>s.ToString())]),actor.CharacterName);
			}
			else
			{
				return string.Format(TagFormatBasic,string.Join(',',[.. _activeStatuses.Select(s=>s.ToString())]));
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
				(string)			reader.ReadString(),
				(ItemCategory)		reader.ReadUInt16(),
				(ulong)				reader.ReadUInt64(),
				(EnvironmentSource)	reader.ReadUInt16(),
				(float)				reader.ReadSingle()
			);
			//_activeStatuses;
		}
		else
		{
			result	=	new InventoryItem(
				(SpecificItem)		reader.ReadUInt16(),
				(EnvironmentSource)	reader.ReadUInt16(),
				(float)				reader.ReadSingle()
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
			writer.Write((ushort)	_environmentSource);
			writer.Write((float)	_bulk);
			//_activeStatuses;
		}
		else
		{
			writer.Write((ushort)	SpecificItemType);
			writer.Write((ushort)	_environmentSource);
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
/// <summary>Constructs an item with an actorId as source. Must be Initialized!!</summary>
	internal InventoryItem(string name,ItemCategory category,ulong taggedById,float bulk = 0.0f,uint amount = 1)
		: this(name,category,taggedById,EnvironmentSource.None,[],bulk,amount)
	{ }

/// <summary>Constructs an item with an actorId as source. Must be Initialized!!</summary>
	internal InventoryItem(string name,ItemCategory category,ulong taggedById,EnvironmentSource environmentSource,float bulk = 0.0f,uint amount = 1)
		: this(name,category,taggedById,environmentSource,[],bulk,amount)
	{ }
	
/// <summary>Constructs an item with an actorId as source. Must be Initialized!!</summary>
	internal InventoryItem(string name,ItemCategory category,ulong taggedById,EnvironmentSource environmentSource,ActiveStatusEffect[] activeStatuses,float bulk = 0.0f,uint amount = 1)
		: this(category,environmentSource,bulk,amount,true)
	{
		_name			=	name;
		_description	=	DefaultDescription;
		_specificItem	=	SpecificItem.Custom;
		_taggedBy		=	null!;
		_taggedById		=	taggedById;
		_activeStatuses	=	[.. activeStatuses];
	}

/// <summary>Constructs an item with an actor as source.</summary>
	internal InventoryItem(string name,ItemCategory category,Actor taggedBy,float bulk = 0.0f,uint amount = 1)
		: this(name,category,taggedBy,EnvironmentSource.None,[],bulk,amount)
	{ }

/// <summary>Constructs an item with an actor as source.</summary>
	internal InventoryItem(string name,ItemCategory category,Actor taggedBy,EnvironmentSource environmentSource,float bulk = 0.0f,uint amount = 1)
		: this(name,category,taggedBy,environmentSource,[],bulk,amount)
	{ }
	
/// <summary>Constructs an item with an actor as source.</summary>
	internal InventoryItem(string name,ItemCategory category,Actor taggedBy,EnvironmentSource environmentSource,ActiveStatusEffect[] activeStatuses,float bulk = 0.0f,uint amount = 1)
		: this(category,environmentSource,bulk,amount,true)
	{
		_name			=	name;
		_description	=	DefaultDescription;
		_specificItem	=	SpecificItem.Custom;
		_taggedBy		=	taggedBy;
		_taggedById		=	_taggedBy.ActorId;
		_activeStatuses	=	[.. activeStatuses];
	}

/// <summary>Constructs a specific pre-defined item.</summary>
	internal InventoryItem(SpecificItem specificItem,float bulk = 0.0f,uint amount = 1)
		: this(specificItem,EnvironmentSource.World,bulk,amount)
	{ }

/// <summary>Constructs a specific pre-defined item.</summary>
	internal InventoryItem(SpecificItem specificItem,EnvironmentSource environmentSource,float bulk = 0.0f,uint amount = 1)
		: this(specificItem.GetEnumAttribute<SpecificItem,SpecificItemCategoryAttribute>().Category,environmentSource,bulk,amount,false)
	{
		_name				=	specificItem.GetEnumAttribute<SpecificItem,DescriptionAttribute>().Description;	//	TODO: create attribute
		_description		=	DefaultDescription;
		_specificItem		=	specificItem;
		_taggedBy			=	null!;
		_activeStatuses		=	[];	//	TODO: create attribute
	}
#endregion


#region Private Constructor
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
	private InventoryItem(ItemCategory category,EnvironmentSource environmentSource,float bulk,uint amount,bool occupiesSlot) : this(bulk,amount,occupiesSlot)
	{
		_itemCategory		=	category;
		_environmentSource	=	environmentSource;
	}

	private InventoryItem(float bulk,uint amount,bool occupiesSlot) : this(occupiesSlot)
	{
		_bulk	=	bulk;
		_amount	=	amount;
	}

	private InventoryItem(bool occupiesSlot) : this()
	{
		_occupiesSlot	=	occupiesSlot;
	}

	private InventoryItem()
	{
		Equipped		=	false;
		_occupiesSlot	=	true;
	}
#pragma warning restore CS8618
#endregion
}
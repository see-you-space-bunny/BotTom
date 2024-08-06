using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Systems;

namespace RoleplayingGame.SheetComponents;

internal class InventoryManager : IList<InventoryItem>
{
#region (-) Fields
	private readonly List<InventoryItem>	_items;
	private uint	_capacity;
	private ushort	_slots;
#endregion


#region (~) Properties
	internal bool OverEncumbered					=>	Count > _slots || _items.Sum(ii=>ii.Bulk) > _capacity;
	internal IEnumerable<InventoryItem>	Invisible	=>	_items.Where(ii=>!ii.OccupiesSlot&&!ii.Equipped);
	internal IEnumerable<InventoryItem>	Equipped	=>	_items.Where(ii=>ii.Equipped);
#endregion


#region (~) Indexer
	internal InventoryItem this[int index]
	{
		get	=>	_items.Where(ii=>ii.OccupiesSlot).ElementAt(index);
		set	=>	_items.Where(ii=>ii.OccupiesSlot).ToList()[index] = value;
	}
#endregion


#region (~) ContainsSpecific
	internal bool ContainsSpecificItem(SpecificItem value)	=>
		_items.Any(ii=>!ii.IsCustomItem && ii.SpecificItemType == value);
#endregion


#region (~) Modify Capacity
	internal InventoryManager ModifyCapacity(int value)
	{
		_capacity	=	(uint)Math.Max(_capacity+value,0);
		return this;
	}
#endregion


#region (~) Modify SlotCount
	internal InventoryManager ModifySlotCount(int value)
	{
		_slots		=	(ushort)Math.Max(_slots+value,0);
		return this;
	}
#endregion


#region (~) Add/Create
	internal bool TryAddItem(InventoryItem value)
	{
		if (Count < _slots)
		{
			Add(value);
			return true;
		}
		return false;
	}
#endregion


#region (~) Remove
	internal InventoryManager Sort()
	{
		_items.RemoveAll(ii=>ii==default||ii is null);
		_items.Sort((i1, i2)=>string.Compare(i1.Name,i2.Name,true));
		return this;
	}
#endregion


#region (+) ToString
	public override string ToString()	=>
		ToString(string.Empty);

	public string ToString(string indentation)
	{
		const string Empty	=	"Empty";
		StringBuilder sb	=	new ();
		ushort index		=	1;
		foreach (InventoryItem item in this)
		{
			sb.AppendFormat("{2}{1}. {0}\n",item != default ? (item?.ToString() ?? Empty) : Empty,index,indentation);
			index++;
		}
		return sb.ToString();
	}
#endregion


#region Serialization
	internal static InventoryManager Deserialize(BinaryReader reader,CharacterTracker characterTracker)
	{
		var result	=	new InventoryManager();
		result._items.Clear();
		for (ushort i=0;i<reader.ReadUInt16();i++)
		{
			result._items.Add(InventoryItem.Deserialize(reader,characterTracker));
		}
		return result;
	}

	internal void Serialize(BinaryWriter writer)
	{
		writer.Write((ushort)	_items.Count);
		foreach (InventoryItem item in _items)
		{
			item.Serialize(writer);
		}
	}
#endregion


#region IList
	public int Count => _items.Where(ii=>ii.OccupiesSlot).Count();

	public bool IsReadOnly => false;

	InventoryItem IList<InventoryItem>.this[int index] { get => this[index]; set => this[index] = value; }

	public int IndexOf(InventoryItem item)
	{
		return _items.IndexOf(item);
	}

	public void Insert(int index, InventoryItem item)
	{
		_items.Insert(index,item);
	}

	public void RemoveAt(int index)
	{
		if (_items.ElementAtOrDefault(index) == default)
			return;
		_items[index]	=	default!;
	}

	public void Add(InventoryItem item)
	{
		_items.Add(item);
	}

	public void Clear()
	{
		_items.RemoveAll(ii=>ii.OccupiesSlot);
	}

	public bool Contains(InventoryItem item)
	{
		return _items.Where(ii=>ii.OccupiesSlot).Contains(item);
	}

	public void CopyTo(InventoryItem[] array, int arrayIndex)
	{
		_items.Where(ii=>ii.OccupiesSlot).ToList().CopyTo(array,arrayIndex);
	}

	public bool Remove(InventoryItem item)
	{
		int index	=	_items.IndexOf(item);
		if (index == -1)
			return false;
		_items[index]	=	default!;
		return true;
	}

	public IEnumerator<InventoryItem> GetEnumerator()
	{
		return _items.Where(ii=>ii.OccupiesSlot).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _items.Where(ii=>ii.OccupiesSlot).GetEnumerator();
	}
#endregion


#region Constructor
	internal InventoryManager(uint capacity,ushort slots)
	{
		_capacity	=	capacity;
		_slots		=	slots;
		_items		=	[];
	}

	private InventoryManager() : this(0,1)
	{ }
#endregion


#region Static Constructor
	static InventoryManager()
	{ }
#endregion
}
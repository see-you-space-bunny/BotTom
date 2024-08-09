using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Interfaces;
using RoleplayingGame.Systems;

namespace RoleplayingGame.SheetComponents;

internal class ObjectManager<TObject> : IList<TObject>, IObjectManager
{
#region (-) Fields
	protected readonly List<TObject>	_objects;
#endregion


#region (~) Indexer
	internal virtual TObject this[int index]
	{
		get	=>	_objects.ElementAt(index);
		set	=>	_objects[index] = value;
	}
#endregion


#region (~) Sort
	internal virtual ObjectManager<TObject> Sort()
	{
		_objects.Sort();
		return this;
	}
#endregion


#region Initialize
	internal virtual void Initialize(params IObjectManager[] objectManagers) { }
#endregion


#region Serialization
	public static ObjectManager<TObject> Deserialize(BinaryReader reader)	=>	null!;
	public virtual void Serialize(BinaryWriter writer) { }
#endregion


#region IList
	public virtual int Count => _objects.Count;

	public virtual bool IsReadOnly => false;

	TObject IList<TObject>.this[int index] { get => this[index]; set => this[index] = value; }

	public virtual int IndexOf(TObject item)
	{
		return _objects.IndexOf(item);
	}

	public virtual void Insert(int index, TObject item)
	{
		_objects.Insert(index,item);
	}

	public virtual void RemoveAt(int index)
	{
		_objects.RemoveAt(index);
	}

	public virtual void Add(TObject item)
	{
		_objects.Add(item);
	}

	public virtual void Clear()
	{
		_objects.Clear();
	}

	public virtual bool Contains(TObject item)
	{
		return _objects.Contains(item);
	}

	public virtual void CopyTo(TObject[] array, int arrayIndex)
	{
		_objects.CopyTo(array,arrayIndex);
	}

	public virtual bool Remove(TObject item)
	{
		_objects.Remove(item);
		return true;
	}

	public IEnumerator<TObject> GetEnumerator()
	{
		return _objects.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _objects.GetEnumerator();
	}
#endregion


#region Constructor
	public ObjectManager()
	{
		_objects	=	[];
	}
#endregion


#region Static Constructor
	static ObjectManager()
	{ }
#endregion
}
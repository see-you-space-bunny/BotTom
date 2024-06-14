using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal class Inventory() : IBinarySerializable
{
  [DataMember]
  internal int Silver { get; set; } = 0;
  
  internal IEnumerable<InventoryItem> Weapons => Items.Where((ii)=>ii.Details is Weapon);
  internal IEnumerable<InventoryItem> Armors => Items.Where((ii)=>ii.Details is Armor);
  
  [DataMember]
  internal List<InventoryItem> Items { get; } = [];

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    Silver = reader.ReadInt16();
    for(int i = reader.ReadInt16(); i > 0; i--)
    {
      InventoryItem inventoryItem = new InventoryItem();
      (inventoryItem as IBinarySerializable).Deserialize(reader);
      Items.Add(inventoryItem);
    
    }
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((Int16)Silver);
    writer.Write((Int16)Items.Count);
    foreach(IBinarySerializable item in Items.OfType<IBinarySerializable>())
      item.Serialize(writer);
  }
  #endregion
}

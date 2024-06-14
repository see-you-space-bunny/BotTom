using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Serialization;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal class InventoryItem(IItem? details = null,int quantity = 1) : IBinarySerializable
{
  [DataMember]
  internal IItem Details = details ?? new Item();

  [DataMember]
  internal int Quantity = quantity;

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    Quantity = reader.ReadInt16();
    switch((ItemTypes)reader.ReadInt16())
    {
      case ItemTypes.Misc:
      Details = new Item();
      break;
      case ItemTypes.Weapon:
      Details = new Weapon();
      break;
      case ItemTypes.Armor:
      Details = new Armor();
      break;
      case ItemTypes.Feat:
      Details = new Feat();
      break;
    }
		Details.Deserialize(reader);
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((Int16)Quantity);
    (Details as IBinarySerializable).Serialize(writer);
  }
  #endregion
}

using FChatApi.Enums;
using RoleplayingGame.Enums;

namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class AbilityInfoAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly AbilityGroup _group;
		readonly AbilityType _type;
		readonly BBCodeColor _color;
		
		// This is a positional argument
		public AbilityInfoAttribute(AbilityGroup group,AbilityType type,BBCodeColor color = BBCodeColor.white)
		{
				this._group = group;
				this._type = type;
				this._color = color;
				
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public BBCodeColor Color
		{
				get { return this._color; }
		}
		
		public AbilityGroup Group
		{
				get { return this._group; }
		}
		
		public AbilityType Type
		{
				get { return this._type; }
		}
		
		// This is a named argument
		/* public int NamedInt { get; set; } */

		
	public static string GetEnumDescription<T>(T enumerableItem)
	{
		try
		{
			var enumType = typeof(T);
			var memberInfos = enumType.GetMember(enumerableItem!.ToString()!);
			var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
			var valueAttributes = enumValueMemberInfo!.GetCustomAttributes(typeof(ShortFormAttribute),false);
			var description = ((ShortFormAttribute)valueAttributes[0]).Description;
			return description;
		}
		catch
		{
			return (enumerableItem!.ToString()) ?? "null";
		}
	}
}
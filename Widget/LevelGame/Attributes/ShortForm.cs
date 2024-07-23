namespace LevelGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class ShortFormAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly string _value;
		
		// This is a positional argument
		public ShortFormAttribute(string description)
		{
				this._value = description;
				
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public string Description
		{
				get { return _value; }
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
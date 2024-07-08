using System.ComponentModel;

namespace FChatApi.Enums
{
	/// <summary>
	/// &gt; action <c>list</c> requests a list of api-user's ignored characters ?<br/>
	/// &gt; action <c>add</c> adds a character to api-user's ignore list<br/>
	/// &gt; action <c>remove</c> removes a character from api-user's ignore list.<br/>
	/// &gt; action <c>notify</c> notifies a character that their private (<c>PRI</c>) message was ignored.</i>
	/// </summary>
	public enum IgnoreAction
	{
		/// <summary>empty/invalid, failthrough default</summary>
		[Description("invalid option")]
		invalid = 0x00,

		/// <summary requests a list of api-user's ignored characters ?</summary>
		[Description("Requesting a copy of api-user's ignore list.")]
		list 	= 0x01,

		/// <summary>adds a character to api-user's ignore list</summary>
		[Description("Requested that {0} be added to api-user's ignore list.")]
		add 	= 0x02,

		/// <summary>removes a character from api-user's ignore list.</summary>
		[Description("Requested a removal of {0} from api-user's ignore list.")]
		remove 	= 0x03,

		/// <summary>notifies a character that their private (<c>PRI</c>) message was ignored.</summary>
		[Description("Notified {0} that their message was ignored.")]
		notify 	= 0x04,
	}
}
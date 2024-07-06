using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FChatApi.Attributes;
using FChatApi.Enums;

namespace FChatApi.Objects;

public class User
{
#region Constants (-)
	private const string MentionFormat = "[user]{0}[/user]";
#endregion

#region Basic Info (P+)
	/// <summary>the character's kinks and preferences thereof</summary>
	public Dictionary<string, KinkPreference> Kinks { get; }

	/// <summary>the character's status in chat</summary>
	public ChatStatus ChatStatus { get; set; }
	
	/// <summary>the character's status in chat</summary>
	public UserStatus UserStatus { get; set; }

	/// <summary>site memo on this character</summary>
	public string Memo { get; set; }
	
	/// <summary>character name</summary>
	public string Name { get; set; }
	
	/// <summary>that subscriber subtitle ?SPECULATION</summary>
	public string Nickname { get; set; }
#endregion


#region Profile Info (P+)
	/// <summary>all of the character's profile info</summary>
	public Dictionary<ProfileInfoField,string> ProfileInfo { get; set; }
	
	/// <summary>age / meme / commentary</summary>
	public string Age { get=>GetProfileInfo(ProfileInfoField.Age); set=>SetProfileInfo(ProfileInfoField.Age,value); }
	
	/// <summary>the character's gender</summary>
	public string Gender { get=>GetProfileInfo(ProfileInfoField.Gender); set=>SetProfileInfo(ProfileInfoField.Gender,value); }

	/// <summary>the character's sexual orientation</summary>
	public string Orientation { get=>GetProfileInfo(ProfileInfoField.Orientation); set=>SetProfileInfo(ProfileInfoField.Orientation,value); }

	/// <summary>the player's language preference</summary>
	public string LanguagePref { get=>GetProfileInfo(ProfileInfoField.LanguagePreference); set=>SetProfileInfo(ProfileInfoField.LanguagePreference,value); }

	/// <summary>race / species / meme</summary>
	public string Species { get=>GetProfileInfo(ProfileInfoField.Species); set=>SetProfileInfo(ProfileInfoField.Species,value); }
	
	/// <summary>furry / human preference</summary>
	public string FurryPref { get=>GetProfileInfo(ProfileInfoField.FurryPreference); set=>SetProfileInfo(ProfileInfoField.FurryPreference,value); }
	
	/// <summary>the character's dom/sub role</summary>
	public string Role { get=>GetProfileInfo(ProfileInfoField.DomSubRole); set=>SetProfileInfo(ProfileInfoField.DomSubRole,value); }
	
	/// <summary>the character's top/bottom position</summary>
	public string Position { get=>GetProfileInfo(ProfileInfoField.Position); set=>SetProfileInfo(ProfileInfoField.Position,value); }
#endregion


#region Custom Info (P+)
	public string Mention => string.Format(MentionFormat,Name);
#endregion


////////////////////


#region Constructors (+)
	/// <summary>
	/// depreciated constructor
	/// </summary>
	/// <param name="kinks">kinks list by preference and kinks</param>
	public User(Dictionary<KinkPreference, List<string>> kinks) : this()
	{
		Kinks = [];
		foreach((KinkPreference preference,List<string> kinksList) in kinks)
		{
			foreach(string kink in kinksList)
			{
				Kinks.TryAdd(kink,preference);
			}
		}
	}

	/// <summary>
	/// preferred constructor
	/// </summary>
	/// <param name="kinks">kinks list by kink and preference</param>
	public User(Dictionary<string,KinkPreference> kinks) : this()
	{
		Kinks = kinks;
	}

	/// <summary>
	/// basic constructor mainly for use in deserialization
	/// </summary>
	public User()
	{
		Name        = string.Empty;
		Nickname    = string.Empty;
		Memo        = string.Empty;
		Gender      = string.Empty;

		UserStatus  = UserStatus.None;
		ChatStatus  = ChatStatus.Invalid;

		Kinks ??= [];
	}
#endregion


////////////////////


#region Get Methods (+)
	/// <summary>
	/// gets a list of kinks by the user's preference thereof
	/// </summary>
	/// <param name="preference">the preference we wish to filter for</param>
	/// <returns>filtered list containing the kinks of the desired preference</returns>
	public IEnumerable<string> GetKinks(KinkPreference preference) => Kinks.Where(k=>k.Value==preference).Select(k=>k.Key);

	/// <remarks>
	/// TODO:Attributes --> when moving AttributeHandler into the Api<br/>currently returns an empty dictionary 
	/// </remarks>
	/// <summary>
	/// gets a specific profile info tab from the user's profile info
	/// </summary>
	/// <param name="tab">the tab we wish to filter for</param>
	/// <returns>filtered dictionary containing the desired tab</returns>
	public IDictionary<ProfileInfoField,string> GetProfileTab(ProfileInfoTab tab)
	{
		// TODO: filter by attribute
		//return ProfileInfo.Where(i=>i.Key.GetEnumAttribute<ProfileInfoField,InfoTabAttribute>().Tab==tab).ToDictionary();
		return new Dictionary<ProfileInfoField,string>();
	}

	/// <summary>
	/// get a specific profile info field's value
	/// </summary>
	/// <param name="info">the field we want to get</param>
	/// <returns>the profile info as a string, or string.Empty if it was not found</returns>
	public string GetProfileInfo(ProfileInfoField info)
	{
		if (ProfileInfo.TryGetValue(info,out string value))
			return value;
		return string.Empty;
	}
#endregion


#region Set Methods (-)
	/// <remarks>
	/// we should not be calling this method ourselves 99% of the time
	/// </remarks>
	/// <summary>
	/// sets a specific profile info's value
	/// </summary>
	/// <param name="info">the field we want to set</param>
	/// <param name="value">the value we want to assign to the field/param>
	private void SetProfileInfo(ProfileInfoField info,string value)
	{
		if (!ProfileInfo.TryAdd(info,value))
			ProfileInfo[info] = value;
	}
#endregion
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.Interfaces;

namespace FChatApi.Objects;

public class User : IMessageRecipient
{
#region Constants (-)
	/// <summary>user mention format</summary>
	private const string MentionFormat = "[user]{0}[/user]";

	/// <summary>user icon format</summary>
	private const string IconFormat = "[icon]{0}[/icon]";
#endregion


////////////////////////////////////////////////
///

#region Fields (-)
	/// <summary>character's userlist key</summary>
	private string _key;

	/// <summary>character name</summary>
	private string _name;
#endregion


////////////////////////////////////////////////


#region Chat Info (P+)
	/// <summary>the character's status in chat</summary>
	public ChatStatus ChatStatus { get; set; }
	
	/// <summary>the character's status in chat</summary>
	public UserStatus UserStatus { get; set; }

	/// <summary>site memo on this character</summary>
	public string Memo { get; set; }

    /// <summary>character name</summary>
    public string Name { get=>_name; set{ _name = value; _key = value.ToLowerInvariant(); } }
	
	/// <summary>that subscriber subtitle ?SPECULATION</summary>
	public string Nickname { get; set; }
#endregion


////////////////////////////////////////////////


#region Profile Info (P+)
	/// <summary>the character's kinks and preferences thereof</summary>
	public Dictionary<string, KinkPreference> Kinks { get; }

	/// <summary>all of the character's profile info</summary>
	public Dictionary<ProfileInfoField,string> ProfileInfo { get; }
	
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


////////////////////////////////////////////////


#region Custom Info (P+)
	
	/// <summary>key for UserTracker</summary>
	public string Key { get=>_key; private set => _key = value.ToLowerInvariant(); }
	
	/// <summary>a memo separate from the flist memo feature</summary>
	public string BotMemo { get; set; }

	/// <summary>the color of the user's nickname</summary>
	public BBCodeColor NickColor { get; set; }

	/// <summary>the user's pronouns</summary>
	public string Pronouns { get; set; }

	/// <summary>what privilege level does this user have</summary>
	public Privilege PrivilegeLevel { get; set; }

	/// <summary>when did this user first register</summary>
	public DateTime WhenRegistered { get; set; }

	/// <summary>is he user registered with the bot</summary>
	public bool IsRegistered => WhenRegistered > DateTime.MinValue;

	/// <summary>mentions the user in clickable form</summary>
	public string Mention => string.Format(MentionFormat,Name);

	/// <summary>inserts an icon of the user</summary>
	public string Icon => string.Format(IconFormat,Name);
#endregion


////////////////////////////////////////////////


#region Message Timers (+)
	/// <summary>the amount of miliseconds to wait before the next message may be sent</summary>
	public TimeSpan SleepInterval { get; }

	/// <summary>the next earliest point at which a message may be sent</summary>
	public DateTime Next { get; set; }

	/// <summary>sets the earliest time the next message can be sent</summary>
    void IMessageRecipient.MessageSent() => Next = DateTime.Now + SleepInterval;
#endregion


////////////////////////////////////////////////


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
		// Chat Info
		Name        = string.Empty;
		UserStatus  = UserStatus.None;
		ChatStatus  = ChatStatus.Invalid;

		// Profile Info
		Nickname    = string.Empty;
		Memo        = string.Empty;
		ProfileInfo	??= [];
		Kinks		??= [];

		// IMessageRecipient
		SleepInterval	= new TimeSpan(0,0,0,0,milliseconds: 1);

		// Custom Info
		BotMemo			= string.Empty;
		NickColor		= BBCodeColor.white;
		Pronouns		= string.Empty;
		PrivilegeLevel	= Privilege.UnregisteredUser;
		WhenRegistered	= DateTime.MinValue;
	}
#endregion


////////////////////////////////////////////////


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


////////////////////////////////////////////////


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


////////////////////////////////////////////////


#region Update (+)
	public async Task Update(User value)
	{
		// Chat Info
		UserStatus  = value.UserStatus;
		ChatStatus  = value.ChatStatus;

		// Profile Info
		Nickname    = value.Nickname;
		Memo        = value.Memo;

		Task[] tasks =
        [
            Task.Run(()=>
                {
                    Kinks.Clear();
                    foreach ((string kink,KinkPreference preference) in value.Kinks)
                    {
                        Kinks.TryAdd(kink,preference);
                    }
                }
            ),
            Task.Run(()=>
                {
                    ProfileInfo.Clear();
                    foreach ((ProfileInfoField field,string info) in value.ProfileInfo)
                    {
                        ProfileInfo.TryAdd(field,info);
                    }
                }
            ),
        ];
        await Task.WhenAll(tasks.Where(t => t != null).ToArray());
	}

#endregion

////////////////////////////////////////////////


#region Serialization (+)
	/// <summary>
	/// deserializes the object to binary
	/// </summary>
	/// <param name="reader"></param>
	/// <returns>a deserialized user object with the Custom Info and Name fields filled</returns>
	public static User Deserialize(BinaryReader reader)
	{
		return new User()
		{
			Name			=   (string)		reader.ReadString(),
			BotMemo			=   (string)		reader.ReadString(),
			NickColor		=   (BBCodeColor)	reader.ReadUInt16(),
			PrivilegeLevel	=   (Privilege)		reader.ReadUInt16(),
			WhenRegistered	=   new DateTime(
				year: reader.ReadInt32(),
				month: reader.ReadInt32(), 
				day: reader.ReadInt32()
			),
			Pronouns		=   (string)		reader.ReadString(),
		};
	}

	/// <summary>
	/// serializes the object to binary
	/// </summary>
	/// <param name="writer"></param>
	public void Serialize(BinaryWriter writer)
	{
		writer.Write((string)	Name);
		writer.Write((string)	BotMemo);
		writer.Write((ushort)	NickColor);
		writer.Write((ushort)	PrivilegeLevel);
		writer.Write((int)		WhenRegistered.Year);
		writer.Write((int)		WhenRegistered.Month);
		writer.Write((int)		WhenRegistered.Day);
		writer.Write((string)	Pronouns);
	}
#endregion
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Systems;
using FChatApi.Enums;
using Engine.Serialization;
using Engine.ModuleHost.Enums;

namespace Engine.ModuleHost.CardiApi;

public static class UserExtensions
{
    public static RegisteredUser ToRegisteredUser(this User user)
    {
        if (!ChatBot.RegisteredUsers.TryGetValue(user.Name.ToLower(),out var tempUser))
            tempUser = new RegisteredUser(user);
        tempUser.Update(user);
        return tempUser;
    }
    
    public static RegisteredUser ToRegisteredUser(this string user)
    {
        if (!ChatBot.RegisteredUsers.TryGetValue(user.ToLower(),out var tempUser))
        {
            tempUser = ApiConnection.GetUserByName(user.ToLower()).ToRegisteredUser();
        }
        else
        {
            tempUser.Update(ApiConnection.GetUserByName(user.ToLower()));
        }
        return tempUser;
    }
}

public class RegisteredUser
{
    #region Base Properties
    
    private string _name;

    /// <summary>the user's kink preferences</summary>
    public Dictionary<KinkPreference, List<string>> Kinks => _cardiApiUser!.Kinks;

    /// <summary>the character's status in chat</summary>
    public ChatStatus ChatStatus { get=>_cardiApiUser!.ChatStatus; set { _cardiApiUser!.ChatStatus=value; } }
    
    /// <summary>the character's status in chat</summary>
    public UserStatus UserStatus { get=>_cardiApiUser!.UserStatus; set { _cardiApiUser!.UserStatus=value; } }
    
    /// <summary>the character's gender</summary>
    public string Gender { get=>_cardiApiUser!.Gender; set { _cardiApiUser!.Gender=value; } }
    /**
    /// <summary></summary>
    public string GenderPreference { get; set; }

    /// <summary></summary>
    public string Role { get; set; }

    /// <summary></summary>
    public string RolePreference { get; set; }

    /// <summary></summary>
    public string Species { get; set; }
    
    /// <summary></summary>
    public string SpeciesPreference { get; set; }
    */
    /// <summary>the site-memo the bot owner has on the user</summary>
    public string Memo { get=>_cardiApiUser!.Memo; set { _cardiApiUser!.Memo=value; } }

    /// <summary>character name</summary>
    public string Name { get=>_cardiApiUser != null ? _cardiApiUser.Name : _name; set { _cardiApiUser!.Name=value; _name=value; } }
    
    /// <summary>character? name</summary>
    public string Nickname { get=>_cardiApiUser!.Nickname; set=>_cardiApiUser!.Nickname=value; }

    #endregion

    #region Expanded Properties
    
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

    ////////////////////

    private User? _cardiApiUser { get; set; }

    public bool IsLinked => _cardiApiUser is not null;

    ////////////////////

    /// <summary>collection of strings by which the user can be mentioned</summary>
    public Mention Mention => new(this);
    #endregion

    ////////////////////////////////////////

    #region Constructor
    /// <summary>
    /// creates an empty registerd user
    /// </summary>
    /// <remarks>
    /// they are created <c>ChatStatus.Offline</c> instead of <c>ChatStatus.None</c>
    /// </remarks>
    public RegisteredUser() : base()
    {
        _name = string.Empty;

        ////////////////////
        
        NickColor   = BBCodeColor.white;
        Pronouns    = string.Empty;
        BotMemo     = string.Empty;
    }

    /// <summary>
    /// registers the selected user
    /// </summary>
    /// <param name="user">the user being registered</param>
    public RegisteredUser(User user)
    {
        Update(user);
        _name = user.Name;

        ////////////////////
        
        BotMemo     = string.Empty;
        NickColor   = BBCodeColor.white;
        Pronouns    = string.Empty;
    }

    public void Update(User user)
    {
        _cardiApiUser = user;
        _name = _cardiApiUser.Name;
    }

    public static RegisteredUser Deserialize(BinaryReader reader)
    {
        return new RegisteredUser()
        {
            _name           =   (string)        reader.ReadString(),
            BotMemo         =   (string)        reader.ReadString(),
            NickColor       =   (BBCodeColor)   reader.ReadUInt16(),
            PrivilegeLevel  =   (Privilege)     reader.ReadUInt16(),
            WhenRegistered  =   new DateTime(
                year: reader.ReadInt32(),
                month: reader.ReadInt32(), 
                day: reader.ReadInt32()
            ),
            Pronouns        =   (string)        reader.ReadString(),
        };
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write((string)   _name);
        writer.Write((string)   BotMemo);
        writer.Write((ushort)   NickColor);
        writer.Write((ushort)   PrivilegeLevel);
        writer.Write((int)      WhenRegistered.Year);
        writer.Write((int)      WhenRegistered.Month);
        writer.Write((int)      WhenRegistered.Day);
        writer.Write((string)   Pronouns);
    }
    #endregion

    ////////////////////////////////////////


    

    public List<string> GetKinks(KinkPreference preference) => _cardiApiUser!.GetKinks(preference);
}
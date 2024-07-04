using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Objects;
using BinarySerialization;

namespace BotTom.CardiApi;

public static class UserExtensions
{
    public static RegisteredUser ToRegisteredUser(this User user)
    {
        return new RegisteredUser(user);
    }
}

public class RegisteredUser : User
{
    #region Base Properties
    
    /// <summary>character? name</summary>
    [FieldOrder(0)]
    public new string Name => base.Name;
    
    /// <summary>character? name</summary>
    [FieldOrder(1)]
    public new string Nickname => base.Nickname;
    #endregion

    #region Expanded Properties
    
    /// <summary>a memo separate from the flist memo feature</summary>
    [FieldOrder(2)]
    public string BotMemo { get; set; }

    /// <summary></summary>
    [FieldOrder(3)]
    public BBCodeColor NickColor { get; set; }

    /// <summary></summary>
    [FieldOrder(4)]
    public string Pronouns { get; set; }

    ////////////////////

    /// <summary>collection of strings by which the user can be mentioned</summary>
    public override Mention Mention => new(this,NickColor);
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
        ChatStatus  = ChatStatus.Offline;

        ////////////////////
        
        NickColor   = BBCodeColor.White;
        Pronouns    = string.Empty;
        BotMemo     = string.Empty;
    }

    /// <summary>
    /// registers the selected user
    /// </summary>
    /// <param name="user">the user being registered</param>
    public RegisteredUser(User user) : base(user.Kinks)
    {
        base.Name       = user.Name;
        base.Nickname   = user.Nickname;

        Memo        = user.Memo;
        Gender      = user.Gender;

        UserStatus  = user.UserStatus;
        ChatStatus  = user.ChatStatus;
        
        ////////////////////
        
        BotMemo     = string.Empty;
        NickColor   = BBCodeColor.White;
        Pronouns    = string.Empty;
    }
    #endregion

    ////////////////////////////////////////

    #region With Properties
    public RegisteredUser WithName(string value)
    {
        base.Name = value;
        return this;
    }
    public RegisteredUser WithNickname(string value)
    {
        base.Nickname = value;
        return this;
    }
    #endregion

    ////////////////////////////////////////
    
    public static async Task<RegisteredUser> DeserializeAsync(MemoryStream stream, BinarySerializer serializer) =>
        (RegisteredUser)await serializer.DeserializeAsync(stream, typeof(RegisteredUser));

    public async Task SerializeAsync(MemoryStream stream, BinarySerializer serializer) =>
        await serializer.SerializeAsync(stream, this);

}
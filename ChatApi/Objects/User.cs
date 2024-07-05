using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApi.Objects;

public class User(Dictionary<KinkPreference, List<string>> kinks)
{
    /// <summary></summary>
    public Dictionary<KinkPreference, List<string>> Kinks { get; } = kinks;

    /// <summary>the character's status in chat</summary>
    public ChatStatus ChatStatus { get; set; }
    
    /// <summary>the character's status in chat</summary>
    public UserStatus UserStatus { get; set; }
    
    /// <summary>the character's gender</summary>
    public string Gender { get; set; }
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
    /// <summary></summary>
    public string Memo { get; set; }
    
    /// <summary>character? name</summary>
    public string Name { get; set; }
    
    /// <summary>character? name</summary>
    public string Nickname { get; set; }

    #region Custom Properties
    
    /// <summary>character pronouns</summary>
    public string Pronouns { get; set; }
    public virtual Mention Mention => new(this,BBCodeColor.White);
    #endregion

    public User() : this([])
    {
        Name        = string.Empty;
        Nickname    = string.Empty;
        Memo        = string.Empty;
        Gender      = string.Empty;

        UserStatus  = UserStatus.None;
        ChatStatus  = ChatStatus.None;
    }

    public List<string> GetKinks(KinkPreference preference)
    {
        return [.. Kinks[preference]];
    }
}
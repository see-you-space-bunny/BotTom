using System;
using System.Collections.Generic;
using System.Linq;
using FChatApi.Enums;

namespace FChatApi.Objects;

public class Channel
{
    public bool AdEnabled { get; set; }

    /// <summary>the channel</summary>
    public string Name { get; set; }

    /// <summary>what is our status with the channel</summary>
    public ChannelStatus Status { get; set; }

    /// <summary>sending user</summary>
    public string Code { get; set; }

    /// <summary>(accessibility) type of channel</summary>
    public ChannelType Type { get; set; }


    /// <summary>channel moderators</summary>
    internal Dictionary<string, User> Mods { get; set; }


    /// <summary>channel users</summary>
    internal Dictionary<string, User> Users { get; set; }


    /// <summary>did this api create the channel</summary>
    public bool CreatedByApi { get; set; }


    /// <summary>channel description</summary>
    public string Description { get; set; }


    /// <summary>channel owner</summary>
    public User Owner { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="code"></param>
    /// <param name="type"></param>
    /// <param name="adenabled"></param>
    public Channel(string name, string code, ChannelType type, bool adenabled = false)
    {
        Name = name;
        Code = code;
        Status = ChannelStatus.Available;
        Type = type;
        AdEnabled = adenabled;
        Mods = [];
        Users = [];
        CreatedByApi = false;
        Description = string.Empty;
    }

    public Channel AddUser(User user)
    {
        if (!Users.TryAdd(user.Name, user))
            Console.WriteLine($"Skipping duplicate entry: {user.Name}");
        return this;
    }

    public Channel RemoveUser(User user)
    {
        Users.Remove(user.Name);
        return this;
    }

    public Channel AddMod(User user)
    {
        if (!Users.TryAdd(user.Name, user))
            Console.WriteLine($"Skipping duplicate mod entry: {user.Name}");
        return this;
    }

    public Channel RemoveMod(User user)
    {
        Mods.Remove(user.Name);
        return this;
    }
    ////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    private Channel()
    {

    }
}
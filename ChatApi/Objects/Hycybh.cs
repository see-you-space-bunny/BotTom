namespace ChatApi;

public enum Hycybh
{
    /// <summary>user is typing</summary>
    TPN,

    /// <summary>channel message sent/received</summary>
    MSG,

    /// <summary>connected or requesting chat connection</summary>
    IDN,

    /// <summary>private message sent/received</summary>
    PRI,

    /// <summary>public channel list received </summary>
    CHA,

    /// <summary>private channel list received</summary>
    ORS,

    /// <summary>joined or requesting to join a channel</summary>
    JCH,

    /// <summary>left or requesting to leave a channel</summary>
    LCH,

    /// <summary>a ping</summary>
    PIN,

    /// <summary>requests status set</summary>
    STA,

    /// <summary>advertisement</summary>
    LRP,

    /// <summary>create(d) a private, invite-only channel</summary>
    CCR,

    /// <summary>chat system variables</summary>
    VAR,

    /// <summary>server hello command</summary>
    HLO,

    /// <summary>returns number of connected users</summary>
    CON,

    /// <summary>friends list</summary>
    FRL,

    /// <summary>ignore list</summary>
    IGN,

    /// <summary>chat ops list</summary>
    ADL,

    /// <summary>character list and online status</summary>
    LIS,

    /// <summary>a user connected</summary>
    NLN,

    /// <summary>a user disconnected</summary>
    FLN,

    /// <summary>list of channel ops</summary>
    COL,

    /// <summary>initial channel data</summary>
    ICH,

    /// <summary>channel's description has updated</summary>
    CDS,

    /// <summary>basic error response</summary>
    ERR,

    /// <summary>invite user to channel</summary>
    CIU,

    /// <summary>kicks a user</summary>
    CKU,

    /// <summary>bans a user</summary>
    CBU,

    /// <summary>promotes to channel op</summary>
    COA,

    /// <summary>demotes from channel op to user</summary>
    COR,

    /// <summary>time a user out</summary>
    CTU,

    /// <summary>unbans a user</summary>
    CUB
}

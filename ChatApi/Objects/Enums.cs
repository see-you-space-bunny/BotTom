namespace ChatApi.Objects;

public enum ChannelStatus
{
    Kicked,
    Joined,
    Left,
    Banned,
    Invited,
    Pending,
    Available,
    Unknown,
    All,
    AllValid,
    Creating,
    Created,
}

public enum ChannelType
{
    Public,
    Private,
    All,
}

public enum MessageType
{
    Whisper,
    Advertisement,
    Basic,
    Yell,
}

public enum ChatStatus
{
    Looking,
    Online,
    Busy,
    DND,
    Idle,
    Away,
    None,
    Offline,
    Connected,
    Crown,
}

public enum UserRoomStatus
{
    User,
    Moderator,
    Banned,
    Kicked,
    Timeout,
}

public enum UserStatus
{
    Online,
    Ignored,
    Blocked,
    Friended,
    Married,
    None,
}

public enum KinkPreference
{
    Favorite,
    Yes,
    Maybe,
    No,
}

public enum BBCodeColor
{
    Black,
    Blue,
    Cyan,
    Gray,
    Green,
    Orange,
    Pink,
    Purple,
    White,
    Yellow,
}

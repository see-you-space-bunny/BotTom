using System;

namespace FChatApi.Interfaces;

public interface IMessageRecipient
{
	internal TimeSpan SleepInterval { get; }
	internal DateTime Next { get; set; }
    internal void MessageSent();
}

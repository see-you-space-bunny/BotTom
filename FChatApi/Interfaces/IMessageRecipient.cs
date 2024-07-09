using System;

namespace FChatApi.Interfaces;

public interface IMessageRecipient
{
	internal string Name { get; }
	internal TimeSpan SleepInterval { get; }
	internal DateTime Next { get; set; }
    internal void MessageSent();
}

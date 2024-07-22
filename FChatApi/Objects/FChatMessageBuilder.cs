using System.Collections.Generic;
using System.Text;
using FChatApi.Enums;
using FChatApi.Core;
using System;
using FChatApi.Attributes;
using FChatApi.Interfaces;

namespace FChatApi.Objects;

public class FChatMessageBuilder()
{
#region MessageRecipient
	/// <summary>
	/// The message's Recipient as an object that can host a message Queue.<br/>
	/// Throws <c>InvalidOperationException</c> if referenced while builder is incomplete.
	/// </summary>
	public IMessageRecipient MessageRecipient => 
		HasRecipient ? 
			((MessageType == FChatMessageType.Whisper) || (Channel is null) ? Recipient : Channel) :
			throw new InvalidOperationException("This message has no valid MessageRecipient.");

	/// <summary>
	/// Indicates whether or not the message has a valid Recipient.
	/// </summary>
	public bool HasRecipient =>
		!(Recipient is null || string.IsNullOrWhiteSpace(Recipient.Name)) || !(Channel is null || string.IsNullOrWhiteSpace(Channel.Code));
#endregion


////////////////////////////////////////////////


#region Author
	User Author;
	public FChatMessageBuilder WithAuthor(User value)
	{
		Author = value;
		return this;
	}
	public FChatMessageBuilder WithAuthor(string value)
	{
		if (!ApiConnection.Users.TrySingleByName(value, out User result))
			throw new KeyNotFoundException();
		Author = result;
		return this;
	}
#endregion


////////////////////////////////////////////////


#region Recipient
	User Recipient;
	public FChatMessageBuilder WithRecipient(User value)
	{
		Recipient = value;
		return this;
	}
	public FChatMessageBuilder WithRecipient(string value)
	{
		if (!ApiConnection.Users.TrySingleByName(value, out User result))
			throw new KeyNotFoundException();
		Recipient = result;
		return this;
	}
#endregion


////////////////////////////////////////////////


#region Channel
	Channel Channel;
	public FChatMessageBuilder WithChannel(Channel value)
	{
		Channel = value;
		return this;
	}
	public FChatMessageBuilder WithChannel(string value)
	{
		Channel = ApiConnection.Channels.SingleByNameOrCode(value);
		return this;
	}
#endregion


////////////////////////////////////////////////


#region MessageType
	FChatMessageType MessageType = FChatMessageType.Invalid;
	public FChatMessageBuilder WithMessageType(FChatMessageType value)
	{
		MessageType = value;
		return this;
	}

	public FChatMessageBuilder WithMessageType(MessageCode value)
	{
		MessageType = value switch {
			MessageCode.MSG	=>	FChatMessageType.Basic,
			MessageCode.LRP	=>	FChatMessageType.Advertisement,
			MessageCode.PRI	=>	FChatMessageType.Whisper,
			MessageCode.STA	=>	FChatMessageType.Status,
			_				=>	FChatMessageType.Invalid
		};
		return this;
	}
#endregion


////////////////////////////////////////////////


#region Message
	string Message;
	public FChatMessageBuilder WithMessage(string value)
	{
		Message = value;
		return this;
	}
#endregion


////////////////////////////////////////////////


#region Message
	ChatStatus Status = ChatStatus.Invalid;
	public FChatMessageBuilder WithStatus(ChatStatus value)
	{
		Status = value;
		return this;
	}
#endregion


////////////////////////////////////////////////


#region Build Method
	public FChatMessage Build()
	{
		ValidateAuthor();

		ValidateMessageType();
		
		StringBuilder messageStringBuilder = new();

		ValidateMessageBody();

		if (!string.IsNullOrWhiteSpace(Mention))
			messageStringBuilder.Append(Mention).Append(' ');

		messageStringBuilder.Append(System.Web.HttpUtility.JavaScriptStringEncode(Message));
		
		if (Recipient is not null)
			(Recipient as IMessageRecipient).MessageSent();

		if (Channel is not null)
			(Channel as IMessageRecipient).MessageSent();

		return new FChatMessage(MessageType,Author,Recipient,Channel,messageStringBuilder.ToString(),Status);
	}
#endregion


////////////////////////////////////////////////


#region Build Helpers
	/// <summary>
	/// validates the presence of an <c>Author</c>
	/// </summary>
	private void ValidateAuthor()
	{
		if (Author is null || string.IsNullOrWhiteSpace(Author.Name))
			throw new InvalidOperationException("Attempting to build an unsigned (no Author) message. A message should always have an Author!");
	}

	/// <summary>
	/// validates the <c>MessageType</c> and <c>Channel</c> fields<br/>
	/// Sets <c>MessageType</c> to <c>FChatMessageType.Whisper</c> if <c>Channel</c> is <c>null</c>
	/// </summary>
	private void ValidateMessageType()
	{
		if (Status != ChatStatus.Invalid)
		{
			MessageType = FChatMessageType.Status;
		}
		else if (Channel is not null)
		{
			if (!Channel.AdEnabled && MessageType == FChatMessageType.Advertisement)
				throw new InvalidOperationException($"Attempting to post an ad in {Channel.Name} ({Channel.Code}), but that channel doesn't support it.");

			MessageType = FChatMessageType.Basic;
		}
		else if (Recipient is not null)
		{
			MessageType = FChatMessageType.Whisper;
		}
		else if (!HasRecipient)
			throw new InvalidOperationException("The message you are trying to build has no valid MessageRecipient.");
		else if (MessageType == FChatMessageType.Invalid)
			throw new InvalidOperationException($"Attempted to send a message with an invalid message type or failed to provide data required to build a {MessageType}-type message.");
	}
	
	/// <remarks>
	/// this must be run AFTER <c>ValidateMessageType</c>, as until then any non-explicitly set <c>MessageType</c> will default to <c>FChatMessageType.Invalid</c>
	/// </remarks>
	/// <summary>
	/// validates <c>Message</c> minimum and maximum length
	/// </summary>
	private void ValidateMessageBody()
	{
		if (string.IsNullOrWhiteSpace(Message))
			throw new InvalidOperationException("Attempting to build an empty message.");

		int maxLength = AttributeExtensions.GetEnumAttribute<FChatMessageType,MaximumLengthAttribute>(MessageType).Length;

		if (Message.Length > maxLength)
			throw new InvalidOperationException($"Max message length of: {maxLength} characters.");
	}
#endregion


////////////////////////////////////////////////


#region Optional Methods
	string Mention;
	public FChatMessageBuilder WithMention(string value)
	{
		Mention = value;
		return this;
	}
	public FChatMessageBuilder WithoutMention()
	{
		Mention = string.Empty;
		return this;
	}
#endregion
}
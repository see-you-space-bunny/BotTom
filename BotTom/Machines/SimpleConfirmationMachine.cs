namespace BotTom.Machines;

internal class SimpleConfirmationMachine : IStateMachine
{
	internal enum State
	{
		[Description("Inactive: The SimpleConfirmationMachine has not yet been initialized.")]
		Inactive,
		[Description("Pending Confirmation: The SimpleConfirmationMachine is awaiting confirmation.")]
		PendingConfirmation,
		[Description("Confirmed: The SimpleConfirmationMachine has been successfully confirmed.")]
		Confirmed,
		[Description("Expired: The SimpleConfirmationMachine expiry timer has timed out.")]
		Expired,
		[Description("Exited: The SimpleConfirmationMachine has exited.")]
		Exited,
		[Description("Cancelled: The SimpleConfirmationMachine has exited without being confirmed.")]
		Cancelled,
		[Description("Event Error: The event supplied to SimpleConfirmationMachine has no legal effect.")]
		EventError,
	}
	internal enum Event
	{
		Initiate, // Inactive --> PendingConfirmation
		Confirm, // PendingConfirmation --> Confirmed
		Reset, // ANY --> Inactive
		Undo, // Current --> Previous
		Exit, // ANY --> Exited
		Cancel, // ANY --> Cancelled
	}

	internal enum ValidTarget
	{
		DeleteClockAll,
		DeleteClockSingle,
		DeleteClockGroup,
	}

	internal ValidTarget Target;
	internal string TargetValue;
	internal string? Passphrase;
	internal ulong UserId;
	internal DateTime TimeInitiated;
	internal DateTime ExpireTime;
	private bool IsExpired => DateTime.Compare(ExpireTime,DateTime.Now) < 0;
	internal Event PreviousEvent;
	internal State PreviousState;
	internal State CurrentState;
	internal string InfoMessage => GetStateInfo(CurrentState);
	internal string StateInfo => GetStateInfo(CurrentState);
	internal string PreviousStateInfo => GetStateInfo(PreviousState);
	private readonly State[] EndStates = [State.Confirmed,State.Exited];

	internal SimpleConfirmationMachine(ulong userId,ValidTarget target,string value,string? passphrase=null) : this(userId,target,value,passphrase,new TimeSpan(hours: 0,minutes: 0,seconds: 90))
	{ }

	internal SimpleConfirmationMachine(ulong userId,ValidTarget target,string value,string? passphrase,TimeSpan expiresIn)
	{
		UserId = userId;
		Target = target;
		TargetValue = value;
		Passphrase = passphrase;
		PreviousState = State.Inactive;
		CurrentState  = State.Inactive;
		TimeInitiated = DateTime.Now;
		ExpireTime    = TimeInitiated.Add(expiresIn);
	}

	internal void AdvanceState(Event @event)
	{
		var previousState = PreviousState;
		PreviousState = CurrentState;

		if (IsExpired)
		{
			CurrentState  = State.Expired;
			return;
		}

		switch(@event)
		{
			case Event.Initiate when CurrentState==State.Inactive:
				CurrentState = State.PendingConfirmation;
				break;

			case Event.Confirm when CurrentState==State.PendingConfirmation:
				CurrentState = State.Confirmed;
				break;

			case Event.Exit:
				CurrentState = State.Exited;
				break;

			case Event.Cancel:
				CurrentState = State.Cancelled;
				break;

			case Event.Reset:
				CurrentState = State.Inactive;
				break;

			case Event.Undo:
				CurrentState = previousState;
				break;
			
			default:
				CurrentState = State.EventError;
				break;
		}
	}

	private string GetStateInfo(State state)
	{
		try
		{
			var enumType = typeof(State);
			var memberInfos = enumType.GetMember(state.ToString());
			var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
			var valueAttributes = enumValueMemberInfo!.GetCustomAttributes(typeof(DescriptionAttribute),false);
			var description = ((DescriptionAttribute)valueAttributes[0]).Description;
			return description;
		}
		catch
		{
			return state.ToString();
		}
	}

	bool IStateMachine.AtTerminalStage => EndStates.Contains(CurrentState);
	bool IStateMachine.IsExpired => IsExpired;
	
}
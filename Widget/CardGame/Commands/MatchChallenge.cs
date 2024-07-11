using System.ComponentModel;

using FChatApi.Objects;

using CardGame.Enums;
using CardGame.Interfaces;
using CardGame.MatchEntities;
using CardGame.PersistentEntities;

namespace CardGame.Commands;


public class MatchChallenge : ICommandIO<User>
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

		[Description("Event Error: The event supplied to SimpleConfirmationMachine has no legal effect.")]
		EventError,
	}
	internal enum Event
	{
		Initiate, // Inactive --> PendingConfirmation
		Confirm, // PendingConfirmation --> Confirmed
		Undo, // Current --> Previous
		Cancel, // ANY --> Exited
	}

	internal string? Passphrase;
	internal MatchPlayer Player1;
	internal MatchPlayer? Player2;
	internal User Challenger;
	internal User Target;
	internal DateTime TimeInitiated;
	internal DateTime ExpireTime;
	internal bool IsExpired => DateTime.Compare(ExpireTime,DateTime.Now) < 0;
	internal Event PreviousEvent;
	internal State PreviousState;
	internal State CurrentState;
	private readonly State[] EndStates = [State.Confirmed,State.Exited,State.Expired];

	/**
	internal string InfoMessage => GetStateInfo(CurrentState);
	internal string StateInfo => GetStateInfo(CurrentState);
	internal string PreviousStateInfo => GetStateInfo(PreviousState);
	*/

	internal MatchChallenge(User challenger,MatchPlayer player1,User player2,string? passphrase=null)
		: this(challenger,player1,player2,passphrase,new TimeSpan(hours: 0,minutes: 5,seconds: 0))
	{ }

	internal MatchChallenge(User challenger,MatchPlayer player1,User target,string? passphrase,TimeSpan expiresIn)
	{
		Challenger      = challenger;
		Player1         = player1;
		Target          = target;
		Passphrase      = passphrase;
		PreviousState   = State.Inactive;
		CurrentState    = State.Inactive;
		ExpireTime      = TimeInitiated.Add(expiresIn);
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
				TimeInitiated = DateTime.Now;
				break;

			case Event.Confirm when CurrentState==State.PendingConfirmation:
				CurrentState = State.Confirmed;
				break;

			case Event.Cancel:
				CurrentState = State.Exited;
				break;

			case Event.Undo:
				CurrentState = previousState;
				break;
			
			default:
				CurrentState = State.EventError;
				break;
		}
	}

	internal bool AtTerminalStage => EndStates.Contains(CurrentState);

	User ICommandIO<User>.AlternateKey => Target;
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Widget.CardGame.Enums;
using Widget.CardGame.Interfaces;
using Widget.CardGame.MatchEntities;
using Widget.CardGame.PersistentEntities;
using ChatApi;

namespace Widget.CardGame.Commands;


internal class MatchChallenge : ICommandIO<string>, ICommand
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

  internal string? Passphrase;
  internal MatchPlayer Player1;
  internal MatchPlayer? Player2;
  internal PlayerCharacter Target;
  internal DateTime TimeInitiated;
  internal DateTime ExpireTime;
  internal bool IsExpired => DateTime.Compare(ExpireTime,DateTime.Now) < 0;
  internal Event PreviousEvent;
  internal State PreviousState;
  internal State CurrentState;
  private readonly State[] EndStates = [State.Confirmed,State.Exited];

  /**
  internal string InfoMessage => GetStateInfo(CurrentState);
  internal string StateInfo => GetStateInfo(CurrentState);
  internal string PreviousStateInfo => GetStateInfo(PreviousState);
  */

  internal MatchChallenge(MatchPlayer player1,PlayerCharacter player2,string? passphrase=null)
    : this(player1,player2,passphrase,new TimeSpan(hours: 0,minutes: 5,seconds: 0))
  { }

  internal MatchChallenge(MatchPlayer player1,PlayerCharacter target,string? passphrase,TimeSpan expiresIn)
  {
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

  internal bool AtTerminalStage => EndStates.Contains(CurrentState);

  internal void AcceptWithDeckArchetype(CharacterStat stat1, CharacterStat? stat2) => Player2 = Target.CreateMatchPlayer(stat1,stat2);

  void ICommand.ExecuteCommand()
  {
    if (CurrentState == State.Confirmed)
    {
      if (Player2 is null)
        throw new ArgumentNullException(nameof(Player2));

      TournamentOrganiser.OngoingMatches.Add(new BoardState(Player1,Player2));
      AdvanceState(Event.Exit);
    }
  }

  string ICommandIO<string>.AlternateKey => Target.Identity.ToLower();
}
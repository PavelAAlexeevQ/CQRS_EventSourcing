namespace CQRS_EventSourcing.Events.Interfaces;

//event for a substance amount modification  
public interface IModifySubstanceAmountEvent : IEvent
{
    // used (negative) or added (positive) amount of substance
    int Amount { get; }
    // date of the action
    DateTime Date { get; }
}
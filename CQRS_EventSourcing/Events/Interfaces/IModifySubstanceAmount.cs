namespace CQRS_EventSourcing.Events.Interfaces;

public interface IModifySubstanceAmountEvent : IEvent
{
    int Amount { get; }
    DateTime Date { get; }
}
namespace CQRS_EventSourcing.Events.Interfaces;

// generic event
public interface IEvent
{
    Guid EventGuid { get; }
}
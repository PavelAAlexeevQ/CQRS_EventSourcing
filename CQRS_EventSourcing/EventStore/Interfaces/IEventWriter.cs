using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventStore.Interfaces;

public interface IEventWriter
{
    void AddEvent(IEvent e);
}
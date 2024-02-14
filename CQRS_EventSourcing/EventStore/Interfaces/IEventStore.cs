using System.Collections.ObjectModel;

using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventStore.Interfaces;


// event storage
public interface IEventStore
{
    // append a new event
    void AddEvent(IEvent e);
}
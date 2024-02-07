using System.Collections.ObjectModel;

using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventStore.Interfaces;


// event storage
public interface IEventStore
{
    // raw collection of Events
    ReadOnlyCollection<IEvent> EventList { get; }
    
    // append a new event
    void AddEvent(IEvent e);
}
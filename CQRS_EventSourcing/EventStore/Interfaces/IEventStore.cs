using System.Collections.ObjectModel;

using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventStore.Interfaces;

public interface IEventStore
{
    ReadOnlyCollection<IEvent> EventList { get; }
    
    void AddEvent(IEvent e);
}
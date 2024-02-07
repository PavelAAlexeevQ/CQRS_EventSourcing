using System.Collections.ObjectModel;

using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventStore.Interfaces;

public interface IEventReader
{
    ReadOnlyCollection<IEvent> EventList { get; }
}
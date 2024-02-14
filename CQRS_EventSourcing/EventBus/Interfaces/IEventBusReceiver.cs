using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventBus.Interfaces;


// Event Bus model, receiver

public interface IEventBusReceiver
{
// get all events by Query Service
    IAsyncEnumerable<IEvent> EventsReceiver();
    List<IEvent> GetAllEvents();
}
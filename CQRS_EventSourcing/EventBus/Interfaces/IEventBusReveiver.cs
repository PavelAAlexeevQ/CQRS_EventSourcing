using System.Collections.Generic;

using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventBus.Interfaces;

public delegate void EventHandlerWithArg(object? sender, EventReceivedArg e);

// Event Bus model, receiver

public interface IEventBusReceiver
{
// get all events by Query Service
    Queue<IEvent> GetAllEvents();

    //event to which Query Service should subscribe to get events
    event EventHandlerWithArg EventReceived;
}
public class EventReceivedArg : EventArgs
{
    public EventReceivedArg(IEvent ev)
    {
        EventVal = ev;
    }
    public IEvent EventVal { get; }
} 
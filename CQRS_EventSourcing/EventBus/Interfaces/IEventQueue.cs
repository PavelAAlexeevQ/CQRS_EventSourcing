using System.Collections.Generic;

using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventBus.Interfaces;

public delegate void EventHandlerWithArg(object? sender, EventReceivedArg e);
public interface IEventQueue
{ 
    void SendEvent(IEvent e);

    Queue<IEvent> GetAllEvents();
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
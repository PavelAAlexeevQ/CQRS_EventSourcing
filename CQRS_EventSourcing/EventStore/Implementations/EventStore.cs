using System.Collections.ObjectModel;
using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventStore.Implementations;

public class EventStore : IEventReader, IEventWriter
{
    private readonly IEventQueue _eventQueue;
    private readonly List<IEvent> _eventStore = new List<IEvent>();
    
    public EventStore(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
    }


    public ReadOnlyCollection<IEvent> EventList
    {
        get => _eventStore.AsReadOnly();
    }

    public void AddEvent(IEvent e)
    {
        _eventStore.Add(e);
        SendEvent(e);
    }
    
    protected void SendEvent(IEvent e)
    {
        _eventQueue.SendEvent(e);
    }

}
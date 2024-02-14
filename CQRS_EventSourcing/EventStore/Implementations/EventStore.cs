using System.Collections.ObjectModel;
using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventStore.Implementations;

public class EventStore : IEventStore
{
    private readonly IEventBusSender _eventBus;
    private readonly List<IEvent> _eventStore = new List<IEvent>();
    
    public EventStore(IEventBusSender eventBus)
    {
        _eventBus = eventBus;
    }

    
    public void AddEvent(IEvent e)
    {
        _eventStore.Add(e);
        SendEvent(e);
    }
    
    private void SendEvent(IEvent e)
    {
        _eventBus.SendEvent(e);
    }

}
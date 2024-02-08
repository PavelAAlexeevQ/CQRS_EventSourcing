using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventBus.Interfaces;

// Event Bus model, sender
public interface IEventBusSender
{ 
    // send event to the bus by Command Service 
    void SendEvent(IEvent e);
}
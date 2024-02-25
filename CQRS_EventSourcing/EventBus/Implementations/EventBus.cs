using System.Threading.Channels;
using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventBus.Implementations;

public class EventBusImpl : IEventBusSender, IEventBusReceiver
{
    private readonly Queue<IEvent> _eventQueue;

    private readonly Channel<IEvent> _channel;
    
    public EventBusImpl()
    {
        _eventQueue = new Queue<IEvent>();
        _channel = Channel.CreateUnbounded<IEvent>(options: new UnboundedChannelOptions()
        {
            SingleReader = false,
            SingleWriter = true,
        } );
    }
    
    public void SendEvent(IEvent e)
    {
        _eventQueue.Enqueue(e);
        _channel.Writer.WriteAsync(e);
    }
    
    public IAsyncEnumerable<IEvent> EventsReceiver()
    {
        return _channel.Reader.ReadAllAsync();
    }
    
    public List<IEvent> GetAllEvents()
    {
        return new List<IEvent>(_eventQueue);
    }
}
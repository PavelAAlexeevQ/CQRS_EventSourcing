using System.Threading.Channels;
using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.EventBus.Implementations;

public class EventBus : IEventBusSender, IEventBusReceiver
{
    private readonly Queue<IEvent> _eventQueue;

    private readonly Channel<IEvent> _channel;
    
    public EventBus()
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
        _channel.Writer.WriteAsync(e);
    }
    
    public IAsyncEnumerable<IEvent> GetEvents()
    {
        return _channel.Reader.ReadAllAsync();
    }

    public Queue<IEvent> GetAllEvents()
    {
        return _eventQueue;
    }
}
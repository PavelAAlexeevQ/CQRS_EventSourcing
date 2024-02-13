using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.Events.Interfaces;
using CQRS_EventSourcing.Services.QueryService.Interfaces;

namespace CQRS_EventSourcing.Services.QueryService.Implementations;

public class QueryService : IQueryService
{
    private readonly IEventBusReceiver _eventBus;
    private readonly Queue<IEvent> _events;
    private readonly IAsyncEnumerable<IEvent> _eventsReceiver;
    private readonly CancellationToken _cancelReciever;
    
    //materialized view with filtered IModifySubstanceAmountEvent-s
    private readonly List<IModifySubstanceAmountEvent> _substanceAmountEvents;
    public QueryService(IEventBusReceiver eventBus)
    {
        _eventBus = eventBus;
        _eventsReceiver = _eventBus.GetEvents();
        _events = new Queue<IEvent>();
        _substanceAmountEvents = new List<IModifySubstanceAmountEvent>();
        EventsReceiverProcedure();
    }

    private void EventsReceiverProcedure()
    {
        var receiverTask = Task.Run(async () =>
        {
            await foreach (var receivedEvent in _eventsReceiver)
            {
                lock (_events)
                {
                    _events.Enqueue(receivedEvent);                    
                }

                var modifySubstanceAmountEvent = receivedEvent as IModifySubstanceAmountEvent;
                if (modifySubstanceAmountEvent != null)
                {
                    lock (_substanceAmountEvents)
                    {
                        _substanceAmountEvents.Add(modifySubstanceAmountEvent);
                    }
                }
            }
        });
    }
    
    public int GetLatestAmount()
    {
        lock (_substanceAmountEvents)
        {
            return _substanceAmountEvents.Sum(x => x.Amount);
        }
    }

    public int GetPrecessedAmountForPeriod(DateTime from, DateTime to)
    {
        lock (_substanceAmountEvents)
        {
            return _substanceAmountEvents.Where(x => x.Date >= from && x.Date <= to)
                .Where(x => x.Amount < 0)
                .Sum(x => x.Amount);
        }
    }
}
using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.Events.Interfaces;
using CQRS_EventSourcing.Services.QueryService.Interfaces;

namespace CQRS_EventSourcing.Services.QueryService.Implementations;

public class QueryService : IQueryService
{
    // Query Service depends on EventBus service to receive messages
    private readonly IEventBusReceiver _eventBus;
    
    //All received events
    private readonly List<IEvent> _events;
    //materialized view with filtered IModifySubstanceAmountEvent-s
    private readonly List<IModifySubstanceAmountEvent> _substanceAmountEvents;

    public QueryService(IEventBusReceiver eventBus)
    {
        _eventBus = eventBus;
        //catch up all existing events from EventBus
        _events = _eventBus.GetAllEvents();

        // subscribe for new events
        var eventsReceiver = _eventBus.EventsReceiver();
        
        // materialize view with necessary event types
        _substanceAmountEvents = _events
            .Select(x => x as IModifySubstanceAmountEvent)
            .Where(x => x != null)
            .ToList();
        
        //start incoming events processing
        EventsReceiverProcedure(eventsReceiver);
    }

    private void EventsReceiverProcedure(IAsyncEnumerable<IEvent> eventsReceiver)
    {
        var receiverTask = Task.Run(async () =>
        {
            await foreach (var receivedEvent in eventsReceiver)
            {
                lock (_events)
                {
                    _events.Add(receivedEvent);                    
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
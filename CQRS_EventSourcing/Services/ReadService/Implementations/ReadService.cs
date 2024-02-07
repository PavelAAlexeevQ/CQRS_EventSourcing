using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.Events.Interfaces;
using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.WriteService.Interfaces;

using CQRS_EventSourcing.ReadService.Interfaces;

namespace CQRS_EventSourcing.ReadService.Implementations;


public class ReadService : IReadService
{
    private IEventQueue _eventQueue;
    private Queue<IEvent> _events;
    private List<IModifySubstanceAmountEvent> _substanceAmountEvents;
    public ReadService(IEventQueue eventQueue)
    {
        _eventQueue = eventQueue;
        eventQueue.EventReceived += OnEventReceived;
        _events = eventQueue.GetAllEvents();
        _substanceAmountEvents = _events.Where(x => x is IModifySubstanceAmountEvent)
            .Select(x => (IModifySubstanceAmountEvent)x).ToList();
    }

    private void OnEventReceived(object? sender, EventReceivedArg arg)
    {
        _events.Enqueue(arg.EventVal);
        if (arg.EventVal is IModifySubstanceAmountEvent)
        {
            _substanceAmountEvents.Add((IModifySubstanceAmountEvent)arg.EventVal);
        }
    }
    
    public int GetLatestAmount()
    {
        return _substanceAmountEvents.Sum(x => x.Amount);
    }

    public int GetPrecessedAmountForPeriod(DateTime from, DateTime to)
    {
        return _substanceAmountEvents.Where(x => x.Date >= from && x.Date <= to)
            .Where(x => x.Amount < 0)
            .Sum(x => x.Amount);
    }
    
    
}
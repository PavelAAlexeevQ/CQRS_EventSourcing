using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.Events.Interfaces;

using CQRS_EventSourcing.Services.ReadService.Interfaces;

namespace CQRS_EventSourcing.Services.ReadService.Implementations;

public class ReadService : IReadService
{
    private readonly IEventBusReceiver _eventBus;
    private readonly Queue<IEvent> _events;
    
    //materialized view with filtered IModifySubstanceAmountEvent-s
    private readonly List<IModifySubstanceAmountEvent> _substanceAmountEvents;
    public ReadService(IEventBusReceiver eventBus)
    {
        _eventBus = eventBus;
        _eventBus.EventReceived += OnEventReceived;
        _events = _eventBus.GetAllEvents();
        _substanceAmountEvents = _events.OfType<IModifySubstanceAmountEvent>().ToList();
    }

    private void OnEventReceived(object? sender, EventReceivedArg arg)
    {
        _events.Enqueue(arg.EventVal);
        var modifySubstanceAmountEvent = arg.EventVal as IModifySubstanceAmountEvent;
        if (modifySubstanceAmountEvent != null)
        {
            _substanceAmountEvents.Add(modifySubstanceAmountEvent);
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
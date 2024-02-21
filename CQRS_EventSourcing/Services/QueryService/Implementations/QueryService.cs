using CQRS_EventSourcing.DomainModels;
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
    //materialized view with filtered IModifyReagentAmountEvent-s
    private readonly List<IModifyReagentAmountEvent> _reagentAmountEvents;

    //"streams" for different equipment types  
    private readonly Dictionary<EquipmentTypes, List<IModifyReagentAmountEvent>> _reagentAmountEventsByEquipmentTypes;

    //precalculated results for different equipment types  
    private readonly Dictionary<EquipmentTypes, int> _reagentAmountByEquipmentTypes;

    
    public QueryService(IEventBusReceiver eventBus)
    {
        _eventBus = eventBus;
        //catch up all existing events from EventBus
        _events = _eventBus.GetAllEvents();

        // subscribe for new events
        var eventsReceiver = _eventBus.EventsReceiver();
        
        // materialize view with necessary event types
        _reagentAmountEvents = _events
            .Select(x => x as IModifyReagentAmountEvent)
            .Where(x => x != null)
            .ToList();

        _reagentAmountEventsByEquipmentTypes = new Dictionary<EquipmentTypes, List<IModifyReagentAmountEvent>>();
        _reagentAmountByEquipmentTypes = new Dictionary<EquipmentTypes, int>();
        foreach (var e in _reagentAmountEvents)
        {
            if(!_reagentAmountEventsByEquipmentTypes.ContainsKey((EquipmentTypes)e.EquipmentNum))
            {
                _reagentAmountEventsByEquipmentTypes[(EquipmentTypes)e.EquipmentNum] =
                    new List<IModifyReagentAmountEvent>();
            }
            _reagentAmountEventsByEquipmentTypes[(EquipmentTypes)e.EquipmentNum].Add(e);

            if(!_reagentAmountByEquipmentTypes.ContainsKey((EquipmentTypes)e.EquipmentNum))
            {
                _reagentAmountByEquipmentTypes[(EquipmentTypes)e.EquipmentNum] = 0;
            }
            _reagentAmountByEquipmentTypes[(EquipmentTypes)e.EquipmentNum] += e.Amount;
        }
            
        //start incoming events processing
        EventsReceiverProcedure(eventsReceiver);
    }

    private void EventsReceiverProcedure(IAsyncEnumerable<IEvent> eventsReceiver)
    {
        var receiverTask = Task.Run(async () =>
        {
            await foreach (var e in eventsReceiver)
            {
                lock (_events)
                {
                    _events.Add(e);
                }

                var modifyReagentAmountEvent = e as IModifyReagentAmountEvent;
                if (modifyReagentAmountEvent == null)
                    continue;

                lock (_reagentAmountEvents)
                {
                    _reagentAmountEvents.Add(modifyReagentAmountEvent);
                }

                lock (_reagentAmountEventsByEquipmentTypes)
                {
                    if (!_reagentAmountEventsByEquipmentTypes.ContainsKey(
                            (EquipmentTypes)modifyReagentAmountEvent.EquipmentNum))
                    {
                        _reagentAmountEventsByEquipmentTypes[(EquipmentTypes)modifyReagentAmountEvent.EquipmentNum] =
                            new List<IModifyReagentAmountEvent>();
                    }

                    _reagentAmountEventsByEquipmentTypes[(EquipmentTypes)modifyReagentAmountEvent.EquipmentNum]
                        .Add(modifyReagentAmountEvent);
                }

                lock (_reagentAmountByEquipmentTypes)
                {
                    if (!_reagentAmountByEquipmentTypes.ContainsKey(
                            (EquipmentTypes)modifyReagentAmountEvent.EquipmentNum))
                    {
                        _reagentAmountByEquipmentTypes[(EquipmentTypes)modifyReagentAmountEvent.EquipmentNum] = 0;
                    }
                    _reagentAmountByEquipmentTypes[(EquipmentTypes)modifyReagentAmountEvent.EquipmentNum] +=
                        modifyReagentAmountEvent.Amount;
                }
            }
        });
    }
    

    public int GetLatestAmount(EquipmentTypes equipmentType)
    {
        lock (_reagentAmountByEquipmentTypes)
        {
            var containsKey = _reagentAmountByEquipmentTypes.TryGetValue(equipmentType, out var amount);
            return containsKey ? amount : 0;
        }
    }
    
    public int GetProcessedAmountForPeriod(ProcessedAmountRequest request)
    {
        lock (_reagentAmountEventsByEquipmentTypes)
        {
            var containsKey = _reagentAmountEventsByEquipmentTypes.ContainsKey(request.EquipmentType);
            if (containsKey)
            {
                return _reagentAmountEventsByEquipmentTypes[request.EquipmentType]
                    .Where(x => x.Date >= request.FromDate && x.Date <= request.ToDate)
                    .Where(x => x.Amount < 0)
                    .Sum(x => x.Amount); 
            }
            else
            {
                return 0;
            }
        }
    }
}
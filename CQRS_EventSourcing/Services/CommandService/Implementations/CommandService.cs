using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.Services.CommandService.Interfaces;
using CQRS_EventSourcing.Events.Implementations;

namespace CQRS_EventSourcing.Services.CommandService.Implementations;

public class CommandService : ICommandService
{
    private readonly IEventStore _storeWriter; 
    public CommandService(IEventStore storeWriter)
    {
        _storeWriter = storeWriter;
    }
    
    public void SetAmountDiff(int amountDiff, DateTime actionDate)
    {
        var modifyAmountEvent = new ModifySubstanceAmountEvent(amountDiff, actionDate);
        _storeWriter.AddEvent(modifyAmountEvent);    
    }
}
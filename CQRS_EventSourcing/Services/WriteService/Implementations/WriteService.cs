using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.Services.WriteService.Interfaces;
using CQRS_EventSourcing.Events.Implementations;

namespace CQRS_EventSourcing.Services.WriteService.Implementations;

public class WriteService : IWriteService
{
    private readonly IEventStore _storeWriter; 
    public WriteService(IEventStore storeWriter)
    {
        _storeWriter = storeWriter;
    }
    
    public void SetAmountDiff(int amountDiff, DateTime actionDate)
    {
        var modifyAmountEvent = new ModifySubstanceAmountEvent(amountDiff, actionDate);
        _storeWriter.AddEvent(modifyAmountEvent);    
    }
}
using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.WriteService.Interfaces;

using CQRS_EventSourcing.Events.Implementations;

namespace CQRS_EventSourcing.WriteService.Implementations;

public class WriteService : IWriteService
{
    private IEventWriter _writer; 
    public WriteService(IEventWriter writer)
    {
        _writer = writer;
    }
    
    public void SetAmountDiff(int amountDiff, DateTime actionDate)
    {
        var modifyAmountEvent = new ModifySubstanceAmountEvent(amountDiff, actionDate);
        _writer.AddEvent(modifyAmountEvent);    
    }
}
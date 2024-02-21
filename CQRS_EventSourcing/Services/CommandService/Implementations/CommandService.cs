using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.Services.CommandService.Interfaces;
using CQRS_EventSourcing.Events.Implementations;
using CQRS_EventSourcing.DomainModels;

namespace CQRS_EventSourcing.Services.CommandService.Implementations;

public class CommandService : ICommandService
{
    private readonly IEventStore _storeWriter; 
    public CommandService(IEventStore storeWriter)
    {
        _storeWriter = storeWriter;
    }

    public void PerformExperiment(ExperimentDetails experimentDetails)
    {
        var actionDate = experimentDetails.ExperimentDate;
        var amountDiff = -experimentDetails.UsedAmount;
        var equipment = experimentDetails.EquipmentType;
        SetAmountDiff(amountDiff, actionDate, equipment);
    }

    public void LoadInstrument(ReloadEquipmentDetails reloadEquipmentDetails)
    {
        var actionDate = reloadEquipmentDetails.ReloadDate;
        var amountDiff = reloadEquipmentDetails.AddedAmount;
        var equipment = reloadEquipmentDetails.EquipmentType;
        SetAmountDiff(amountDiff, actionDate, equipment);       
    }

    
    private void SetAmountDiff(int amountDiff, DateTime actionDate, EquipmentTypes equipmentType)
    {
        var modifyAmountEvent = new ModifyReagentAmountEvent(amountDiff, actionDate, (int)equipmentType);
        _storeWriter.AddEvent(modifyAmountEvent);    
    }
}
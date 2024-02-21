namespace CQRS_EventSourcing.Events.Interfaces;

//event for the reagent amount modification  
public interface IModifyReagentAmountEvent : IEvent
{
    // used (negative) or added (positive) amount of the reagent
    int Amount { get; }
    // date of the action
    DateTime Date { get; }
    
    // Equipment type
    int EquipmentNum { get; }
}
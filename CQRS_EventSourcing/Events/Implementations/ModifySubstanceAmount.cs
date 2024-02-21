using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.Events.Implementations;

public class ModifyReagentAmountEvent : IModifyReagentAmountEvent
{
    public ModifyReagentAmountEvent(int amount, DateTime date, int equipmentNum)
    {
        Amount = amount;
        Date = date;
        EquipmentNum = equipmentNum;
        EventGuid = Guid.NewGuid();
    }
    public int EquipmentNum { get; }
    public int Amount { get; }
    public DateTime Date { get; }
    public Guid EventGuid { get; }
}
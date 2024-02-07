using CQRS_EventSourcing.Events.Interfaces;

namespace CQRS_EventSourcing.Events.Implementations;

public class ModifySubstanceAmountEvent : IModifySubstanceAmountEvent
{
    public ModifySubstanceAmountEvent(int amount, DateTime date)
    {
        Amount = amount;
        Date = date;
        EventGuid = Guid.NewGuid();
    }
    public int Amount { get; }
    public DateTime Date { get; }
    public Guid EventGuid { get; }
}
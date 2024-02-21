namespace CQRS_EventSourcing.DomainModels;

public record ReloadEquipmentDetails
{
    public int AddedAmount;
    public EquipmentTypes EquipmentType;
    public DateTime ReloadDate;
}
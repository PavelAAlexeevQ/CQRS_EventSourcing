namespace CQRS_EventSourcing.DomainModels;

public record ExperimentDetails
{
    public int UsedAmount;
    public EquipmentTypes EquipmentType;
    public DateTime ExperimentDate;
}
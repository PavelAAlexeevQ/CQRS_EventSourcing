namespace CQRS_EventSourcing.DomainModels;

public record ProcessedAmountRequest
{
    public EquipmentTypes EquipmentType;
    public DateTime FromDate;
    public DateTime ToDate;
}
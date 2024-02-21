using CQRS_EventSourcing.DomainModels;

namespace CQRS_EventSourcing.Services.QueryService.Interfaces;

// Query Service
public interface IQueryService
{
    // calculate amount of the reagent at the current moment by collecting all related events
    int GetLatestAmount(EquipmentTypes equipmentType);

    // calculate amount of used reagent between 2 dates
    // by collecting all related events for the period with negative amount (negative amount means what the reagent was used)
    int GetProcessedAmountForPeriod(ProcessedAmountRequest request);
}
namespace CQRS_EventSourcing.Services.QueryService.Interfaces;

// Query Service
public interface IQueryService
{
    // calculate amount of a substance at the current moment by collecting all related events
    int GetLatestAmount();

    // calculate amount of used substance between 2 dates
    // by collecting all related events for the period with negative amount (negative amount means what a substance was used)
    int GetPrecessedAmountForPeriod(DateTime from, DateTime to);
}
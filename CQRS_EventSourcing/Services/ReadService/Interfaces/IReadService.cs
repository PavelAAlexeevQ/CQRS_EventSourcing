namespace CQRS_EventSourcing.ReadService.Interfaces;

// Read Service, or Query Service
public interface IReadService
{
    // calculate amount of a substance at the current moment by collecting all related events
    int GetLatestAmount();

    // calculate amount of used substance between 2 dates
    // by collecting all related events for the period with negative amount (negative amount means what a substance was used)
    int GetPrecessedAmountForPeriod(DateTime from, DateTime to);
}
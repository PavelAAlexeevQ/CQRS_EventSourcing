namespace CQRS_EventSourcing.ReadService.Interfaces;

public interface IReadService
{
    int GetLatestAmount();
    int GetPrecessedAmountForPeriod(DateTime from, DateTime to);
}
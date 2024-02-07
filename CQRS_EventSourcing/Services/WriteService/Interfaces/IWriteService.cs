namespace CQRS_EventSourcing.WriteService.Interfaces;

public interface IWriteService
{
    void SetAmountDiff(int amountDiff, DateTime actionDate);
}
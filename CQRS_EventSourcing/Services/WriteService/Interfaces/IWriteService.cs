namespace CQRS_EventSourcing.Services.WriteService.Interfaces;

// Write Service, or Command Service
public interface IWriteService
{
    //writes event for amount count modification (negative or positive) and date of the action
    void SetAmountDiff(int amountDiff, DateTime actionDate);
}
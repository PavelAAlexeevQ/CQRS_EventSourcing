namespace CQRS_EventSourcing.Services.CommandService.Interfaces;

//Command Service
public interface ICommandService
{
    //writes event for amount count modification (negative or positive) and date of the action
    void SetAmountDiff(int amountDiff, DateTime actionDate);
}
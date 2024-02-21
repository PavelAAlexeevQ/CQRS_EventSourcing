using CQRS_EventSourcing.DomainModels;


namespace CQRS_EventSourcing.Services.CommandService.Interfaces;

//Command Service
public interface ICommandService
{
    //Perform experiment using requested equipment
    void PerformExperiment(ExperimentDetails experimentDetails);
    
    //Load i.e. add some amount of reagent  
    void LoadInstrument(ReloadEquipmentDetails experimentDetails);

}
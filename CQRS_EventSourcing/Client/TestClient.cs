using Autofac;
using CQRS_EventSourcing.DomainModels;
using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.EventBus.Implementations;
using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.EventStore.Implementations;
using CQRS_EventSourcing.Services.QueryService.Interfaces;
using CQRS_EventSourcing.Services.QueryService.Implementations;
using CQRS_EventSourcing.Services.CommandService.Interfaces;
using CQRS_EventSourcing.Services.CommandService.Implementations;


namespace CQRS_EventSourcing.Client;

public class TestClient
{ 

    private readonly ILifetimeScope _scope;
    private readonly ICommandService _commandService;
    private readonly Random _rand = new Random();
    public TestClient()
    {        
        //setup all our services
        var builder = new ContainerBuilder();
        // all interfaces are singletons except Query Service, which may be created in any count 
        //EventBus has 2 "sides" - read and write, se we register the type for 2 interfaces
        builder.RegisterType<EventBusImpl>().As<IEventBusSender, IEventBusReceiver>().SingleInstance();
        builder.RegisterType<EventStoreImpl>().As<IEventStore>().SingleInstance();
        builder.RegisterType<CommandService>().As<ICommandService>().SingleInstance();
        builder.RegisterType<QueryService>().As<IQueryService>();
        
        var container = builder.Build();
        _scope = container.BeginLifetimeScope(); // should be disposed in the end, but this step is omitted
        _commandService = _scope.Resolve<ICommandService>();
    }

    public void RunTest()
    {
        var queryService = _scope.Resolve<IQueryService>();

        // perform several commands
        for (var i = 100; i > 0; i--)
        {
            LoadReagentIntoInstrument(i);
            
            PerformExperiment(i);
        }

        // here is the first problem - virtual consistency.
        // As events goes from Storage to QueryService in async mode we shall wait a while
        // to have the latest updates in the QueryService  
        Thread.Sleep(100);

        // now let's query something 
        var latestAmount = GetLatestAmount(queryService, EquipmentTypes.Instrument_1);
        Console.WriteLine(
            $"QueryService: Current amount of the reagent in {EquipmentTypes.Instrument_1.ToString()} is: {latestAmount}");

        var usedAmount = GetUsedAmount(queryService, EquipmentTypes.Instrument_1);
        Console.WriteLine(
            $"QueryService: Used amount of the reagent in {EquipmentTypes.Instrument_1.ToString()} during last 100 days: {usedAmount}");

        
        //imagine we need one more query service. We may instantiate another one
        var queryServiceAnother = _scope.Resolve<IQueryService>();
        var latestAmountAnother = GetLatestAmount(queryServiceAnother, EquipmentTypes.Instrument_2);
        Console.WriteLine($"Service(another): Current amount of the reagent is: {latestAmountAnother}");

        var processedAmountRequestAnother = new ProcessedAmountRequest
        {
            EquipmentType = EquipmentTypes.Instrument_2,
            FromDate = DateTime.Now.AddDays(-100),
            ToDate = DateTime.Now
        };
        var usedAmountAnother = GetUsedAmount(queryService, EquipmentTypes.Instrument_2);
        Console.WriteLine(
            $"Service(another): Used amount of the reagent in {EquipmentTypes.Instrument_2.ToString()} during last 100 days: {usedAmountAnother}");
    }

    private void PerformExperiment(int dayNum)
    {
        var amount = _rand.Next(1000) - 500;
        var instrument = (EquipmentTypes)_rand.Next((int)EquipmentTypes.Instrument_2 + 1);

        var experimentDetails = new ExperimentDetails()
        {
            UsedAmount = amount,
            EquipmentType = instrument,
            ExperimentDate = DateTime.Now.AddDays(-dayNum),
        };
        _commandService.PerformExperiment(experimentDetails);
    }

    private void LoadReagentIntoInstrument(int dayNum)
    {
        var amount = _rand.Next(1000) - 500;
        var instrument = (EquipmentTypes)_rand.Next((int)EquipmentTypes.Instrument_2 + 1);
        var reloadEquipmentDetails = new ReloadEquipmentDetails()
        {
            AddedAmount = amount,
            EquipmentType = instrument,
            ReloadDate = DateTime.Now.AddDays(-dayNum),
        };
        _commandService.LoadInstrument(reloadEquipmentDetails);
    }

    private int GetLatestAmount(IQueryService queryService, EquipmentTypes equipmentType)
    {
        return queryService.GetLatestAmount(equipmentType);
    }

    private int GetUsedAmount(IQueryService queryService, EquipmentTypes equipmentType)
    {
        var processedAmountRequest = new ProcessedAmountRequest
        {
            EquipmentType = equipmentType,
            FromDate = DateTime.Now.AddDays(-100),
            ToDate = DateTime.Now
        };
        return Math.Abs(queryService.GetProcessedAmountForPeriod(processedAmountRequest));

    }
}

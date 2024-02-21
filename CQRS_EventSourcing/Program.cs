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

class CQRS_EventSourcingModel
{
    public static int Main()
    {
        //setup all our services
        var builder = new ContainerBuilder();
        // all interfaces are singletons except Query Service, which may be created in any count 
        //EventBus has 2 "sides" - read and write, se we register the type for 2 interfaces
        builder.RegisterType<EventBus>().As<IEventBusSender,IEventBusReceiver>().SingleInstance();
        builder.RegisterType<EventStore>().As<IEventStore>().SingleInstance();
        builder.RegisterType<CommandService>().As<ICommandService>().SingleInstance();
        builder.RegisterType<QueryService>().As<IQueryService>();

        var container = builder.Build();
        using var scope = container.BeginLifetimeScope();
        {
            var commandService = scope.Resolve<ICommandService>();
            var queryService = scope.Resolve<IQueryService>();
            var rand = new Random();

            for (var i = 100; i > 0; i--)
            {
                var amount = rand.Next(1000) - 500;
                var instrument = (EquipmentTypes)rand.Next((int)EquipmentTypes.Instrument2 + 1);

                var experimentDetails = new ExperimentDetails()
                {
                    UsedAmount = amount,
                    EquipmentType = instrument,
                    ExperimentDate = DateTime.Now.AddDays(-i),
                };
                commandService.PerformExperiment(experimentDetails); 

                amount = rand.Next(1000) - 500;
                instrument = (EquipmentTypes)rand.Next((int)EquipmentTypes.Instrument2 + 1);
                var reloadEquipmentDetails = new ReloadEquipmentDetails()
                {
                    AddedAmount = amount,
                    EquipmentType = instrument,
                    ReloadDate = DateTime.Now.AddDays(-i),
                };
                commandService.LoadInstrument(reloadEquipmentDetails);
            }
            // here is the first problem - virtual consistency.
            // As events goes from Storage to QueryService in async mode we shall wait a while
            // to have the latest updates in the QueryService  
            Thread.Sleep(1000);
            
            // now let's query something 
            var latestAmount = queryService.GetLatestAmount(EquipmentTypes.Instrument1);
            Console.WriteLine($"QueryService: Current amount of the reagent in {EquipmentTypes.Instrument1.ToString()} is: {latestAmount}");

            var processedAmountRequest = new ProcessedAmountRequest
            {
                EquipmentType = EquipmentTypes.Instrument1,
                FromDate = DateTime.Now.AddDays(-100),
                ToDate = DateTime.Now
            };
            var usedAmount =
                Math.Abs(queryService.GetProcessedAmountForPeriod(processedAmountRequest));
            Console.WriteLine($"QueryService: Used amount of the reagent in {EquipmentTypes.Instrument1.ToString()} during last 100 days: {usedAmount}");
            
            //imagine we need one more query service. We may instantiate another one
            var queryServiceAnother = scope.Resolve<IQueryService>();
            var latestAmountAnother = queryServiceAnother.GetLatestAmount(EquipmentTypes.Instrument2);
            Console.WriteLine($"Service(another): Current amount of the reagent is: {latestAmountAnother}");
            
            var processedAmountRequestAnother = new ProcessedAmountRequest
            {
                EquipmentType = EquipmentTypes.Instrument2,
                FromDate = DateTime.Now.AddDays(-100),
                ToDate = DateTime.Now
            };
            var usedAmountAnother =
                Math.Abs(queryServiceAnother.GetProcessedAmountForPeriod(processedAmountRequestAnother));
            Console.WriteLine($"Service(another): Used amount of the reagent in {EquipmentTypes.Instrument2.ToString()} during last 100 days: {usedAmountAnother}");

        }
        
        return 0;
    }
}
using Autofac;

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
        builder.RegisterType<EventBus>().As<IEventBusSender,IEventBusReceiver>().SingleInstance();
        builder.RegisterType<EventStore>().As<IEventStore>().SingleInstance();
        builder.RegisterType<CommandService>().As<ICommandService>().SingleInstance();
        builder.RegisterType<QueryService>().As<IQueryService>();

        var container = builder.Build();
        using var scope = container.BeginLifetimeScope();
        {
            // Logger and ConfigReader will be populated.
            var commandService = scope.Resolve<ICommandService>();
            var queryService1 = scope.Resolve<IQueryService>();
            var rand = new Random();
            for (var i = 100; i > 0; i--)
            {
                var amountDiff = rand.Next(1000) - 500;
                commandService.SetAmountDiff(amountDiff, DateTime.Now.AddDays(-i));
            }

            var latestAmount1 = queryService1.GetLatestAmount();
            Console.WriteLine($"Service1: Current amount of a substance is: {latestAmount1}");
            var usedAmount1 =
                Math.Abs(queryService1.GetPrecessedAmountForPeriod(from: DateTime.Now.AddDays(-100), to: DateTime.Now));
            Console.WriteLine($"Service1: Used amount of a substance during last 100 days: {usedAmount1}");
            
            var queryService2 = scope.Resolve<IQueryService>();
            var latestAmount2 = queryService2.GetLatestAmount();
            Console.WriteLine($"Service2: Current amount of a substance is: {latestAmount2}");
            var usedAmount2 =
                Math.Abs(queryService2.GetPrecessedAmountForPeriod(from: DateTime.Now.AddDays(-100), to: DateTime.Now));
            Console.WriteLine($"Service2: Used amount of a substance during last 100 days: {usedAmount2}");

            if (latestAmount1 != latestAmount2 || usedAmount1 != usedAmount2)
            {
                Console.Error.WriteLine("Our proof of concept is failed :(");
            }
        }
        
        return 0;
    }
}
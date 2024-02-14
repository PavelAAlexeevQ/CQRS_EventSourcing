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
                var amountDiff = rand.Next(1000) - 500;
                commandService.SetAmountDiff(amountDiff, DateTime.Now.AddDays(-i));
            }
            // here is the first problem - virtual consistency.
            // As events goes from Storage to QueryService in async mode we shall wait a while
            // to have the latest updates in the QueryService  
            Thread.Sleep(100);
            
            // now let's query something 
            var latestAmount = queryService.GetLatestAmount();
            Console.WriteLine($"QueryService: Current amount of a substance is: {latestAmount}");
            var usedAmount =
                Math.Abs(queryService.GetPrecessedAmountForPeriod(from: DateTime.Now.AddDays(-100), to: DateTime.Now));
            Console.WriteLine($"QueryService: Used amount of a substance during last 100 days: {usedAmount}");
            
            //imagine we need one more query service. We may instantiate another one
            var queryServiceAnother = scope.Resolve<IQueryService>();
            var latestAmountAnother = queryServiceAnother.GetLatestAmount();
            Console.WriteLine($"Service(another): Current amount of a substance is: {latestAmountAnother}");
            var usedAmountAnother =
                Math.Abs(queryServiceAnother.GetPrecessedAmountForPeriod(from: DateTime.Now.AddDays(-100), to: DateTime.Now));
            Console.WriteLine($"Service(another): Used amount of a substance during last 100 days: {usedAmountAnother}");

            //check if both QueryServices works in the same way (but actually it is now guaranteed at all)
            if (latestAmount != latestAmountAnother || usedAmount != usedAmountAnother)
            {
                Console.Error.WriteLine("Our proof of concept is failed :(");
            }
        }
        
        return 0;
    }
}
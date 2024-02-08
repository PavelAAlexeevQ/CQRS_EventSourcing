using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.EventBus.Implementations;
using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.EventStore.Implementations;
using CQRS_EventSourcing.Services.ReadService.Interfaces;
using CQRS_EventSourcing.Services.ReadService.Implementations;
using CQRS_EventSourcing.Services.WriteService.Interfaces;
using CQRS_EventSourcing.Services.WriteService.Implementations;

class CQRS_EventSourcingModel
{
    public static int Main()
    {
        //setup all our services
        var serviceProvider = new ServiceCollection()
            .AddSingleton<EventBus>()
            .AddSingleton<IEventBusSender>(x => x.GetRequiredService<EventBus>())
            .AddSingleton<IEventBusReceiver>(x => x.GetRequiredService<EventBus>())
            .AddSingleton<IEventStore, EventStore>()
            .AddSingleton<IWriteService, WriteService>()
            .BuildServiceProvider();

        
        // get one 
        var commandService = serviceProvider.GetService<IWriteService>();
        var eventBusReceiver = serviceProvider.GetService<IEventBusReceiver>();
        if (commandService == null || eventBusReceiver == null)
        {
            Console.WriteLine("Something wrong with my DI");
            return -1;
        }

        var queryService1 = new ReadService(eventBusReceiver);
        var queryService2 = new ReadService(eventBusReceiver);

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

        var latestAmount2 = queryService2.GetLatestAmount();
        Console.WriteLine($"Service2: Current amount of a substance is: {latestAmount2}");
        var usedAmount2 =
            Math.Abs(queryService2.GetPrecessedAmountForPeriod(from: DateTime.Now.AddDays(-100), to: DateTime.Now));
        Console.WriteLine($"Service2: Used amount of a substance during last 100 days: {usedAmount2}");

        if (latestAmount1 != latestAmount2 || usedAmount1 != usedAmount2)
        {
            Console.Error.WriteLine("Our proof of concept is failed :(");
        }
        return 0;
    }
}
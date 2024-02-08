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
            .AddSingleton<IReadService, ReadService>()
            .AddSingleton<IWriteService, WriteService>()
            .BuildServiceProvider();

        
        // get one 
        var commandService = serviceProvider.GetService<IWriteService>();
        var queryService = serviceProvider.GetService<IReadService>();
        if (commandService == null || queryService == null)
        {
            Console.WriteLine("Something wrong with my DI");
            return -1;
        }

        var rand = new Random();
        for (var i = 100; i > 0; i--)
        {
            var amountDiff = rand.Next(1000) - 500;
            commandService.SetAmountDiff(amountDiff, DateTime.Now.AddDays(-i));
        }

        var latestAmount = queryService.GetLatestAmount();
        Console.WriteLine($"Current amount of a substance is: {latestAmount}");
        var usedAmount =
            Math.Abs(queryService.GetPrecessedAmountForPeriod(from: DateTime.Now.AddDays(-100), to: DateTime.Now));
        Console.WriteLine($"Used amount of a substance during last 100 days: {usedAmount}");

        return 0;
    }
}
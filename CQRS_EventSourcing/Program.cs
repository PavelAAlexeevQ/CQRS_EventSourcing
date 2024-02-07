using Microsoft.Extensions.DependencyInjection;


using CQRS_EventSourcing.EventBus.Interfaces;
using CQRS_EventSourcing.EventBus.Implementations;
using CQRS_EventSourcing.EventStore.Interfaces;
using CQRS_EventSourcing.EventStore.Implementations;
using CQRS_EventSourcing.ReadService.Interfaces;
using CQRS_EventSourcing.ReadService.Implementations;
using CQRS_EventSourcing.WriteService.Interfaces;
using CQRS_EventSourcing.WriteService.Implementations;


var serviceProvider = new ServiceCollection()
    .AddSingleton<IEventQueue, EventQueue>()
    .AddSingleton<IEventStore, EventStore>()
    .AddSingleton<IReadService, ReadService>()
    .AddSingleton<IWriteService, WriteService>()
    .BuildServiceProvider();


//do the actual work here
var writeService = serviceProvider.GetService<IWriteService>();
var readService = serviceProvider.GetService<IReadService>();

var rand = new Random();
for (int i = 100; i > 0; i--)
{
    int amountDiff = rand.Next(1000) - 500;
    writeService.SetAmountDiff(amountDiff, DateTime.Now.AddDays(-i));
}

int latestAmount = readService.GetLatestAmount();
Console.WriteLine($"Current amount of a substance is: {latestAmount}");
int usedAmount = Math.Abs(readService.GetPrecessedAmountForPeriod(from: DateTime.Now.AddDays(-100), to: DateTime.Now));
Console.WriteLine($"Used amount of a substance during last 100 days: {usedAmount}");

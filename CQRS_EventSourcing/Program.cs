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
using CQRS_EventSourcing.Client;

class CQRS_EventSourcingModel
{
    public static int Main()
    {
        var testClient = new TestClient();
        testClient.RunTest();

        return 0;
    }
}
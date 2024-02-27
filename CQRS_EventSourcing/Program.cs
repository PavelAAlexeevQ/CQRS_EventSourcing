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
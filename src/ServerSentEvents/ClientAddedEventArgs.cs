namespace ServerSentEvents
{
    internal class ClientAddedEventArgs
    {
        public ClientAddedEventArgs(IClient newClient)
        {
            NewClient = newClient;
        }

        public IClient NewClient { get; }
    }
}

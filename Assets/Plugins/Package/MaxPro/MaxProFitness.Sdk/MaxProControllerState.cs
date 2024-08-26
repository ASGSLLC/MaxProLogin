namespace MaxProFitness.Sdk
{
    public enum MaxProControllerState : byte
    {
        Disconnected,
        Disconnecting,
        Initializing,
        Scanning,
        Connecting,
        Subscribing,
        Validating,
        Connected,
    }
}

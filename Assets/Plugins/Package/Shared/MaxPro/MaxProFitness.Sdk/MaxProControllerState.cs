namespace maxprofitness.shared
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

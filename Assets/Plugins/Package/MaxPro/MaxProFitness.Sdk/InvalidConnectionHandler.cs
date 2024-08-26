using System;
using UnityEngine.Scripting;

namespace MaxProFitness.Sdk
{
    [Preserve]
    public class InvalidConnectionHandler : IMaxProConnectionHandler
    {
        public void Initialize(Action onSucceed, Action<string> onFailed)
        {
            onFailed?.Invoke($"Attempting to initialize an {nameof(InvalidConnectionHandler)}");
        }

        public void Dispose(Action onFinished)
        {
            onFinished?.Invoke();
        }

        public void MakeLEPairRequest(string deviceName, Action<string, string> onDeviceFound) { }

        public void StopScan() { }

        public void Connect(string address, string deviceName, Action<string, string, string> onSucceed, Action<string> onFailed)
        {
            onFailed?.Invoke($"Attempting to connect with an {nameof(InvalidConnectionHandler)}");
        }

        public void Disconnect(string address, Action onFinished)
        {
            onFinished?.Invoke();
        }

        public void StartListening(string address, string service, string characteristic, Action onFinished, Action<byte[]> onDataReceived)
        {
            onFinished?.Invoke();
        }

        public void StopListening(string address, string service, string characteristic, Action onFinished)
        {
            onFinished?.Invoke();
        }

        public void Send(string address, string service, string characteristic, byte[] data, Action onFinished)
        {
            onFinished?.Invoke();
        }

        public void OnUpdate() { }
    }
}

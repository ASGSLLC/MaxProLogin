using System;
using UnityEngine.Scripting;

namespace maxprofitness.login
{
    [Preserve]
    public interface IMaxProConnectionHandler
    {
        void Initialize(Action onSucceed, Action<string> onFailed);

        void Dispose(Action onFinished);

        void MakeLEPairRequest(string deviceName, Action<string, string> onDeviceFound);

       // void StopScan();

        void Connect(string address, string deviceName, Action<string, string, string> onSucceed, Action<string> onFailed);

        void Disconnect(string address, Action onFinished);

        void StartListening(string address, string service, string characteristic, Action onFinished, Action<byte[]> onDataReceived);

        void StopListening(string address, string service, string characteristic, Action onFinished);

        void Send(string address, string service, string characteristic, byte[] data, Action onFinished);

        void OnUpdate();
    }
}

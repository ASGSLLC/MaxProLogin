using MaxProFitness.Sdk;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace maxprofitness.login
{
    [Preserve]
    [Serializable]
    public sealed class MultiplatformConnectionHandler : IMaxProConnectionHandler
    {
        [SerializeField] private InterfaceField<IMaxProConnectionHandler> _windows;
        [SerializeField] private InterfaceField<IMaxProConnectionHandler> _mobile;
        [SerializeField] private InterfaceField<IMaxProConnectionHandler> _fallback;

        private IMaxProConnectionHandler _current;

        public void Initialize(Action onSucceed, Action<string> onFailed)
        {
            if (Application.isEditor)
            {
                _current = Application.platform == RuntimePlatform.WindowsEditor ? _windows.Value : _fallback.Value;
            }
            else
            {
                if (Application.isMobilePlatform)
                {
                    _current = _mobile.Value;
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    _current = _windows.Value;
                }
                else
                {
                    _current = _fallback.Value;
                }
            }

            if (_current == null)
            {
                _current = _fallback.Value;
            }

            if (_current != null)
            {
                _current.Initialize(onSucceed, onFailed);
            }
            else
            {
                onFailed?.Invoke("Could not find a valid connection handler for the current platform!");
            }
        }

        public void Dispose(Action onFinished)
        {
            _current?.Dispose(onFinished);
        }

        public void MakeLEPairRequest(string deviceName, Action<string, string> onDeviceFound)
        {
            _current?.MakeLEPairRequest(deviceName, onDeviceFound);
        }

        public void StopScan()
        {
           // _current?.StopScan();
        }

        public void Connect(string address, string deviceName, Action<string, string, string> onSucceed, Action<string> onFailed)
        {
            _current?.Connect(address, deviceName, onSucceed, onFailed);
        }

        public void Disconnect(string address, Action onFinished)
        {
            _current?.Disconnect(address, onFinished);
        }

        public void StartListening(string address, string service, string characteristic, Action onFinished, Action<byte[]> onDataReceived)
        {
            _current?.StartListening(address, service, characteristic, onFinished, onDataReceived);
        }

        public void StopListening(string address, string service, string characteristic, Action onFinished)
        {
            _current?.StopListening(address, service, characteristic, onFinished);
        }

        public void Send(string address, string service, string characteristic, byte[] data, Action onFinished)
        {
            _current?.Send(address, service, characteristic, data, onFinished);
        }

        public void OnUpdate()
        {
            _current?.OnUpdate();
        }
    }
}

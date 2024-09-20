#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#define UNITY_WINDOWS

using System;
using System.Collections.Generic;
//using WCL;
#endif
//using MaxProFitness.Sdk;
using System.Text;
using UnityEngine.Scripting;

namespace maxprofitness.shared
{
    /*[Preserve]
    public sealed class WindowsBluetoothFrameworkNativeConnectionHandler
#if UNITY_WINDOWS
        : IMaxProConnectionHandler
#else
        : InvalidConnectionHandler
#endif
    {
#if UNITY_WINDOWS
        private sealed class Device
        {
            public string Name;

            public long Address;

            public IntPtr Radio;

            public override string ToString()
            {
                return Address.ToString("X12");
            }
        }

        private const byte Timeout = 10; // should be in range [2, 61]
        private BluetoothManager _manager;
        private IntPtr _radio;
        private GattClient _client;
        private Dictionary<string, Device> _deviceFromAddress;
        private Dictionary<string, Dictionary<string, GattCharacteristic>> _characteristicFromUuidFromService;
        private Action<string> _onConnectFailed;
        private Action<string, string> _onDeviceFound;
        private Action<string, string, string> _onConnectSucceed;
        private Action<byte[]> _onDataReceived;

        public void Initialize(Action onSucceed, Action<string> onFailed)
        {
            _client = new GattClient();
            _client.OnChanged -= HandleCharacteristicChanged;
            _client.OnChanged += HandleCharacteristicChanged;
            _client.OnConnect -= HandleClientConnect;
            _client.OnConnect += HandleClientConnect;

            _manager = new BluetoothManager();
            _manager.OnDeviceFound -= HandleDeviceFound;
            _manager.OnDeviceFound += HandleDeviceFound;
            _manager.OnDiscoveringCompleted -= HandleDiscoveringCompleted;
            _manager.OnDiscoveringCompleted += HandleDiscoveringCompleted;

            _characteristicFromUuidFromService = new Dictionary<string, Dictionary<string, GattCharacteristic>>();
            _deviceFromAddress = new Dictionary<string, Device>();

            int errorCode = _manager.Open();

            if (errorCode != BluetoothErrors.WCL_E_SUCCESS)
            {
                onFailed?.Invoke(GetErrorMessage(errorCode));
            }
            else
            {
                for (int i = 0; i < _manager.Count; i++)
                {
                    if (_manager.IsRadioAvailable(_manager[i]))
                    {
                        _radio = _manager[i];
                        onSucceed?.Invoke();

                        return;
                    }
                }

                onFailed?.Invoke("No available Bluetooth adapter was found!");
            }
        }

        public void Dispose(Action onFinished)
        {
            _client.OnChanged -= HandleCharacteristicChanged;
            _client.OnConnect -= HandleClientConnect;
            _client.Dispose();
            _manager.OnDiscoveringCompleted -= HandleDiscoveringCompleted;
            _manager.OnDeviceFound -= HandleDeviceFound;
            _manager.Close();
            _manager.Dispose();

            _deviceFromAddress.Clear();
            _characteristicFromUuidFromService.Clear();
            onFinished?.Invoke();
        }

        public void StartScan(Action<string, string> onDeviceFound)
        {
            if (onDeviceFound == null || _onDeviceFound != null)
            {
                _onDeviceFound = onDeviceFound;

                return;
            }

            _onDeviceFound = onDeviceFound;
            _deviceFromAddress.Clear();
            _manager.Discover(_radio, Timeout);
        }

        public void StopScan()
        {
            _onDeviceFound = null;
        }

        public void Connect(string address, string deviceName, Action<string, string, string> onSucceed, Action<string> onFailed)
        {
            if (onSucceed == null || _onConnectSucceed != null)
            {
                _onConnectSucceed = onSucceed;
                _onConnectFailed = onFailed;

                return;
            }

            if (_deviceFromAddress.TryGetValue(address, out Device device))
            {
                int errorCode = _client.Connect(device.Radio, device.Address);

                if (errorCode != BluetoothErrors.WCL_E_SUCCESS)
                {
                    onFailed?.Invoke(GetErrorMessage(errorCode));

                    return;
                }

                _onConnectSucceed = onSucceed;
                _onConnectFailed = onFailed;
            }
            else
            {
                onFailed?.Invoke($"The address {address} doesn't points to a previously scanned device!");
            }

        }

        public void Disconnect(string address, Action onFinished)
        {
            _client.Disconnect();
            onFinished?.Invoke();
        }

        public void StartListening(string address, string service, string characteristic, Action onFinished, Action<byte[]> onDataReceived)
        {
            if (onDataReceived == null || _onDataReceived != null)
            {
                _onDataReceived = onDataReceived;

                return;
            }

            if (_characteristicFromUuidFromService.TryGetValue(service, out Dictionary<string, GattCharacteristic> characteristicFromUuid)
             && characteristicFromUuid.TryGetValue(characteristic, out GattCharacteristic subscribeCharacteristic))
            {
                if (subscribeCharacteristic.IsNotifiable && subscribeCharacteristic.IsIndicatable)
                {
                    subscribeCharacteristic.IsIndicatable = false;
                }

                _onDataReceived = onDataReceived;
                _client.Subscribe(subscribeCharacteristic);
                _client.WriteClientConfiguration(subscribeCharacteristic, true, GattOperationFlag.goNone, GattProtectionLevel.plNone);
            }

            onFinished?.Invoke();
        }

        public void StopListening(string address, string service, string characteristic, Action onFinished)
        {
            if (_characteristicFromUuidFromService.TryGetValue(service, out Dictionary<string, GattCharacteristic> characteristicFromUuid)
             && characteristicFromUuid.TryGetValue(characteristic, out GattCharacteristic subscribeCharacteristic))
            {
                _onDataReceived = null;
                _client.WriteClientConfiguration(subscribeCharacteristic, false, GattOperationFlag.goNone, GattProtectionLevel.plNone);
                _client.Unsubscribe(subscribeCharacteristic);
            }

            onFinished?.Invoke();
        }

        public void Send(string address, string service, string characteristic, byte[] data, Action onFinished)
        {
            if (_characteristicFromUuidFromService.TryGetValue(service, out Dictionary<string, GattCharacteristic> characteristicFromUuid)
             && characteristicFromUuid.TryGetValue(characteristic, out GattCharacteristic writeCharacteristic))
            {
                _client.WriteValue(writeCharacteristic, data);
            }

            onFinished?.Invoke();
        }

        public void OnUpdate() { }

        private static string GetUuid(GattUuid uuid)
        {
            return uuid.IsShortUuid ? uuid.ShortUuid.ToString("X4") : uuid.LongUuid.ToString();
        }

        private static string GetErrorMessage(int errorCode)
        {
            if (BluetoothErrors.GetErrorInfo(errorCode, out _, out _, out _, out string errorMessage))
            {
                return errorMessage;
            }

            return $"Unknown error with code {errorCode}";
        }

        private void HandleCharacteristicChanged(object sender, ushort handle, byte[] value)
        {
            _onDataReceived?.Invoke(value);
        }

        private void HandleClientConnect(object sender, int errorCode)
        {
            int errorCount = 0;
            StringBuilder errorBuilder = new StringBuilder();

            if (errorCode != BluetoothErrors.WCL_E_SUCCESS)
            {
                errorCount++;
                errorBuilder.AppendLine(GetErrorMessage(errorCode));
            }

            errorCode = _client.GetServices(out GattServices services);

            if (errorCode != BluetoothErrors.WCL_E_SUCCESS)
            {
                errorCount++;
                errorBuilder.AppendLine(GetErrorMessage(errorCode));
            }

            if (services.Services != null)
            {
                for (int i = 0; i < services.Count; i++)
                {
                    GattService service = services.Services[i];
                    string serviceUuid = GetUuid(service.Uuid);

                    if (MaxProController.AreUuidEqual(serviceUuid, MaxProController.ShortServiceUuid))
                    {
                        errorCode = _client.GetCharacteristics(service, out GattCharacteristics characteristics);

                        if (errorCode != BluetoothErrors.WCL_E_SUCCESS)
                        {
                            errorCount++;
                            errorBuilder.AppendLine(GetErrorMessage(errorCode));
                        }

                        if (characteristics.Chars != null)
                        {
                            Dictionary<string, GattCharacteristic> characteristicFromUuid = new Dictionary<string, GattCharacteristic>();
                            _characteristicFromUuidFromService[serviceUuid] = characteristicFromUuid;

                            string subscribeCharacteristic = null;
                            string writeCharacteristic = null;

                            for (int j = 0; j < characteristics.Count; j++)
                            {
                                GattCharacteristic characteristic = characteristics.Chars[j];
                                string characteristicUuid = GetUuid(characteristic.Uuid);
                                characteristicFromUuid[characteristicUuid] = characteristic;
                                subscribeCharacteristic = MaxProController.AreUuidEqual(characteristicUuid, MaxProController.ShortSubscribeCharacteristicUuid) ? characteristicUuid : subscribeCharacteristic;
                                writeCharacteristic = MaxProController.AreUuidEqual(characteristicUuid, MaxProController.ShortWriteCharacteristicUuid) ? characteristicUuid : writeCharacteristic;
                            }

                            if (subscribeCharacteristic != null && writeCharacteristic != null)
                            {
                                _onConnectSucceed?.Invoke(serviceUuid, subscribeCharacteristic, writeCharacteristic);
                                _onConnectSucceed = null;

                                return;
                            }
                        }
                    }
                }
            }

            _onConnectSucceed = null;

            if (errorCount > 0)
            {
                string errorMessage = errorBuilder.ToString();
                _onConnectFailed?.Invoke(errorCount == 1 ? errorMessage : $"Multiple errors found: {errorMessage}");
            }
            else
            {
                _onConnectFailed?.Invoke("Could not found target Subscribe and/or Write Characteristic UUID!");
            }
        }

        private void HandleDeviceFound(object sender, IntPtr radio, long address)
        {
            Device device = new Device()
            {
                Address = address,
                Radio = radio,
            };

            _deviceFromAddress[device.ToString()] = device;
        }

        private void HandleDiscoveringCompleted(object sender, IntPtr radio, int errorCode)
        {
            foreach (KeyValuePair<string, Device> pair in _deviceFromAddress)
            {
                _manager.GetRemoteName(radio, pair.Value.Address, out pair.Value.Name);
                _onDeviceFound?.Invoke(pair.Key, pair.Value.Name);
            }

            _onDeviceFound = null;
        }
#endif
    
}*/
}

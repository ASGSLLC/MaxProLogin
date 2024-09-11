#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
#define UNITY_WINDOWS

using System;
using System.Collections.Generic;
using wclBluetooth;
using wclCommon;
#endif
using MaxProFitness.Sdk;
using System.Text;
using UnityEngine.Scripting;

namespace MaxProFitness.Integrations
{
    [Preserve]
    public sealed class WindowsBluetoothFrameworkConnectionHandler
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

            public wclBluetoothRadio Radio;

            public override string ToString()
            {
                return Address.ToString("X12");
            }
        }

        private const byte Timeout = 10; // should be in range [2, 61]
        private wclBluetoothManager _manager;
        private wclBluetoothRadio _radio;
        private wclGattClient _client;
        private Dictionary<string, Device> _deviceFromAddress;
        private Dictionary<string, Dictionary<string, wclGattCharacteristic>> _characteristicFromUuidFromService;
        private Action<string> _onConnectFailed;
        private Action<string, string> _onDeviceFound;
        private Action<string, string, string> _onConnectSucceed;
        private Action<byte[]> _onDataReceived;

        public void Initialize(Action onSucceed, Action<string> onFailed)
        {
            _client = new wclGattClient();
            _client.OnCharacteristicChanged -= HandleCharacteristicChanged;
            _client.OnCharacteristicChanged += HandleCharacteristicChanged;
            _client.OnConnect -= HandleClientConnect;
            _client.OnConnect += HandleClientConnect;

            _manager = new wclBluetoothManager();
            _manager.OnDeviceFound -= HandleDeviceFound;
            _manager.OnDeviceFound += HandleDeviceFound;
            _manager.OnDiscoveringCompleted -= HandleDiscoveringCompleted;
            _manager.OnDiscoveringCompleted += HandleDiscoveringCompleted;

            _characteristicFromUuidFromService = new Dictionary<string, Dictionary<string, wclGattCharacteristic>>();
            _deviceFromAddress = new Dictionary<string, Device>();

            int errorCode = _manager.Open();

            if (errorCode != wclErrors.WCL_E_SUCCESS)
            {
                onFailed?.Invoke(GetErrorMessage(errorCode));
            }
            else
            {
                for (int i = 0; i < _manager.Count; i++)
                {
                    if (_manager[i].Available)
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
            _client.OnCharacteristicChanged -= HandleCharacteristicChanged;
            _client.OnConnect -= HandleClientConnect;
            _manager.OnDiscoveringCompleted -= HandleDiscoveringCompleted;
            _manager.OnDeviceFound -= HandleDeviceFound;
            _manager.Close();
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
            _radio.Discover(Timeout, wclBluetoothDiscoverKind.dkBle);
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
                _client.Address = device.Address;

                int errorCode = _client.Connect(device.Radio);

                if (errorCode != wclErrors.WCL_E_SUCCESS)
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

            if (_characteristicFromUuidFromService.TryGetValue(service, out Dictionary<string, wclGattCharacteristic> characteristicFromUuid)
             && characteristicFromUuid.TryGetValue(characteristic, out wclGattCharacteristic subscribeCharacteristic))
            {
                if (subscribeCharacteristic.IsNotifiable && subscribeCharacteristic.IsIndicatable)
                {
                    subscribeCharacteristic.IsIndicatable = false;
                }

                _onDataReceived = onDataReceived;
                _client.Subscribe(subscribeCharacteristic);
                _client.BeginReliableWrite();
                _client.WriteClientConfiguration(subscribeCharacteristic, true, wclGattOperationFlag.goNone);
                _client.EndReliableWrite();
            }

            onFinished?.Invoke();
        }

        public void StopListening(string address, string service, string characteristic, Action onFinished)
        {
            if (_characteristicFromUuidFromService.TryGetValue(service, out Dictionary<string, wclGattCharacteristic> characteristicFromUuid)
             && characteristicFromUuid.TryGetValue(characteristic, out wclGattCharacteristic subscribeCharacteristic))
            {
                _onDataReceived = null;
                _client.BeginReliableWrite();
                _client.WriteClientConfiguration(subscribeCharacteristic, false, wclGattOperationFlag.goNone);
                _client.EndReliableWrite();
                _client.Unsubscribe(subscribeCharacteristic);
            }

            onFinished?.Invoke();
        }

        public void Send(string address, string service, string characteristic, byte[] data, Action onFinished)
        {
            if (_characteristicFromUuidFromService.TryGetValue(service, out Dictionary<string, wclGattCharacteristic> characteristicFromUuid)
             && characteristicFromUuid.TryGetValue(characteristic, out wclGattCharacteristic writeCharacteristic))
            {
                _client.BeginReliableWrite();
                _client.WriteCharacteristicValue(writeCharacteristic, data);
                _client.EndReliableWrite();
            }

            onFinished?.Invoke();
        }

        public void OnUpdate() { }

        private static string GetUuid(wclGattUuid uuid)
        {
            return uuid.IsShortUuid ? uuid.ShortUuid.ToString("X4") : uuid.LongUuid.ToString();
        }

        private static string GetErrorMessage(int errorCode)
        {
            if (wclHelpers.GetErrorInfo(errorCode, out _, out _, out _, out string errorMessage))
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

            if (errorCode != wclErrors.WCL_E_SUCCESS)
            {
                errorCount++;
                errorBuilder.AppendLine(GetErrorMessage(errorCode));
            }

            errorCode = _client.ReadServices(wclGattOperationFlag.goNone, out wclGattService[] services);

            if (errorCode != wclErrors.WCL_E_SUCCESS)
            {
                errorCount++;
                errorBuilder.AppendLine(GetErrorMessage(errorCode));
            }

            if (services != null)
            {
                foreach (wclGattService service in services)
                {
                    string serviceUuid = GetUuid(service.Uuid);

                    if (MaxProController.AreUuidEqual(serviceUuid, MaxProController.ShortServiceUuid))
                    {
                        errorCode = _client.ReadCharacteristics(service, wclGattOperationFlag.goNone, out wclGattCharacteristic[] characteristics);

                        if (errorCode != wclErrors.WCL_E_SUCCESS)
                        {
                            errorCount++;
                            errorBuilder.AppendLine(GetErrorMessage(errorCode));
                        }

                        if (characteristics != null)
                        {
                            Dictionary<string, wclGattCharacteristic> characteristicFromUuid = new Dictionary<string, wclGattCharacteristic>();
                            _characteristicFromUuidFromService[serviceUuid] = characteristicFromUuid;

                            string subscribeCharacteristic = null;
                            string writeCharacteristic = null;

                            foreach (wclGattCharacteristic characteristic in characteristics)
                            {
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

        private void HandleDeviceFound(object sender, wclBluetoothRadio radio, long address)
        {
            Device device = new Device()
            {
                Address = address,
                Radio = radio,
            };

            _deviceFromAddress[device.ToString()] = device;
        }

        private void HandleDiscoveringCompleted(object sender, wclBluetoothRadio radio, int errorCode)
        {
            foreach (KeyValuePair<string, Device> pair in _deviceFromAddress)
            {
                radio.GetRemoteName(pair.Value.Address, out pair.Value.Name);
                _onDeviceFound?.Invoke(pair.Key, pair.Value.Name);
            }

            _onDeviceFound = null;
        }
#endif
    }
}

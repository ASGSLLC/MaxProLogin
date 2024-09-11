using MaxProFitness.Sdk;
using System;
using System.Diagnostics;
using UnityEngine.Scripting;

namespace MaxProFitness.Integrations
{
    [Preserve]
    public sealed class BluetoothLowEnergyHardwareInterfaceConnectionHandler : IMaxProConnectionHandler
    {
        public void Initialize(Action onSucceed, Action<string> onFailed)
        {
            // ReSharper disable once InvocationIsSkipped
            RequestAndroidPermission();

            BluetoothLEHardwareInterface.Initialize(true, false, delegate
            {
                onSucceed?.Invoke();
            }, delegate(string error)
            {
                onFailed?.Invoke(error);
            });
        }

        public void Dispose(Action onFinished)
        {
            BluetoothLEHardwareInterface.DeInitialize(onFinished);
        }

        public void StartScan(Action<string, string> onDeviceFound)
        {
            BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(null, onDeviceFound);
        }

        public void StopScan()
        {
            BluetoothLEHardwareInterface.StopScan();
        }

        public void Connect(string address, string deviceName, Action<string, string, string> onSucceed, Action<string> onFailed)
        {
            string subscribeCharacteristic = null;
            string writeCharacteristic = null;

            try
            {
                BluetoothLEHardwareInterface.ConnectToPeripheral(address, null, null, delegate(string _, string service, string characteristic)
                {
                    if (!MaxProController.AreUuidEqual(service, MaxProController.ShortServiceUuid))
                    {
                        return;
                    }

                    subscribeCharacteristic = MaxProController.AreUuidEqual(characteristic, MaxProController.ShortSubscribeCharacteristicUuid) ? characteristic : subscribeCharacteristic;
                    writeCharacteristic = MaxProController.AreUuidEqual(characteristic, MaxProController.ShortWriteCharacteristicUuid) ? characteristic : writeCharacteristic;

                    if (subscribeCharacteristic != null && writeCharacteristic != null)
                    {
                        onSucceed?.Invoke(service, subscribeCharacteristic, writeCharacteristic);
                    }
                }, delegate(string error)
                {
                    onFailed?.Invoke(error);
                });
            }
            catch (Exception e)
            {
                onFailed?.Invoke(e.Message);
            }
        }

        public void Disconnect(string address, Action onFinished)
        {
            BluetoothLEHardwareInterface.DisconnectPeripheral(address, delegate
            {
                onFinished?.Invoke();
            });
        }

        public void StartListening(string address, string service, string characteristic, Action onFinished, Action<byte[]> onDataReceived)
        {
            BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(address, service, characteristic, delegate
            {
                onFinished?.Invoke();
            }, delegate(string n, string c, byte[] data)
            {
                onDataReceived?.Invoke(data);
            });
        }

        public void StopListening(string address, string service, string characteristic, Action onFinished)
        {
            BluetoothLEHardwareInterface.UnSubscribeCharacteristic(address, service, characteristic, delegate
            {
                onFinished?.Invoke();
            });
        }

        public void Send(string address, string service, string characteristic, byte[] data, Action onFinished)
        {
            BluetoothLEHardwareInterface.WriteCharacteristic(address, service, characteristic, data, data.Length, true, delegate
            {
                onFinished?.Invoke();
            });
        }

        public void OnUpdate() { }

        [Conditional("UNITY_ANDROID")]
        private static void RequestAndroidPermission()
        {
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation))
            {
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
            }
        }
    }
}

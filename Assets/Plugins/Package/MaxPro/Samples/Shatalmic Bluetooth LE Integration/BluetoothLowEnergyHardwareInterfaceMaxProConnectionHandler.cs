using MaxProFitness.Sdk;
using System;
using System.Diagnostics;
using UnityEngine.Android;
using UnityEngine.Scripting;
using MaxProFitness.Integrations;

namespace maxprofitness.login
{
    [Preserve]
    public sealed class BluetoothLowEnergyHardwareInterfaceConnectionHandler : IMaxProConnectionHandler
    {
        public void Initialize(Action onSucceed, Action<string> onFailed)
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Initialize// ");

#if ISMOBILE
            BluetoothLEHardwareInterface.RequestAndroidPermission(
            () =>
            {
#endif
                BluetoothLEHardwareInterface.Initialize(true, false, delegate
                {
                    //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Initialize// onSucceed");

                    onSucceed?.Invoke();
                }, delegate (string error)
                {
                    //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Initialize// onFailed " + error);

                    onFailed?.Invoke(error);
                });
#if ISMOBILE
            },
            (error) =>
            {
                onFailed?.Invoke(error);
            });
#endif
        }

        public void Dispose(Action onFinished)
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Dispose// ");

            BluetoothLEHardwareInterface.DeInitialize(onFinished);
        }

        public void MakeLEPairRequest(string deviceName, Action<string, string> onDeviceFound)
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//StartScan// ");

            BluetoothLEHardwareInterface.MakeLEPairRequest(deviceName, null, onDeviceFound);
        }

        /*public void StopScan()
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//StopScan// ");

            BluetoothLEHardwareInterface.StopScan();
        }*/

        public void Connect(string address, string deviceName, Action<string, string, string> onSucceed, Action<string> onFailed)
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Connect// address " + address + " deviceName " + deviceName);

            string subscribeCharacteristic = null;
            string writeCharacteristic = null;

            try
            {
                BluetoothLEHardwareInterface.ConnectToPeripheral(address, null, null, delegate (string _, string service, string characteristic)
                {
                    //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//ConnectToPeripheral// service " + service + " characteristic " + characteristic);
                    //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//ConnectToPeripheral// MaxProController.ShortServiceUuid " + MaxProController.ShortServiceUuid);

                    //if (!MaxProController.AreUuidEqual(service, MaxProController.ShortServiceUuid))
                    //{
                    //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//AreUuidEqual// false return");

                    //return;
                    //}

                    //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//AreUuidEqual// true");

                    subscribeCharacteristic = MaxProController.AreUuidEqual(characteristic, MaxProController.ShortSubscribeCharacteristicUuid) ? characteristic : subscribeCharacteristic;
                    writeCharacteristic = MaxProController.AreUuidEqual(characteristic, MaxProController.ShortWriteCharacteristicUuid) ? characteristic : writeCharacteristic;

                    if (subscribeCharacteristic != null && writeCharacteristic != null)
                    {
                        //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//ConnectToPeripheral//onSucceed");

                        onSucceed?.Invoke(service, subscribeCharacteristic, writeCharacteristic);
                    }
                }, delegate (string error)
                {
                    //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//ConnectToPeripheral//onFailed");

                    onFailed?.Invoke(error);
                });
            }
            catch (Exception e)
            {
                //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//catch// e.Message " + e.Message);

                onFailed?.Invoke(e.Message);
            }
        }

        public void Disconnect(string address, Action onFinished)
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Disconnect// ");

            BluetoothLEHardwareInterface.DisconnectPeripheral(address, delegate
            {
                //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Disconnect//onFinished ");

                onFinished?.Invoke();
            });
        }

        public void StartListening(string address, string service, string characteristic, Action onFinished, Action<byte[]> onDataReceived)
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//StartListening// ");

            BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(address, service, characteristic, delegate
            {
                //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//StartListening// onFinished");

                onFinished?.Invoke();
            }, delegate (string n, string c, byte[] data)
            {
                //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//StartListening// onDataReceived");

                onDataReceived?.Invoke(data);
            });
        }

        public void StopListening(string address, string service, string characteristic, Action onFinished)
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//StopListening// ");

            BluetoothLEHardwareInterface.UnSubscribeCharacteristic(address, service, characteristic, delegate
            {
                //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//StopListening// onFinished");

                onFinished?.Invoke();
            });
        }

        public void Send(string address, string service, string characteristic, byte[] data, Action onFinished)
        {
            //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Send// ");

            BluetoothLEHardwareInterface.WriteCharacteristic(address, service, characteristic, data, data.Length, true, delegate
            {
                //UnityEngine.Debug.Log("BluetoothLowEnergyHardwareInterfaceConnectionHandler//Send// onFinished");

                onFinished?.Invoke();
            });
        }

        public void OnUpdate() { }


    }
}

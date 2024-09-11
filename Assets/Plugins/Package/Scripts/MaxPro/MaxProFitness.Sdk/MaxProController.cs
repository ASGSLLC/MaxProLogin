using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
//using maxprofitness.login;
using MaxProFitness.Sdk;

namespace maxprofitness.login
{
    [DisallowMultipleComponent]
    [AddComponentMenu("MAXPRO Fitness/Max Pro Controller")]
    public sealed class MaxProController : MonoBehaviour
    {
        public delegate void AppCommandHandler(MaxProController sender, CommandType type, string command);

        public delegate void AppCommandHandler<in T>(MaxProController sender, T command)
            where T : struct, IAppCommand;

        public delegate void EventHandler(MaxProController sender);

        public delegate void MaxProCommandHandler(MaxProController sender, CommandType type, byte[] command);

        public delegate void MaxProCommandHandler<in T>(MaxProController sender, T command)
            where T : struct, IMaxProCommand;

        private static class MaxProCommandRouter<T>
            where T : struct, IMaxProCommand
        {
            internal static readonly Dictionary<MaxProController, MaxProCommandHandler<T>> EventMap = new Dictionary<MaxProController, MaxProCommandHandler<T>>();
            internal static readonly Dictionary<MaxProController, T> HalfPacketMap = new Dictionary<MaxProController, T>();
        }

        [PublicAPI]
        public event EventHandler OnGameModeChanged;

        [PublicAPI]
        public event MaxProCommandHandler OnMaxProCommandReceived;

        [PublicAPI]
        public event EventHandler OnStateChanged;

        [SerializeField] private InterfaceField<IMaxProConnectionHandler> _defaultConnectionHandler;
        [SerializeField] private bool _autoSendEventCommand = true;
        [SerializeField] private float _scanTimeout = 15;
        [SerializeField] private float _gameModeTimeout = 3;
        [SerializeField] private float _powerOffTimeout = 3;
        [SerializeField] private string _deviceAddress;
        [SerializeField] private string _deviceName;
        [SerializeField] private string _serviceUuid;
        [SerializeField] private string _subscribeCharacteristicUuid;
        [SerializeField] private string _writeCharacteristicUuid;
        [SerializeField] private MaxProControllerState _state;
        [SerializeField] private bool _isGameModeOn;
        [SerializeField] private InterfaceField<IAppCommand> _debugAppCommand;

        [PublicAPI]
        public const string DefaultDeviceName = "MAXPRO";

        [PublicAPI]
        public const string ShortServiceUuid = "FFF0";

        [PublicAPI]
        public const string ShortSubscribeCharacteristicUuid = "FFF1";

        [PublicAPI]
        public const string ShortWriteCharacteristicUuid = "FFF2";

        private const float ScanInvokeDelay = 0.1f;
        private const string AppCommandFormat = "5B{0}5D";
        private const string MaxProCommandFormat = "7B{0}7D";
        private bool _sendDisconnectCommand;
        private float _currentGameModeTimeout;
        private float _nextScanTime;
        private IMaxProConnectionHandler _connectionHandler;
        private Coroutine _powerOffCoroutine;

        [PublicAPI]
        public string DeviceAddress
        {
            get => _deviceAddress;
            private set => _deviceAddress = value;
        }

        [PublicAPI]
        public string DeviceName
        {
            get => _deviceName;
            private set => _deviceName = value;
        }

        [PublicAPI]
        public string ServiceUuid
        {
            get => _serviceUuid;
            private set => _serviceUuid = value;
        }

        [PublicAPI]
        public string SubscribeCharacteristicUuid
        {
            get => _subscribeCharacteristicUuid;
            private set => _subscribeCharacteristicUuid = value;
        }

        [PublicAPI]
        public string WriteCharacteristicUuid
        {
            get => _writeCharacteristicUuid;
            private set => _writeCharacteristicUuid = value;
        }

        [PublicAPI]
        public bool AutoSendEventCommand
        {
            get => _autoSendEventCommand;
            set => _autoSendEventCommand = value;
        }

        [PublicAPI]
        public float GameModeTimeout
        {
            get => _gameModeTimeout;
            set => _gameModeTimeout = value;
        }

        [PublicAPI]
        public bool IsGameModeOn
        {
            get => _isGameModeOn;
            private set
            {
                if (_isGameModeOn != value)
                {
                    _isGameModeOn = value;
                    OnGameModeChanged?.Invoke(this);
                }

                if (value)
                {
                    _currentGameModeTimeout = _gameModeTimeout;
                }
            }
        }

        [PublicAPI]
        public float PowerOffTimeout
        {
            get => _powerOffTimeout;
            set => _powerOffTimeout = value;
        }

        [PublicAPI]
        public float ScanTimeout
        {
            get => _scanTimeout;
            set => _scanTimeout = value;
        }

        [PublicAPI]
        public MaxProControllerState State
        {
            get => _state;
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged?.Invoke(this);
                }
            }
        }

        [PublicAPI]
        public IMaxProConnectionHandler DefaultConnectionHandler
        {
            get => _defaultConnectionHandler.Value;
            set => _defaultConnectionHandler = new InterfaceField<IMaxProConnectionHandler>(value);
        }

        internal IAppCommand DebugAppCommand => _debugAppCommand.Value;

        [PublicAPI]
        public static bool AreUuidEqual(string a, string b)
        {
            const string uuidFormat = "0000{0}00001000800000805f9b34fb";
            const string invalidChars = "[^a-zA-Z0-9]";

            if (a.Length == 4)
            {
                a = string.Format(uuidFormat, a);
            }
            else
            {
                a = Regex.Replace(a, invalidChars, "", RegexOptions.Compiled);
            }

            if (b.Length == 4)
            {
                b = string.Format(uuidFormat, b);
            }
            else
            {
                b = Regex.Replace(b, invalidChars, "", RegexOptions.Compiled);
            }

            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        [PublicAPI]
        public void AddMaxProCommandReceivedListener<T>(MaxProCommandHandler<T> callback)
            where T : struct, IMaxProCommand
        {
            if (MaxProCommandRouter<T>.EventMap.TryGetValue(this, out MaxProCommandHandler<T> value))
            {
                MaxProCommandRouter<T>.EventMap[this] = value + callback;
            }
            else
            {
                MaxProCommandRouter<T>.EventMap.Add(this, callback);
            }
        }

        [PublicAPI]
        public void Disconnect(Action OnComplete = null)
        {
            //Debug.Log("MaxProController//Disconnect// State " + State.ToString());

            if (State == MaxProControllerState.Disconnecting || State == MaxProControllerState.Disconnected)
            {
                OnComplete?.Invoke();

                return;
            }

            switch (State)
            {
                case MaxProControllerState.Initializing:
                    {
                        //Debug.Log("MaxProController//Disconnect// Initializing calling Dispose ");

                        _connectionHandler.Dispose(() =>
                        {
                            Reset();
                            OnComplete?.Invoke();
                        });

                        break;
                    }

                case MaxProControllerState.Scanning:
                    {
                        //Debug.Log("MaxProController//Disconnect// Scanning calling StopScan ");

                        //_connectionHandler.StopScan();
                        _connectionHandler.Dispose(() =>
                        {
                            Reset();
                            OnComplete?.Invoke();
                        });

                        break;
                    }

                case MaxProControllerState.Connecting:
                    {
                        //Debug.Log("MaxProController//Disconnect// Connecting calling Disconnect ");

                        _connectionHandler.Disconnect(DeviceAddress, delegate
                        {
                            //Debug.Log("MaxProController//Disconnect// Connecting Disconnect done Dispose ");

                            _connectionHandler.Dispose(() =>
                            {
                                Reset();
                                OnComplete?.Invoke();
                            });
                        });

                        break;
                    }

                case MaxProControllerState.Subscribing:
                    {
                        //Debug.Log("MaxProController//Disconnect// Subscribing StopListening ");

                        _connectionHandler.StopListening(DeviceAddress, ServiceUuid, SubscribeCharacteristicUuid, delegate
                        {
                            //Debug.Log("MaxProController//Disconnect// Subscribing StoppedListening ");

                            _connectionHandler.Disconnect(DeviceAddress, delegate
                            {
                                _connectionHandler.Dispose(() =>
                                {
                                    Reset();
                                    OnComplete?.Invoke();
                                });
                            });
                        });

                        break;
                    }

                case MaxProControllerState.Validating:
                    {
                        //Debug.Log("MaxProController//Disconnect// Validating StopListening ");

                        _sendDisconnectCommand = true;
                        _connectionHandler.StopListening(DeviceAddress, ServiceUuid, SubscribeCharacteristicUuid, delegate
                        {
                            //Debug.Log("MaxProController//Disconnect// Validating StoppedListening ");

                            _connectionHandler.Disconnect(DeviceAddress, delegate
                            {
                                //Debug.Log("MaxProController//Disconnect// Validating Disconnected ");

                                _connectionHandler.Dispose(() =>
                                {
                                    Reset();
                                    OnComplete?.Invoke();
                                });
                            });
                        });

                        break;
                    }

                case MaxProControllerState.Connected:
                    {
                        //Debug.Log("MaxProController//Disconnect// Connected  ");

                        SendAppCommand(new GameEventRequestAppCommand(false), true, null);
                        SendAppCommand(new DisconnectAppCommand(), true, null);
                        _connectionHandler.StopListening(DeviceAddress, ServiceUuid, SubscribeCharacteristicUuid, delegate
                        {
                            //Debug.Log("MaxProController//Disconnect// Connected StopppedListening ");

                            _connectionHandler.Disconnect(DeviceAddress, delegate
                            {
                                //Debug.Log("MaxProController//Disconnect// Connected Disconnect ");

                                _connectionHandler.Dispose(() =>
                                {
                                    Reset();
                                    OnComplete?.Invoke();
                                });
                            });
                        });

                        break;
                    }


                default:
                   // Debug.Log("MaxProController//Disconnect// default ");

                    Reset();
                    OnComplete?.Invoke();

                    break;
            }

            if (State == MaxProControllerState.Disconnecting || State == MaxProControllerState.Disconnected)
            {
                return;
            }

            State = MaxProControllerState.Disconnecting;
        }

        [PublicAPI]
        public void Initialize(Action action, Action<string> actionFailed, IMaxProConnectionHandler connectionHandler = null, string deviceName = DefaultDeviceName)
        {
            //Debug.Log("MaxProController//Initialize// ");

            if (State != MaxProControllerState.Disconnected)
            {
                //Debug.LogWarning("MaxProController//Initialize// State not disconnected ", gameObject);

             //return;
            }

            _connectionHandler = connectionHandler != null ? connectionHandler : DefaultConnectionHandler;

            if (_connectionHandler == null)
            {
                //Debug.LogError("MaxProController//Initialize//ConnectionHandler is null and no default ConnectionHandler was defined!", gameObject);

                return;
            }

            DeviceName = deviceName;

            void onInitializeSucceed()
            {
                Debug.Log($"MaxProController//onInitializeSucceed//");
                State = MaxProControllerState.Initializing;

                action?.Invoke();

                Debug.Log($"MaxProController//onInitializeSucceed// scan ");

                Invoke(nameof(MakeLEPairRequest), ScanInvokeDelay);
            }

            void onInitializeFailed(string message)
            {
                Debug.LogWarning($"MaxProController//onInitializeFailed//Failed to initialize the bluetooth connection: {message}", gameObject);

                Disconnect(() =>
                {
                    actionFailed?.Invoke(message);
                });
            }

            _connectionHandler.Initialize(onInitializeSucceed, onInitializeFailed);
        }

        [PublicAPI]
        public void RemoveMaxProCommandReceivedListener<T>(MaxProCommandHandler<T> callback)
            where T : struct, IMaxProCommand
        {
            //Debug.Log("MaxProController//RemoveMaxProCommandReceivedListener// ");

            if(MaxProCommandRouter<T>.EventMap == null)
            {
               // Debug.Log("MaxProController.cs // RemoveMaxProCommandReceivedListener() // EventMap Value: " + MaxProCommandRouter<T>.EventMap);
            }

            if (MaxProCommandRouter<T>.EventMap.TryGetValue(this, out MaxProCommandHandler<T> value))
            {
                // ReSharper disable once DelegateSubtraction
                MaxProCommandRouter<T>.EventMap[this] = value - callback;
            }
        }

        [PublicAPI]
        public bool SendAppCommand<T>(T command, AppCommandHandler<T> onAppCommandSent = null)
            where T : struct, IAppCommand
        {
            //Debug.Log("MaxProController//SendAppCommand370//SendAppCommand " + command.ToString());

            return SendAppCommand(command, false, onAppCommandSent);
        }

        [PublicAPI]
        public bool SendAppCommand(CommandType commandType, string hexCommandData, AppCommandHandler onAppCommandSent = null)
        {
            //Debug.Log("MaxProController//SendAppCommand376//SendAppCommand " + commandType.ToString() + " Hex " + hexCommandData);

            return SendAppCommand(commandType, hexCommandData, false, onAppCommandSent);
        }

        [PublicAPI]
        public void SimulateMaxProCommand<T>(T command)
            where T : struct, IMaxProCommand
        {
            SimulateMaxProCommand(command.CommandType, command.ToHexData());
        }

        [PublicAPI]
        public void SimulateMaxProCommand(CommandType commandType, string hexCommandData)
        {
            byte[] data = ConversionUtility.ToBytes(string.Format(MaxProCommandFormat, $"{(byte)commandType:x2}{hexCommandData}"));
            ReceiveMaxProCommand(data);
        }

        private void Reset()
        {
            //Debug.Log("MaxProController//Reset//");

            _defaultConnectionHandler = GetComponent<SimulatorConnectionHandler>();
            _sendDisconnectCommand = false;
            DeviceAddress = null;
            DeviceName = null;
            IsGameModeOn = false;
            State = MaxProControllerState.Disconnected;
        }

       /* private void Update()
        {
            if (_connectionHandler == null || (_connectionHandler is UnityEngine.Object o && o == null))
            {
                return;
            }

            _connectionHandler.OnUpdate();

            if (IsGameModeOn)
            {
                _currentGameModeTimeout -= Time.deltaTime;

                if (_currentGameModeTimeout <= 0)
                {
                    Debug.Log($"No response received after {_gameModeTimeout} seconds.", gameObject);
                    _connectionHandler.Dispose(Reset);
                }
            }
            else
            {
                _currentGameModeTimeout = _gameModeTimeout;
            }

            if (State == MaxProControllerState.Scanning && Time.time >= _nextScanTime)
            {
                // _connectionHandler.StopScan();
                // Debug.Log($"No more devices found after {_scanTimeout} seconds.", gameObject);

                // Debug.Log($"MaxProController//Update// calling scan ");

                //Scan();
            }
        }
       */
        private void OnDestroy()
        {
            Disconnect();
        }

        private bool CanSendAppCommand(CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.Connect:
                    {
                        Debug.LogError("Connect event shouldn't be sent manually, use Connect method instead!", gameObject);

                        return false;
                    }

                case CommandType.Disconnect:
                    {
                        Debug.LogError("Disconnect event shouldn't be sent manually, use Disconnect method instead!", gameObject);

                        return false;
                    }

                default:
                    return State == MaxProControllerState.Connected;
            }
        }

        private void Connect()
        {
            State = MaxProControllerState.Connecting;
            //Debug.Log($"MaxProController//Connect// Connecting to {DeviceName}", gameObject);

            void onConnectSucceed(string service, string subscribeCharacteristic, string writeCharacteristic)
            {
                //Debug.Log($"MaxProController//onConnectSucceed// State " + State);

                if (State != MaxProControllerState.Connecting)
                {
                    return;
                }

                ServiceUuid = service;
                SubscribeCharacteristicUuid = subscribeCharacteristic;
                WriteCharacteristicUuid = writeCharacteristic;
                Subscribe();
            }

            void onConnectFailed(string message)
            {
                //Debug.LogWarning($"MaxProController//onConnectFailed//Failed to connect to {DeviceName} at {DeviceAddress}: {message}", gameObject);
                Disconnect();
            }

            _connectionHandler.Connect(DeviceAddress, DeviceName, onConnectSucceed, onConnectFailed);
        }

        private void HandleConnectEvent(MaxProController sender, CommandType type, byte[] command)
        {
            //Debug.Log($"MaxProController//HandleConnectEvent// CommandType " + type.ToString());

            OnMaxProCommandReceived -= HandleConnectEvent;

            if (sender.State == MaxProControllerState.Validating)
            {
                sender.State = MaxProControllerState.Connected;
                //Debug.Log($"MaxProController//HandleConnectEvent// {DeviceAddress} connected and subscribed to {SubscribeCharacteristicUuid}", gameObject);
            }
            else if (sender.State == MaxProControllerState.Disconnecting && _sendDisconnectCommand)
            {
                //Debug.Log($"MaxProController//HandleConnectEvent// Disconnecting");

                _sendDisconnectCommand = false;
                SendAppCommand(new GameEventRequestAppCommand(false), true, null);
                SendAppCommand(new DisconnectAppCommand(), true, null);
            }
        }

        private void InvokeMaxProCommandReceived<T>(T command, byte[] data)
            where T : struct, IMaxProCommand
        {
            //Debug.Log("MaxProController//InvokeMaxProCommandReceived// command " + command.ToString() + " data.length " + data.Length);

            InvokeMaxProCommandReceived(ref command, data);
        }

        private void InvokeMaxProCommandReceived<T>(ref T command, byte[] data)
            where T : struct, IMaxProCommand
        {
            //Debug.Log("MaxProController//InvokeMaxProCommandReceived// ref command " + command.ToString() + " data.length " + data.Length);

            if (MaxProCommandRouter<T>.HalfPacketMap.TryGetValue(this, out T existingCommand))
            {
                command = existingCommand;
                Debug.LogWarning($"MaxProController//InvokeMaxProCommandReceived//Received {data.Length} bytes for command {command.CommandType}.", gameObject);
            }

            if (command.Deserialize(data))
            {
                MaxProCommandRouter<T>.HalfPacketMap.Remove(this);

                if (MaxProCommandRouter<T>.EventMap.TryGetValue(this, out MaxProCommandHandler<T> handler))
                {
                    handler?.Invoke(this, command);
                }

                //Debug.Log($"MaxProController//InvokeMaxProCommandReceived// OnMaxProCommandReceived");

                OnMaxProCommandReceived?.Invoke(this, command.CommandType, data);
            }
            else
            {
                MaxProCommandRouter<T>.HalfPacketMap[this] = command;
                Debug.LogWarning($"MaxProController//InvokeMaxProCommandReceived//Received {data.Length} bytes for command {command.CommandType}.", gameObject);
            }
        }

        private string concatenatedHex = "";
        private string hex1 = "";
        private string hex2 = "";
        private CommandType cmd;
        private void ReceiveMaxProCommand(byte[] data)
        {
            string hexConversion = ConversionUtility.ToHex(data, 0, data.Length);

            //Debug.Log($"MaxProController//ReceiveMaxProCommand// Received " + hexConversion);
            //Debug.Log($"MaxProController//ReceiveMaxProCommand// data length " + data.Length);

            if (State != MaxProControllerState.Connected)
            {
                if (data.Length < 2 || data[1] != (byte)CommandType.Connect)
                {
                    Debug.LogWarning($"MaxProController//ReceiveMaxProCommand// Received data when not connected: {ConversionUtility.ToHex(data, 0, data.Length)}", gameObject);
                }
            }
            else if (_powerOffCoroutine != null)
            {
                StopCoroutine(_powerOffCoroutine);

                _powerOffCoroutine = null;
            }

            //if ((hexConversion.Contains("7b") && !hexConversion.Contains("7d")) || (hexConversion.Contains("7b") && hexConversion.Contains("7d")))
            cmd = (CommandType)data[1];

            //CommandType type = CommandType.Connect;

           // Debug.Log("MaxProController//ReceiveMaxProCommand//type " + cmd.ToString());

            switch (cmd)
            {
                case CommandType.Connect:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.Connect");

                        InvokeMaxProCommandReceived(new ConnectMaxProCommand(), data);

                        break;
                    }

                case CommandType.StatusRequest:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.StatusRequest");

                        InvokeMaxProCommandReceived(new StatusRequestMaxProCommand(), data);

                        break;
                    }

                case CommandType.TotalRepsClear:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.TotalRepsClear");

                        InvokeMaxProCommandReceived(new TotalRepsClearMaxProCommand(), data);

                        break;
                    }

                case CommandType.SetKnobActuatorPosition:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.SetKnobActuatorPosition");

                        InvokeMaxProCommandReceived(new SetKnobActuatorPositionMaxProCommand(), data);

                        break;
                    }

                case CommandType.SetWindingStartPosition:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.SetWindingStartPosition");

                        InvokeMaxProCommandReceived(new SetWindingStartPositionMaxProCommand(), data);

                        break;
                    }

                case CommandType.MinimumLengthToPull:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.MinimumLengthToPull");

                        InvokeMaxProCommandReceived(new MinimumLenghtToPullMaxProCommand(), data);

                        break;
                    }

                case CommandType.Event:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.Event");

                        EventCommand command = new EventCommand();
                        InvokeMaxProCommandReceived(ref command, data);

                        if (_autoSendEventCommand)
                        {
                            SendAppCommand(command, true, null);
                        }

                        if (command.UserPowerOff || command.BatteryPowerOff)
                        {
                            if (_powerOffCoroutine == null)
                            {
                                IEnumerator coroutine()
                                {
                                    yield return new WaitForSecondsRealtime(_powerOffTimeout);

                                    Disconnect();

                                    _powerOffCoroutine = null;
                                }

                                _powerOffCoroutine = StartCoroutine(coroutine());
                            }
                        }

                        break;
                    }

                case CommandType.KnobPositionCalibration:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.KnobPositionCalibration");

                        InvokeMaxProCommandReceived(new KnobPositionCalibrationMaxProCommand(), data);

                        break;
                    }

                case CommandType.Disconnect:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.Disconnect");

                        InvokeMaxProCommandReceived(new DisconnectMaxProCommand(), data);

                        break;
                    }

                case CommandType.GameEventRequest:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.GameEventRequest");

                        GameEventRequestMaxProCommand command = new GameEventRequestMaxProCommand();
                        InvokeMaxProCommandReceived(ref command, data);

                        IsGameModeOn = command.IsGameModeOn;

                        break;
                    }

                case CommandType.GameEventRequestUpdate:
                    {
                        //Debug.Log("MaxProController//ReceiveMaxProCommand// CommandType.GameEventRequestUpdate");

                        IsGameModeOn = true;
                        InvokeMaxProCommandReceived(new GameEventRequestUpdateMaxProCommand(), data);

                        break;
                    }
            }
        }

        private void MakeLEPairRequest()
        {
            //Debug.Log($"MaxProController//MakeLEPairRequest//");

            _nextScanTime = Time.time + _scanTimeout;
            State = MaxProControllerState.Scanning;
            //Debug.Log("Scanning devices to connect.", gameObject);

            void onDeviceFound(string deviceAddress, string deviceName)
            {
                _nextScanTime = Time.time + _scanTimeout;

                if (State != MaxProControllerState.Scanning || deviceName == null || !deviceName.Contains(DeviceName))
                {
                    Debug.Log($"Skipping {deviceName} at {deviceAddress} while searching for {DeviceName}", gameObject);

                    return;
                }

                DeviceAddress = deviceAddress;
                //_connectionHandler.StopScan();
                Debug.Log($"Found {deviceName} at {deviceAddress}", gameObject);
                Connect();
            }

            _connectionHandler.MakeLEPairRequest("MAXPRO", onDeviceFound);
        }

        private bool SendAppCommand<T>(T command, bool bypassValidation, AppCommandHandler<T> onAppCommandSent)
            where T : struct, IAppCommand
        {
            if (!bypassValidation && !CanSendAppCommand(command.CommandType))
            {
                return false;
            }

            string hexCommand = string.Format(AppCommandFormat, $"{(byte)command.CommandType:x2}{command.ToHexData()}");
            byte[] bytes = ConversionUtility.ToBytes(hexCommand);
            _connectionHandler.Send(DeviceAddress, ServiceUuid, WriteCharacteristicUuid, bytes, delegate
            {
                //Debug.Log($"MaxProController//SendAppCommand1// Sent {hexCommand} ({command.CommandType}) to {WriteCharacteristicUuid} in {ServiceUuid} at {DeviceAddress}", gameObject);
                onAppCommandSent?.Invoke(this, command);

                if (command.CommandType == CommandType.SetWifiModuleName)
                {
                    Disconnect();
                }
            });

            return true;
        }

        private bool SendAppCommand(CommandType commandType, string hexCommandData, bool bypassValidation, AppCommandHandler onAppCommandSent)
        {
            if (!bypassValidation && !CanSendAppCommand(commandType))
            {
                return false;
            }

            string hexCommand = string.Format(AppCommandFormat, $"{(byte)commandType:x2}{hexCommandData}");
            byte[] bytes = ConversionUtility.ToBytes(hexCommand);
            _connectionHandler.Send(DeviceAddress, ServiceUuid, WriteCharacteristicUuid, bytes, delegate
            {
                //Debug.Log($"MaxProController//SendAppCommand2// Sent {hexCommand} ({commandType}) to {WriteCharacteristicUuid} in {ServiceUuid} at {DeviceAddress}", gameObject);
                onAppCommandSent?.Invoke(this, commandType, hexCommand);

                if (commandType == CommandType.SetWifiModuleName)
                {
                    Disconnect();
                }
            });

            return true;
        }

        private void Subscribe()
        {
            State = MaxProControllerState.Subscribing;
            //Debug.Log($"MaxProController//Subscribe// Subscribing to {SubscribeCharacteristicUuid} in {ServiceUuid} at {DeviceAddress}", gameObject);

            void onStartListeningFinished()
            {
                //Debug.Log($"MaxProController//onStartListeningFinished// State " + State);

                if (State != MaxProControllerState.Subscribing)
                {
                    return;
                }

                State = MaxProControllerState.Validating;
                OnMaxProCommandReceived -= HandleConnectEvent;
                OnMaxProCommandReceived += HandleConnectEvent;
                //Debug.Log($"MaxProController//onStartListeningFinished// Validating connection to {SubscribeCharacteristicUuid} in {ServiceUuid} at {DeviceAddress}", gameObject);
                SendAppCommand(new ConnectAppCommand(), true, null);
            }

            _connectionHandler.StartListening(DeviceAddress, ServiceUuid, SubscribeCharacteristicUuid, onStartListeningFinished, ReceiveMaxProCommand);
        }
    }
}

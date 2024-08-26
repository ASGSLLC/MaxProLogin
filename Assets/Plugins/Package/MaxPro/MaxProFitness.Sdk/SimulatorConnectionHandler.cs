using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaxProFitness.Sdk
{
    [DisallowMultipleComponent]
    [AddComponentMenu("MAXPRO Fitness/Simulator Connection Handler")]
    [RequireComponent(typeof(MaxProController))]
    public sealed class SimulatorConnectionHandler : MonoBehaviour, IMaxProConnectionHandler
    {
        [SerializeField] private SimulatorCanvas _canvas;
        [SerializeField] private SimulatorCanvas _canvasPrefab;
        [SerializeField] private Key _bothHandsKey = Key.LeftAlt;
        [SerializeField] private bool _isGameModeOn;
        [SerializeField] [Range(0, 100)] private byte _batteryPercent = 100;
        [SerializeField] [Min(0)] private float _initializeDelay = 0.5f;
        [SerializeField] private bool _canInitialize = true;
        [SerializeField] [Min(0)] private float _scanDelay = 0.5f;
        [SerializeField] private string _scanDeviceAddress = "SIMULATOR";
        [SerializeField] private string _scanDeviceName = MaxProController.DefaultDeviceName;
        [SerializeField] [Min(0)] private float _connectDelay = 0.5f;
        [SerializeField] private bool _canConnect = true;
        [SerializeField] [Min(0)] private float _listenDelay = 0.5f;
        [SerializeField] [Min(0)] private float _sendDelay = 0.5f;
        [SerializeField] private EventCommand _lastEventCommand;
        [SerializeField] private EventCommand _nextEventCommand;

        [SerializeField] [HideInInspector]
        private MaxProController _maxProController;

        private const ushort EventDistance = 100;
        private const ushort MaxDistance = 2500;
        private const ushort MaxKnobPosition = 50;
        private const ushort MinDistance = 0;
        private const ushort MinKnobPosition = 1;
        private const float DisconnectEventTimer = 600;
        private const float ExerciseTimeInterval = 60;
        private const float GameEventRequestUpdateInterval = 0.04f;
        private const float RetryLastEventInterval = 1;
        private bool _isLeftHandActive;
        private bool _isRightHandActive;
        private float _lastLeftHandEventTime;
        private float _lastRightHandEventTime;
        private float _disconnectEventTimer;
        private float? _retryLastEventTimer;
        private EventCommand _appEventCommand;
        private SimulatorSlider _mainHandSlider;
        private Action _onDeviceFound;
        private Action<byte[]> _onDataReceived;

        public void Initialize(Action onSucceed, Action<string> onFailed)
        {
            Execute(_initializeDelay, delegate
            {
                if (_canInitialize)
                {
                    if (_canvasPrefab != null)
                    {
                        _canvas = Instantiate(_canvasPrefab, transform);
                        _canvas.gameObject.SetActive(false);

                        _canvas.LeftHandSlider.OnReleased += HandleHandSliderReleased;
                        _canvas.LeftHandSlider.Slider.onValueChanged.AddListener(HandleLeftHandSliderValueChanged);
                        _canvas.LeftKnobSlider.Slider.onValueChanged.AddListener(HandleLeftKnobSliderValueChanged);

                        _canvas.RightHandSlider.OnReleased += HandleHandSliderReleased;
                        _canvas.RightHandSlider.Slider.onValueChanged.AddListener(HandleRightHandSliderValueChanged);
                        _canvas.RightKnobSlider.Slider.onValueChanged.AddListener(HandleRightKnobSliderValueChanged);

                        _nextEventCommand.LeftKnobPosition = ConvertSliderValueToKnobPosition(_canvas.LeftKnobSlider.Slider.value);
                        _nextEventCommand.RightKnobPosition = ConvertSliderValueToKnobPosition(_canvas.RightKnobSlider.Slider.value);
                    }

                    onSucceed?.Invoke();
                }
                else
                {
                    onFailed?.Invoke("Simulating an initialization failure!");
                }
            });
        }

        public void Dispose(Action onFinished)
        {
            CleanUp();
            onFinished?.Invoke();
        }

        public void Connect(string address, string deviceName, Action<string, string, string> onSucceed, Action<string> onFailed)
        {
            Action succeedCallback = () => onSucceed?.Invoke(MaxProController.ShortServiceUuid, MaxProController.ShortSubscribeCharacteristicUuid, MaxProController.ShortWriteCharacteristicUuid);
            Action failedCallback = () => onFailed?.Invoke("Simulating a connection failure!");
            Execute(_connectDelay, _canConnect ? succeedCallback : failedCallback);
        }

        public void Disconnect(string address, Action onFinished)
        {
            CleanUp();
            onFinished?.Invoke();
        }

        public void MakeLEPairRequest(string deviceName, Action<string, string> onDeviceFound)
        {
            _onDeviceFound = delegate
            {
                onDeviceFound?.Invoke(_scanDeviceAddress, _scanDeviceName);
            };

            Execute(_scanDelay, delegate
            {
                _onDeviceFound?.Invoke();
            });
        }

        public void StopScan()
        {
            _onDeviceFound = null;
        }

        public void StartListening(string address, string service, string characteristic, Action onFinished, Action<byte[]> onDataReceived)
        {
            _onDataReceived = onDataReceived;
            Execute(_listenDelay, onFinished);
        }

        public void StopListening(string address, string service, string characteristic, Action onFinished)
        {
            Execute(_listenDelay, delegate
            {
                CleanUp();
                onFinished?.Invoke();
            });
        }

        public void Send(string address, string service, string characteristic, byte[] data, Action onFinished)
        {
            _disconnectEventTimer = DisconnectEventTimer;

            CommandType commandType = (CommandType)data[1];

            switch (commandType)
            {
                case CommandType.Connect:
                {
                    Execute(_sendDelay, delegate
                    {
                        if (_canvas != null)
                        {
                            _canvas.gameObject.SetActive(true);
                        }

                        SimulateCommand(new ConnectMaxProCommand
                        {
                            Result = CommandResult.Completed,
                            Version = MaxProModel.MaxProSimulator.ToString(),
                        });

                        SetupCurrentMode();
                    });

                    break;
                }

                case CommandType.StatusRequest:
                {
                    Execute(_sendDelay, delegate
                    {
                        if (_isGameModeOn)
                        {
                            return;
                        }

                        string hexData = $"{(byte)CommandResult.Completed:x2}{_nextEventCommand.ToHexData().Substring(2)}";
                        SimulateCommand(CommandType.StatusRequest, hexData);
                    });

                    break;
                }

                case CommandType.TotalRepsClear:
                {
                    Execute(_sendDelay, delegate
                    {
                        _nextEventCommand.LeftRepsCount = 0;
                        _nextEventCommand.RightRepsCount = 0;

                        SimulateCommand(new TotalRepsClearMaxProCommand
                        {
                            Result = CommandResult.Completed,
                        });
                    });

                    break;
                }

                case CommandType.Event:
                {
                    if (!_appEventCommand.Deserialize(data))
                    {
                        break;
                    }

                    _appEventCommand = default;

                    if (_appEventCommand != _lastEventCommand)
                    {
                        break;
                    }

                    _retryLastEventTimer = null;

                    break;
                }

                case CommandType.Disconnect:
                {
                    Execute(_sendDelay, delegate
                    {
                        CleanUp();
                        SimulateCommand(new DisconnectMaxProCommand
                        {
                            Result = CommandResult.Completed,
                        });
                    });

                    break;
                }

                case CommandType.GameEventRequest:
                {
                    GameEventRequestAppCommand gameEventRequestAppCommand = new GameEventRequestAppCommand();
                    gameEventRequestAppCommand.Deserialize(data);

                    Execute(_sendDelay, delegate
                    {
                        if (_isGameModeOn == gameEventRequestAppCommand.IsGameModeOn)
                        {
                            return;
                        }

                        _isGameModeOn = gameEventRequestAppCommand.IsGameModeOn;
                        SetupCurrentMode();
                        SimulateCommand(new GameEventRequestMaxProCommand
                        {
                            IsGameModeOn = _isGameModeOn,
                        });
                    });

                    break;
                }

                default:
                {
                    Debug.LogWarning($"{nameof(SimulatorConnectionHandler)} {commandType} command type is not implement!");

                    break;
                }
            }
        }

        public void OnUpdate()
        {
            //Input.GetKeyDown(_bothHandsKey)
            if (Keyboard.current[_bothHandsKey].wasPressedThisFrame)
            {
                _canvas.LeftHandSlider.OnPressed += HandleHandSliderPressed;
                _canvas.RightHandSlider.OnPressed += HandleHandSliderPressed;
            }
            else if (Keyboard.current[_bothHandsKey].wasReleasedThisFrame)
            {
                _canvas.LeftHandSlider.OnPressed -= HandleHandSliderPressed;
                _canvas.RightHandSlider.OnPressed -= HandleHandSliderPressed;
            }
            else if (_mainHandSlider != null)
            {
                _canvas.LeftHandSlider.Slider.value = _mainHandSlider.Slider.value;
                _canvas.RightHandSlider.Slider.value = _mainHandSlider.Slider.value;
            }

            if (_maxProController.State != MaxProControllerState.Connected)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            UpdateDisconnectEventTimer(deltaTime);
            UpdateRetryLastEventTimer(deltaTime);
        }

        private void HandleHandSliderPressed(SimulatorSlider slider)
        {
            if (_mainHandSlider == null)
            {
                _mainHandSlider = slider;
            }
        }

        private void HandleHandSliderReleased(SimulatorSlider sender)
        {
            sender.Slider.value = 0;

            if (_mainHandSlider != sender)
            {
                return;
            }

            _canvas.LeftHandSlider.Slider.value = _mainHandSlider.Slider.value;
            _canvas.RightHandSlider.Slider.value = _mainHandSlider.Slider.value;
            _mainHandSlider = null;
        }

        private static ushort ConvertSliderValueToDistance(float value)
        {
            value = Mathf.Clamp01(value);

            unchecked
            {
                return (ushort)(Mathf.Lerp(MinDistance, MaxDistance, value));
            }
        }

        private static byte ConvertSliderValueToKnobPosition(float value)
        {
            value = Mathf.Clamp((int)value, MinKnobPosition, MaxKnobPosition);

            unchecked
            {
                // min is 10, max is 255
                return (byte)(5 + value * 5);
            }
        }

        private void OnValidate()
        {
            _maxProController = GetComponent<MaxProController>();
            _nextEventCommand.BatteryPercent = _batteryPercent;
        }

        private void CleanUp()
        {
            if (_canvas != null)
            {
                _canvas.LeftHandSlider.OnReleased -= HandleHandSliderReleased;
                _canvas.LeftHandSlider.Slider.onValueChanged.RemoveListener(HandleLeftHandSliderValueChanged);
                _canvas.LeftKnobSlider.Slider.onValueChanged.RemoveListener(HandleLeftKnobSliderValueChanged);

                _canvas.RightHandSlider.OnReleased -= HandleHandSliderReleased;
                _canvas.RightHandSlider.Slider.onValueChanged.RemoveListener(HandleRightHandSliderValueChanged);
                _canvas.RightKnobSlider.Slider.onValueChanged.RemoveListener(HandleRightKnobSliderValueChanged);

                Destroy(_canvas.gameObject);
            }

            _onDataReceived = null;
            CancelInvoke();
            StopAllCoroutines();
        }

        private void Execute(float delay, Action action)
        {
            if (!enabled || !gameObject.activeInHierarchy)
            {
                return;
            }

            if (delay <= 0)
            {
                action.Invoke();
            }
            else
            {
                IEnumerator coroutine()
                {
                    yield return new WaitForSeconds(delay);

                    action.Invoke();
                }

                StartCoroutine(coroutine());
            }
        }

        private void HandleLeftHandSliderValueChanged(float value)
        {
            UpdateHand(ConvertSliderValueToDistance(value),
                       _lastEventCommand.LeftVelocity,
                       EventType.LeftExerciseCount,
                       ref _isLeftHandActive,
                       ref _lastLeftHandEventTime,
                       ref _nextEventCommand.LeftDistance,
                       ref _nextEventCommand.LeftTime,
                       ref _nextEventCommand.LeftVelocity,
                       ref _nextEventCommand.LeftAcceleration,
                       ref _nextEventCommand.LeftRepsCount);
        }

        private void HandleLeftKnobSliderValueChanged(float value)
        {
            _nextEventCommand.LeftKnobPosition = ConvertSliderValueToKnobPosition(value);
            _nextEventCommand.EventType |= EventType.LeftKnobPosition;
            _canvas.LeftKnobValue = ((int)value).ToString();
        }

        private void HandleRightHandSliderValueChanged(float value)
        {
            UpdateHand(ConvertSliderValueToDistance(value),
                       _lastEventCommand.RightVelocity,
                       EventType.LeftExerciseCount,
                       ref _isRightHandActive,
                       ref _lastRightHandEventTime,
                       ref _nextEventCommand.RightDistance,
                       ref _nextEventCommand.RightTime,
                       ref _nextEventCommand.RightVelocity,
                       ref _nextEventCommand.RightAcceleration,
                       ref _nextEventCommand.RightRepsCount);
        }

        private void HandleRightKnobSliderValueChanged(float value)
        {
            _nextEventCommand.RightKnobPosition = ConvertSliderValueToKnobPosition(value);
            _nextEventCommand.EventType |= EventType.RightKnobPosition;
            _canvas.RightKnobValue = ((int)value).ToString();
        }

        private void SendGameEventRequestUpdateCommand()
        {
            GameEventRequestUpdateMaxProCommand gameEventRequestUpdateCommand = new GameEventRequestUpdateMaxProCommand
            {
                LeftDistance = _nextEventCommand.LeftDistance,
                LeftKnobPosition = _nextEventCommand.LeftKnobPosition,
                RightDistance = _nextEventCommand.RightDistance,
                RightKnobPosition = _nextEventCommand.RightKnobPosition,
            };

            _lastEventCommand = _nextEventCommand;
            SimulateCommand(gameEventRequestUpdateCommand);
        }

        private void SendExerciseTimeEvent()
        {
            _nextEventCommand.EventType |= EventType.ExerciseTime;
            SendEventCommand();
        }

        private void SendEventCommand()
        {
            SimulateCommand(_nextEventCommand);
            _retryLastEventTimer = RetryLastEventInterval;
            _lastEventCommand = _nextEventCommand;
            _nextEventCommand.EventType = 0;
        }

        private void SetupCurrentMode()
        {
            _retryLastEventTimer = null;
            _lastLeftHandEventTime = Time.time;
            _lastRightHandEventTime = Time.time;

            if (_isGameModeOn)
            {
                CancelInvoke();
                InvokeRepeating(nameof(SendGameEventRequestUpdateCommand), GameEventRequestUpdateInterval, GameEventRequestUpdateInterval);
            }
            else
            {
                CancelInvoke();
                InvokeRepeating(nameof(SendExerciseTimeEvent), ExerciseTimeInterval, ExerciseTimeInterval);
            }
        }

        private void SimulateCommand<T>(T command)
            where T : struct, IMaxProCommand
        {
            if (_onDataReceived != null)
            {
                // we don't use _onDataReceived directly due it not being type-safe
                _maxProController.SimulateMaxProCommand(command);
            }
        }

        private void SimulateCommand(CommandType commandType, string hexCommandData)
        {
            if (_onDataReceived != null)
            {
                // we don't use _onDataReceived directly due it not being type-safe
                _maxProController.SimulateMaxProCommand(commandType, hexCommandData);
            }
        }

        private void UpdateDisconnectEventTimer(float deltaTime)
        {
            _disconnectEventTimer -= deltaTime;

            if (_disconnectEventTimer > 0)
            {
                return;
            }

            Debug.LogWarning("Connection was automatically terminated!", gameObject);

            CancelInvoke();
            StopAllCoroutines();
        }

        private void UpdateHand(ushort sliderValue,
                                ushort initialVelocity,
                                EventType handEvent,
                                ref bool isActive,
                                ref float lastEventTime,
                                ref ushort distance,
                                ref ushort time,
                                ref ushort velocity,
                                ref ushort acceleration,
                                ref ushort repsCount)
        {
            if (sliderValue > distance)
            {
                isActive = true;
                distance = sliderValue;

                float deltaTime = Time.time - lastEventTime;

                unchecked
                {
                    time = (ushort)(deltaTime * 100f);
                    velocity = (ushort)(distance / deltaTime);
                    acceleration = (ushort)Mathf.Abs((velocity - initialVelocity) / deltaTime);
                }
            }
            else if (sliderValue < distance)
            {
                if (_isGameModeOn || !isActive)
                {
                    distance = sliderValue;
                }
                else if (distance - sliderValue >= EventDistance)
                {
                    _nextEventCommand.EventType |= handEvent;
                    repsCount++;
                    SendEventCommand();

                    distance = sliderValue;
                    isActive = false;
                    lastEventTime = Time.time;
                }
            }
        }

        private void UpdateRetryLastEventTimer(float deltaTime)
        {
            if (!_retryLastEventTimer.HasValue)
            {
                return;
            }

            _retryLastEventTimer -= deltaTime;

            if (_retryLastEventTimer > 0)
            {
                return;
            }

            SimulateCommand(_lastEventCommand);
        }
    }
}

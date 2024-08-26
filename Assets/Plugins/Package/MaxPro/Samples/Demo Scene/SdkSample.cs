using MaxProFitness.Integrations;
using MaxProFitness.Sdk;
using System;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using maxprofitness.login;

namespace MaxProFitness.Samples
{
    public sealed class SdkSample : MonoBehaviour
    {
        [SerializeField] private InterfaceField<IMaxProConnectionHandler> _connectionHandler;
        [SerializeField] private MaxProController _maxProController;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _disconnectButton;
        [SerializeField] private Button _receiveMaxProCommandButton;
        [SerializeField] private Button _sendAppCommandButton;
        [SerializeField] private TMP_InputField _commandTypeField;
        [SerializeField] private TMP_InputField _commandDataField;
        [SerializeField] private TMP_Text _stateLabel;
        [SerializeField] private TMP_Text _lastModeExclusiveCommandLabel;
        [SerializeField] private TMP_Text _lastNonModeExclusiveCommandLabel;
        [SerializeField] private Toggle _gameModeToggle;

        private void OnEnable()
        {
            _maxProController.OnStateChanged += HandleMaxProControllerStateChanged;
            _maxProController.OnMaxProCommandReceived += HandleMaxProControllerMaxProCommandReceived;
            _maxProController.OnGameModeChanged += HandleMaxProControllerGameModeChanged;
            _gameModeToggle.onValueChanged.AddListener(HandleGameModeToggleValueChanged);
            _connectButton.onClick.AddListener(HandleConnectButtonClick);
            _disconnectButton.onClick.AddListener(HandleDisconnectButtonClick);
            _commandTypeField.onValueChanged.AddListener(HandleAppCommandTypeFieldValueChanged);
            _sendAppCommandButton.onClick.AddListener(HandleSendAppCommandButtonClick);
            _receiveMaxProCommandButton.onClick.AddListener(HandleReceiveMaxProCommandButtonClick);
        }

        private void OnDisable()
        {
            _maxProController.OnStateChanged -= HandleMaxProControllerStateChanged;
            _maxProController.OnMaxProCommandReceived -= HandleMaxProControllerMaxProCommandReceived;
            _maxProController.OnGameModeChanged -= HandleMaxProControllerGameModeChanged;
            _gameModeToggle.onValueChanged.RemoveListener(HandleGameModeToggleValueChanged);
            _connectButton.onClick.RemoveListener(HandleConnectButtonClick);
            _disconnectButton.onClick.RemoveListener(HandleDisconnectButtonClick);
            _commandTypeField.onValueChanged.RemoveListener(HandleAppCommandTypeFieldValueChanged);
            _sendAppCommandButton.onClick.RemoveListener(HandleSendAppCommandButtonClick);
            _receiveMaxProCommandButton.onClick.RemoveListener(HandleReceiveMaxProCommandButtonClick);
        }

        private void Start()
        {
            //Debug.Log("SdkSample//Start// ");

            //_receiveMaxProCommandButton.interactable = false;
            //_sendAppCommandButton.interactable = false;
            //_gameModeToggle.interactable = false;
            _stateLabel.text = _maxProController.State.ToString();

           // _connectButton.interactable = true;
            _connectButton.gameObject.SetActive(true);

           // _disconnectButton.interactable = false;
            _disconnectButton.gameObject.SetActive(false);
        }

        private void HandleAppCommandTypeFieldValueChanged(string value)
        {
            //Debug.Log("SdkSample//HandleAppCommandTypeFieldValueChanged// value " + value);

            string commandType = _commandTypeField.text;

            if (commandType.Length < 2)
            {
                _commandTypeField.text = commandType.Length < 1 ? $"{(byte)CommandType.StatusRequest:x2}" : $"0{commandType}";
            }
            else if (commandType.Length > 2)
            {
                _commandTypeField.text = commandType.Substring(0, 2);
            }

            if (byte.TryParse(_commandTypeField.text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
            {
                return;
            }

            _commandTypeField.text = $"{(byte)CommandType.StatusRequest:x2}";
        }

        private void HandleConnectButtonClick()
        {
            //Debug.Log("SdkSample//HandleConnectButtonClick// ");

            _connectButton.gameObject.SetActive(false);
            _disconnectButton.gameObject.SetActive(true);


            if(_maxProController.DefaultConnectionHandler == null)
                _maxProController.DefaultConnectionHandler = new BluetoothLowEnergyHardwareInterfaceConnectionHandler();

            /*_maxProController.Initialize(
                () => 
                {
                    //Debug.Log("SdkSample//HandleConnectButtonClick// Initialize ");

                },
                (error) =>
                {            
                    //Debug.Log("SdkSample//HandleConnectButtonClick// error " + error);

                    _connectButton.gameObject.SetActive(true);
                  //  _connectButton.interactable = true;
                    _disconnectButton.gameObject.SetActive(false);
                    //.State = MaxProControllerState.Disconnected;
                });*/
        }

        private void HandleDisconnectButtonClick()
        {
            //Debug.Log("SdkSample//HandleDisconnectButtonClick//");

            _connectButton.gameObject.SetActive(true);
            _disconnectButton.gameObject.SetActive(false);
            _maxProController.Disconnect();
        }

        private void HandleGameModeToggleValueChanged(bool value)
        {
            //Debug.Log("SdkSample//HandleGameModeToggleValueChanged// value " + value);

            _maxProController.SendAppCommand(new GameEventRequestAppCommand
            {
                IsGameModeOn = value,
            });
        }

        private void HandleMaxProControllerGameModeChanged(MaxProController sender)
        {
            //Debug.Log("SdkSample//HandleMaxProControllerGameModeChanged// sender " + sender.name);

            _gameModeToggle.SetIsOnWithoutNotify(sender.IsGameModeOn);
        }

        private void HandleMaxProControllerMaxProCommandReceived(MaxProController sender, CommandType type, byte[] data)
        {
            //Debug.Log("SdkSample//HandleMaxProControllerMaxProCommandReceived// sender " + sender.name + " commandType " + type + " data " + data.Length);

            switch (type)
            {
                case CommandType.StatusRequest:
                {
                    StatusRequestMaxProCommand command = new StatusRequestMaxProCommand();
                    command.Deserialize(data);
                    UpdateModeExclusiveCommandLabel(type, command);

                    break;
                }

                case CommandType.Event:
                {
                    EventCommand command = new EventCommand();
                    command.Deserialize(data);
                    UpdateModeExclusiveCommandLabel(type, command);

                    break;
                }

                case CommandType.GameEventRequestUpdate:
                {
                    GameEventRequestUpdateMaxProCommand command = new GameEventRequestUpdateMaxProCommand();
                    command.Deserialize(data);
                    UpdateModeExclusiveCommandLabel(type, command);

                    break;
                }

                default:
                {
                    UpdateNonModeExclusiveCommandLabel(type, data);

                    break;
                }
            }
        }

        private void HandleMaxProControllerStateChanged(MaxProController sender)
        {
            //Debug.Log("SdkSample//HandleMaxProControllerStateChanged// sender " + sender.name);

            _stateLabel.text = _maxProController.State.ToString();
           // _receiveMaxProCommandButton.interactable = _maxProController.State == MaxProControllerState.Connected;
            //_sendAppCommandButton.interactable = _maxProController.State == MaxProControllerState.Connected;
           // _gameModeToggle.interactable = _maxProController.State == MaxProControllerState.Connected;
            //_connectButton.interactable = _maxProController.State == MaxProControllerState.Disconnected;
            //_disconnectButton.interactable = _maxProController.State != MaxProControllerState.Disconnected && _maxProController.State != MaxProControllerState.Disconnecting;

            if (sender.State == MaxProControllerState.Disconnected)
            {
                _connectButton.gameObject.SetActive(true);
                _disconnectButton.gameObject.SetActive(false);
            }
        }

        private void HandleReceiveMaxProCommandButtonClick()
        {
            //Debug.Log("SdkSample//HandleReceiveMaxProCommandButtonClick//");

            CommandType commandType = (CommandType)Convert.ToByte(_commandTypeField.text, 16);
            string hexCommandData = _commandDataField.text.Replace(" ", "");
            _maxProController.SimulateMaxProCommand(commandType, hexCommandData.Trim());
        }

        private void HandleSendAppCommandButtonClick()
        {
            //Debug.Log("SdkSample//HandleSendAppCommandButtonClick//");

            CommandType commandType = (CommandType)Convert.ToByte(_commandTypeField.text, 16);
            string hexCommandData = _commandDataField.text.Replace(" ", "");
            _maxProController.SendAppCommand(commandType, hexCommandData.Trim());
        }

        private void UpdateModeExclusiveCommandLabel(CommandType type, object obj)
        {
            // remove leading empty spaces
            //string json = JsonUtility.ToJson(obj, true).Replace("  ", "");
            string json = JsonUtility.ToJson(obj, true);

            //Debug.Log("SdkSample//UpdateModeExclusiveCommandLabel// json " + json);

            // remove "{" and "}" and their newlines
            _lastModeExclusiveCommandLabel.text = $"{type}{Environment.NewLine}{Environment.NewLine}{json.Substring(2, json.Length - 4)}";
        }

        private void UpdateNonModeExclusiveCommandLabel(CommandType type, byte[] data)
        {
            //Debug.Log("SdkSample//UpdateNonModeExclusiveCommandLabel// type " + type.ToString() + " data " + data.Length);

            _lastNonModeExclusiveCommandLabel.text = $"{type}{Environment.NewLine}{Environment.NewLine}<b>{ConversionUtility.ToHex(data, 0, data.Length).ToUpper()}</b>";
        }
    }
}

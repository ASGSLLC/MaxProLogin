//using _Project.FitFighter.RhythmRevamp.Scripts.RhythmGamemode;
//using MaxProFitness.Lucha.Inputs.Events;
//using MaxProFitness.Sdk;
//using MaxProFitness.Shared.Inputs;
//using MaxProFitness.Shared.Utilities;
using System;
using System.Collections;
using UnityEngine;

#if MAXPRO_LOGIN
using maxprofitness.login;
#endif

namespace maxprofitness.shared
{
    /// <summary>
    /// This class is used in the rowing canoe minigame to process MaxPro input
    /// </summary>
    public sealed class RowingCanoeInputController : MonoBehaviour
    {
        /*
        
        public delegate void UpdateSpeedHandler(float newSpeed, ActionSide side);

        public event UpdateSpeedHandler OnMovementUpdated;
        public event Action OnDeviceConnect;

        public DeviceConnectionChannel _deviceConnectionChannel;

        [SerializeField] private float _inputMultiplier = 1;

        private MaxProController _maxProController;

        public void Awake()
        {
            SetInputMultiplier(1);
            /*
            _maxProController = Singleton<MaxProController>.Get();

            if (_maxProController!.State != MaxProControllerState.Connected)
            {
                //_maxProController.Initialize();
            }

            _maxProController.AddMaxProCommandReceivedListener<GameEventRequestUpdateMaxProCommand>(HandleRequestUpdateMaxProCommandReceived);
            */
            //_maxProController.OnStateChanged += HandleStateChanged;

            //StartCoroutine(ActivateGameMode());
            /*
        }

        private void OnDestroy()
        {
            //_maxProController.OnStateChanged -= HandleStateChanged;
           // _maxProController.RemoveMaxProCommandReceivedListener<GameEventRequestUpdateMaxProCommand>(HandleRequestUpdateMaxProCommandReceived);
        }

        private void HandleStateChanged(MaxProController sender)
        {
            if (sender.State != MaxProControllerState.Connected)
            {
                return;
            }

            sender.SendAppCommand(new GameEventRequestAppCommand(true));
            OnDeviceConnect?.Invoke();

            _deviceConnectionChannel.RaiseEvent();
        }

        private void HandleRequestUpdateMaxProCommandReceived(MaxProController sender, GameEventRequestUpdateMaxProCommand inputReceived)
        {
            //new MaxProCommandReceivedEvent(inputReceived).TryInvokeShared(this);
        }

        public void HandleMovementUpdated(float newSpeed, ActionSide side)
        {
            if (_inputMultiplier == 0)
            {
                SetInputMultiplier(1);
            }

            OnMovementUpdated?.Invoke(newSpeed * _inputMultiplier, side);
        }

        private void SetInputMultiplier(float multiplier)
        {
            _inputMultiplier = multiplier;
        }

        private IEnumerator ActivateGameMode()
        {
            if (_maxProController.State == MaxProControllerState.Connected)
            {
                _maxProController.SendAppCommand(new GameEventRequestAppCommand(true));
                //PlayerInputReadyEvent.TryInvokeShared(this);
                OnDeviceConnect?.Invoke();
            }
            else
            {
                //must be done to act when the MaxPro connection is guaranteed
                yield return new WaitForSeconds(0.5f);

                StartCoroutine(ActivateGameMode());
            }
        }
    */}
            
}

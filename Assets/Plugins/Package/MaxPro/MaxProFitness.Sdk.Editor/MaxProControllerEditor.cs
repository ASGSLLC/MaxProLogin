using UnityEditor;
using UnityEngine;

namespace MaxProFitness.Sdk.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaxProController))]
    public class MaxProControllerEditor : UnityEditor.Editor
    {
        private SerializedProperty _script;
        private SerializedProperty _defaultConnectionHandler;
        private SerializedProperty _autoSendEventCommand;
        private SerializedProperty _scanTimeout;
        private SerializedProperty _gameModeTimeout;
        private SerializedProperty _powerOffTimeout;
        private SerializedProperty _deviceAddress;
        private SerializedProperty _serviceUuid;
        private SerializedProperty _subscribeCharacteristicUuid;
        private SerializedProperty _writeCharacteristicUuid;
        private SerializedProperty _deviceName;
        private SerializedProperty _state;
        private SerializedProperty _isGameModeOn;
        private SerializedProperty _debugAppCommand;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_script);
            }

            EditorGUILayout.PropertyField(_defaultConnectionHandler);
            EditorGUILayout.PropertyField(_autoSendEventCommand);
            EditorGUILayout.PropertyField(_scanTimeout);
            EditorGUILayout.PropertyField(_gameModeTimeout);
            EditorGUILayout.PropertyField(_powerOffTimeout);
            EditorGUILayout.Separator();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_deviceAddress);
                EditorGUILayout.PropertyField(_deviceName);
                EditorGUILayout.PropertyField(_serviceUuid);
                EditorGUILayout.PropertyField(_subscribeCharacteristicUuid);
                EditorGUILayout.PropertyField(_writeCharacteristicUuid);
                EditorGUILayout.PropertyField(_state);
                EditorGUILayout.PropertyField(_isGameModeOn);
            }

            MaxProController maxProController = (MaxProController)target;

            using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying || serializedObject.isEditingMultipleObjects || maxProController.State != MaxProControllerState.Connected))
            {
                EditorGUILayout.PropertyField(_debugAppCommand);

                IAppCommand appCommand = maxProController.DebugAppCommand;

                if (appCommand != null && GUILayout.Button("Send"))
                {
                    maxProController.SendAppCommand(appCommand.CommandType, appCommand.ToHexData());
                }
            }

            if (changeCheckScope.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnEnable()
        {
            _script = serializedObject.FindProperty("m_Script");
            _defaultConnectionHandler = serializedObject.FindProperty(nameof(_defaultConnectionHandler));
            _autoSendEventCommand = serializedObject.FindProperty(nameof(_autoSendEventCommand));
            _scanTimeout = serializedObject.FindProperty(nameof(_scanTimeout));
            _gameModeTimeout = serializedObject.FindProperty(nameof(_gameModeTimeout));
            _powerOffTimeout = serializedObject.FindProperty(nameof(_powerOffTimeout));
            _debugAppCommand = serializedObject.FindProperty(nameof(_debugAppCommand));
            _deviceAddress = serializedObject.FindProperty(nameof(_deviceAddress));
            _deviceName = serializedObject.FindProperty(nameof(_deviceName));
            _serviceUuid = serializedObject.FindProperty(nameof(_serviceUuid));
            _subscribeCharacteristicUuid = serializedObject.FindProperty(nameof(_subscribeCharacteristicUuid));
            _writeCharacteristicUuid = serializedObject.FindProperty(nameof(_writeCharacteristicUuid));
            _state = serializedObject.FindProperty(nameof(_state));
            _isGameModeOn = serializedObject.FindProperty(nameof(_isGameModeOn));
        }
    }
}

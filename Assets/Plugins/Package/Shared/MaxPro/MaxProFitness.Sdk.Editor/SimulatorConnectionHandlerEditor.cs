using UnityEditor;
using UnityEngine;
using MaxProFitness.Sdk;

namespace maxprofitness.shared
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SimulatorConnectionHandler))]
    public sealed class SimulatorConnectionHandlerEditor : UnityEditor.Editor
    {
        private const string DefaultCanvasPrefabGuid = "984b51020a6640542b152a97c546b61b";
        private static readonly GUIContent DefaultLabel = new GUIContent("Default");
        private SerializedProperty _script;
        private SerializedProperty _canvas;
        private SerializedProperty _canvasPrefab;
        private SerializedProperty _bothHandsKey;
        private SerializedProperty _isGameModeOn;
        private SerializedProperty _batteryPercent;
        private SerializedProperty _initializeDelay;
        private SerializedProperty _canInitialize;
        private SerializedProperty _scanDelay;
        private SerializedProperty _scanDeviceAddress;
        private SerializedProperty _scanDeviceName;
        private SerializedProperty _connectDelay;
        private SerializedProperty _canConnect;
        private SerializedProperty _listenDelay;
        private SerializedProperty _sendDelay;
        private SerializedProperty _lastEventCommand;
        private SerializedProperty _nextEventCommand;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope();

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(_script);
                EditorGUILayout.PropertyField(_canvas);
            }

            using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
            {
                if (_canvasPrefab.objectReferenceValue == null)
                {
                    Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                    float buttonWidth = EditorStyles.miniPullDown.CalcSize(DefaultLabel).x;
                    Rect fieldPosition = position;
                    fieldPosition.xMax -= buttonWidth + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(fieldPosition, _canvasPrefab);

                    Rect buttonPosition = position;
                    buttonPosition.xMin = fieldPosition.xMax + EditorGUIUtility.standardVerticalSpacing;

                    if (GUI.Button(buttonPosition, DefaultLabel, EditorStyles.miniButton))
                    {
                        string canvasPrefabPath = AssetDatabase.GUIDToAssetPath(DefaultCanvasPrefabGuid);
                        _canvasPrefab.objectReferenceValue = AssetDatabase.LoadAssetAtPath<SimulatorCanvas>(canvasPrefabPath);
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(_canvasPrefab);
                }

                EditorGUILayout.PropertyField(_bothHandsKey);
                EditorGUILayout.PropertyField(_isGameModeOn);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_batteryPercent);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_initializeDelay);
            EditorGUILayout.PropertyField(_canInitialize);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_scanDelay);
            EditorGUILayout.PropertyField(_scanDeviceAddress);
            EditorGUILayout.PropertyField(_scanDeviceName);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_connectDelay);
            EditorGUILayout.PropertyField(_canConnect);
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(_listenDelay);
            EditorGUILayout.PropertyField(_sendDelay);
            EditorGUILayout.Separator();

            _lastEventCommand.isExpanded = true;
            _nextEventCommand.isExpanded = true;

            using (new EditorGUI.DisabledScope(true))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.TextField("Event Command", EditorStyles.label);

                        SerializedProperty copy = _lastEventCommand.Copy();
                        string propertyPath = copy.propertyPath;
                        copy.Next(true);

                        do
                        {
                            EditorGUILayout.TextField(copy.displayName, EditorStyles.label);
                        }
                        while (copy.Next(false) && copy.propertyPath.StartsWith(propertyPath));
                    }

                    using (new EditorGUILayout.VerticalScope())
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            using (new EditorGUILayout.VerticalScope())
                            {
                                EditorGUILayout.TextField("Last", EditorStyles.label);

                                SerializedProperty copy = _lastEventCommand.Copy();
                                string propertyPath = copy.propertyPath;
                                copy.Next(true);

                                do
                                {
                                    EditorGUILayout.PropertyField(copy, GUIContent.none);
                                }
                                while (copy.Next(false) && copy.propertyPath.StartsWith(propertyPath));
                            }

                            using (new EditorGUILayout.VerticalScope())
                            {
                                EditorGUILayout.TextField("Next", EditorStyles.label);

                                SerializedProperty copy = _nextEventCommand.Copy();
                                string propertyPath = copy.propertyPath;
                                copy.Next(true);

                                do
                                {
                                    EditorGUILayout.PropertyField(copy, GUIContent.none);
                                }
                                while (copy.Next(false) && copy.propertyPath.StartsWith(propertyPath));
                            }
                        }
                    }
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
            _canvas = serializedObject.FindProperty(nameof(_canvas));
            _canvasPrefab = serializedObject.FindProperty(nameof(_canvasPrefab));
            _bothHandsKey = serializedObject.FindProperty(nameof(_bothHandsKey));
            _isGameModeOn = serializedObject.FindProperty(nameof(_isGameModeOn));
            _batteryPercent = serializedObject.FindProperty(nameof(_batteryPercent));
            _initializeDelay = serializedObject.FindProperty(nameof(_initializeDelay));
            _canInitialize = serializedObject.FindProperty(nameof(_canInitialize));
            _scanDelay = serializedObject.FindProperty(nameof(_scanDelay));
            _scanDeviceAddress = serializedObject.FindProperty(nameof(_scanDeviceAddress));
            _scanDeviceName = serializedObject.FindProperty(nameof(_scanDeviceName));
            _connectDelay = serializedObject.FindProperty(nameof(_connectDelay));
            _canConnect = serializedObject.FindProperty(nameof(_canConnect));
            _listenDelay = serializedObject.FindProperty(nameof(_listenDelay));
            _sendDelay = serializedObject.FindProperty(nameof(_sendDelay));
            _lastEventCommand = serializedObject.FindProperty(nameof(_lastEventCommand));
            _nextEventCommand = serializedObject.FindProperty(nameof(_nextEventCommand));
        }
    }
}

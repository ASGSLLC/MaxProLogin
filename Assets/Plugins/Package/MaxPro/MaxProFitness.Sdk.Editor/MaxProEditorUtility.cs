using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEngine;
using maxprofitness.login;

namespace MaxProFitness.Sdk.Editor
{
    public static class MaxProEditorUtility
    {
        public delegate void ModifyPrefabHandler(GameObject temporaryInstance);

        public static void ModifyPrefab([NotNull] GameObject prefab, [NotNull] ModifyPrefabHandler modifyPrefabHandler)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            if (modifyPrefabHandler == null)
            {
                throw new ArgumentNullException(nameof(modifyPrefabHandler));
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            modifyPrefabHandler.Invoke(instance);
            PrefabUtility.SaveAsPrefabAsset(instance, AssetDatabase.GetAssetPath(prefab));
            UnityEngine.Object.DestroyImmediate(instance);
        }

        public static void SetConnectionHandler([NotNull] GameObject maxProControllerPrefab, IMaxProConnectionHandler connectionHandler)
        {
            void modifyPrefabHandler(GameObject temporaryInstance)
            {
                MaxProController controller = temporaryInstance.GetComponent<MaxProController>();
                Debug.Assert(controller != null, "Missing MaxProController script in MaxProController prefab!");

                controller.DefaultConnectionHandler = connectionHandler;
            }

            ModifyPrefab(maxProControllerPrefab, modifyPrefabHandler);
        }

        public static void SetSimulatorConnectionHandler([NotNull] GameObject maxProControllerPrefab)
        {
            void modifyPrefabHandler(GameObject temporaryInstance)
            {
                MaxProController controller = temporaryInstance.GetComponent<MaxProController>();
                SimulatorConnectionHandler simulator = temporaryInstance.GetComponent<SimulatorConnectionHandler>();
                Debug.Assert(controller != null, "Missing MaxProController script in MaxProController prefab!");
                Debug.Assert(simulator != null, "Missing SimulatorConnectionHandler script in MaxProController prefab!");

                controller.DefaultConnectionHandler = simulator;
            }

            ModifyPrefab(maxProControllerPrefab, modifyPrefabHandler);
        }
    }
}

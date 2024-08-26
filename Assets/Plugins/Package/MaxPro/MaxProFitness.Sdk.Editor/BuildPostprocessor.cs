using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
// using UnityEditor.iOS.Xcode;

namespace MaxProFitness.Sdk.Editor
{
    /*public static class BuildPostprocessor
    {
        [PostProcessBuild]
        private static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS)
            {
                return;
            }

            string plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;
            rootDict.SetString("NSBluetoothAlwaysUsageDescription", "Used for connecting to MAX PRO");
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }*/
}

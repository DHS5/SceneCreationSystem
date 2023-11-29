using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Dhs5.SceneCreation
{
    public class Utility : MonoBehaviour
    {
        public static readonly string SceneCreationSettingsPath 
            = "/Settings/Editor/SceneCreationSettings.asset";
        public static readonly string SceneCreationSettingsAutoSavePath 
            = "/Settings/Editor/SceneCreationSettingsAutoSave.txt";
        public static readonly string SceneCreationSettingsManualSavePath 
            = "/Settings/Editor/SceneCreationSettingsManualSave.txt";
        public static void AutoSaveSceneCreationSettings()
        {
            string settingsContent = File.ReadAllText(Application.dataPath + SceneCreationSettingsPath);
            File.WriteAllText(Application.dataPath + SceneCreationSettingsAutoSavePath, settingsContent);
        }
        public static void ManualSaveSceneCreationSettings()
        {
            string settingsContent = File.ReadAllText(Application.dataPath + SceneCreationSettingsPath);
            File.WriteAllText(Application.dataPath + SceneCreationSettingsManualSavePath, settingsContent);
        }
        public static void LoadFromManualSaveSceneCreationSettings()
        {
            string settingsContent = File.ReadAllText(Application.dataPath + SceneCreationSettingsManualSavePath);
            File.WriteAllText(Application.dataPath + SceneCreationSettingsPath, settingsContent);
        }
    }
}

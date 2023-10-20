using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dhs5.Utility.Settings;

#if UNITY_EDITOR
using Dhs5.Utility.Settings.Editor;
#endif

namespace Dhs5.SceneCreation
{
    [Settings(SettingsUsage.EditorProject, "Scene Creation Settings")]
    public class SceneCreationSettings : Settings<SceneCreationSettings>
    {
        [SettingsProvider]
        static SettingsProvider GetSettingsProvider() =>
        instance.GetSettingsProvider();

        [SerializeField] private IntersceneVariablesSO intersceneVariablesSO;
        public IntersceneVariablesSO IntersceneVars => intersceneVariablesSO;
    }
}

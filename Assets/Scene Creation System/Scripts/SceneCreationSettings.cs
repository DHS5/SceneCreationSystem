using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dhs5.Utility.Settings;
using System;
using Dhs5.Utility.DirectoryPicker;

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

        [SerializeField] private SceneObjectSettings sceneObjectSettings;
        public SceneObjectSettings SceneObjectSettings => sceneObjectSettings;

        [Space(20f)]

        [SerializeField] private DirectoryPicker sceneVariablesDirectory;
        public string SceneVariablesContainerPath => sceneVariablesDirectory.Path;

        [Space(20f)]

        [SerializeField] private SceneCreationBasePrefabs sceneCreationPrefabs;
        public SceneCreationBasePrefabs Prefabs => sceneCreationPrefabs;
    }

    [Serializable]
    public struct SceneCreationBasePrefabs
    {
        [Header("Base")]
        public GameObject sceneManagerPrefab;
        public GameObject sceneClockPrefab;
        public GameObject sceneObjectPrefab;
        public GameObject sceneSpawnerPrefab;

        [Header("Helpers")]
        public GameObject colliderSceneObjectPrefab;
    }
}

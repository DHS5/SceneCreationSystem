using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Dhs5.SceneCreation
{
    public static class SceneObjectCreator
    {
        private static void CreateSceneObject(string path)
        {
            GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            GameObject obj = PrefabUtility.InstantiatePrefab(go, Selection.activeTransform) as GameObject;
            PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            Selection.activeGameObject = obj;
        }

        [MenuItem("GameObject/SceneObjects/SceneObject", priority = 3)]
        public static void CreateSimpleSceneObject()
        {
            CreateSceneObject("Assets/Scene Creation System/Prefabs/SceneObject.prefab");
        }
        
        [MenuItem("GameObject/SceneObjects/SceneManager", priority = 3)]
        public static void CreateSceneManager()
        {
            CreateSceneObject("Assets/Scene Creation System/Prefabs/SceneManager.prefab");
        }
        
        [MenuItem("GameObject/SceneObjects/SceneClock", priority = 3)]
        public static void CreateSceneClock()
        {
            CreateSceneObject("Assets/Scene Creation System/Prefabs/SceneClock.prefab");
        }
    }
}

#endif

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

using MenuCommand = UnityEditor.MenuCommand;

namespace Dhs5.SceneCreation
{
    public class SceneObjectCreator
    {
        public const string legacyMenuPath = "GameObject/SceneObjects/Legacy/";
        public const string legacyPrefabPath = "Assets/Scene Creation System/Prefabs/";

        protected static void CreateSceneObject(string path, MenuCommand menuCommand)
        {
            GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            GameObject obj = PrefabUtility.InstantiatePrefab(go, Selection.activeTransform) as GameObject;
            GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);
            PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
            Selection.activeGameObject = obj;
        }

        [MenuItem(legacyMenuPath + "SceneObject", priority = 10, secondaryPriority = 3)]
        public static void CreateSimpleSceneObject(MenuCommand menuCommand)
        {
            CreateSceneObject(legacyPrefabPath + "SceneObject.prefab", menuCommand);
        }
        
        [MenuItem(legacyMenuPath + "SceneManager", priority = 10, secondaryPriority = 1)]
        public static void CreateSceneManager(MenuCommand menuCommand)
        {
            CreateSceneObject(legacyPrefabPath + "SceneManager.prefab", menuCommand);
        }
        
        [MenuItem(legacyMenuPath + "SceneClock", priority = 10, secondaryPriority = 2)]
        public static void CreateSceneClock(MenuCommand menuCommand)
        {
            CreateSceneObject(legacyPrefabPath + "SceneClock.prefab", menuCommand);
        }
        
        [MenuItem(legacyMenuPath + "Helpers/Collider SceneObject", priority = 10, secondaryPriority = 100)]
        public static void CreateColliderSceneObject(MenuCommand menuCommand)
        {
            CreateSceneObject(legacyPrefabPath + "Collider SceneObject.prefab", menuCommand);
        }
    }
}

#endif

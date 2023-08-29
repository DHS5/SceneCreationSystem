using UnityEngine;
using UnityEditor;

using MenuCommand = UnityEditor.MenuCommand;

namespace Dhs5.SceneCreation
{
    public class SceneObjectCreator
    {
        public const string legacyMenuPath = "GameObject/SceneObjects/Legacy/";
        public const string legacyPrefabPath = "Assets/Scene Creation System/Prefabs/";

        protected static SceneObject CreateSceneObject(string path, MenuCommand menuCommand)
        {
            GameObject go = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            GameObject obj = PrefabUtility.InstantiatePrefab(go, Selection.activeTransform) as GameObject;
            SceneObject sceneObject = obj.GetComponent<SceneObject>();
            if (sceneObject != null && sceneObject is not SceneManager) sceneObject.GetSceneVariablesSOInScene();
            GameObjectUtility.SetParentAndAlign(obj, menuCommand?.context as GameObject);
            PrefabUtility.UnpackPrefabInstance(obj, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
            Selection.activeGameObject = obj;
            return sceneObject;
        }

        [MenuItem(legacyMenuPath + "SceneObject", priority = 10, secondaryPriority = 3)]
        public static SceneObject CreateSimpleSceneObject(MenuCommand menuCommand)
        {
            return CreateSceneObject(legacyPrefabPath + "SceneObject.prefab", menuCommand);
        }
        
        [MenuItem(legacyMenuPath + "SceneManager", priority = 10, secondaryPriority = 1)]
        public static SceneManager CreateSceneManager(MenuCommand menuCommand)
        {
            return CreateSceneObject(legacyPrefabPath + "SceneManager.prefab", menuCommand) as SceneManager;
        }
        
        [MenuItem(legacyMenuPath + "SceneClock", priority = 10, secondaryPriority = 2)]
        public static SceneClock CreateSceneClock(MenuCommand menuCommand)
        {
            return CreateSceneObject(legacyPrefabPath + "SceneClock.prefab", menuCommand) as SceneClock;
        }
        
        [MenuItem(legacyMenuPath + "Helpers/Collider SceneObject", priority = 10, secondaryPriority = 100)]
        public static Collider_SObj CreateColliderSceneObject(MenuCommand menuCommand)
        {
            return CreateSceneObject(legacyPrefabPath + "Collider SceneObject.prefab", menuCommand) as Collider_SObj;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Dhs5.SceneCreation
{
    public static class EditorHelper
    {
        [MenuItem("SCS/Get SceneVariablesSO of current scene")]
        public static void GetActiveSceneVariablesSO()
        {
            SceneManager manager = GameObject.FindObjectOfType<SceneManager>();
            if (manager != null)
            {
                Selection.activeObject = manager.SceneVariablesSO;
            }
            else
            {
                Debug.LogError("Can't find the SceneManager of the current scene");
            }
        }

        [MenuItem("SCS/Setup all SceneObjects in current scene")]
        public static void SetAllSceneObjectSceneVariablesSO()
        {
            SceneObject[] sceneObjects = GameObject.FindObjectsOfType<SceneObject>();

            if (sceneObjects.Length > 0)
            {
                foreach (var sceneObject in sceneObjects)
                {
                    sceneObject.GetSceneVariablesSOInScene();
                }
            }
            else
            {
                Debug.LogError("Can't find any SceneObject in current scene");
            }
        }

        [MenuItem("SCS/Setup New Scene")]
        public static void SetUpNewScene(MenuCommand menuCommand)
        {
            Scene activeScene = EditorSceneManager.GetActiveScene();
            string sceneName = activeScene.name;
            string newSceneVarsPath = activeScene.path.Substring(0, activeScene.path.LastIndexOf('/') + 1) + sceneName + "_SceneVars.asset";
            SceneVariablesSO newSceneVars;
            if (!File.Exists(newSceneVarsPath))
            {
                newSceneVars = ScriptableObject.CreateInstance<SceneVariablesSO>();
                AssetDatabase.CreateAsset(newSceneVars, activeScene.path.Substring(0, activeScene.path.LastIndexOf('/') + 1) + sceneName + "_SceneVars.asset");
            }
            else
            {
                newSceneVars = AssetDatabase.LoadAssetAtPath<SceneVariablesSO>(newSceneVarsPath);
            }

            SceneManager manager = GameObject.FindObjectOfType<SceneManager>();
            if (manager == null)
            {
                manager = SceneObjectCreator.CreateSceneManager(menuCommand);
            }
            manager.SetSceneVariablesSO(newSceneVars);

            SceneClock clock = GameObject.FindObjectOfType<SceneClock>();
            if (clock == null)
            {
                clock = SceneObjectCreator.CreateSceneClock(menuCommand);
            }
            else
            {
                clock.GetSceneVariablesSOInScene();
            }

            Selection.activeObject = newSceneVars;
        }

        #region Log
        [MenuItem("SCS/Log/Display simple Scene Log", priority = 100)]
        public static void DisplaySimpleSceneLog()
        {
            SceneManager manager = GameObject.FindObjectOfType<SceneManager>();
            if (manager != null)
            {
                Debug.Log(SceneLogger.GetSceneLog(manager.gameObject));
            }
            else
            {
                Debug.LogError("Can't find the SceneManager of the current scene");
            }
        }
        
        [MenuItem("SCS/Log/Display detailed Scene Log", priority = 101)]
        public static void DisplayDetailedSceneLog()
        {
            SceneManager manager = GameObject.FindObjectOfType<SceneManager>();
            if (manager != null)
            {
                Debug.Log(SceneLogger.GetSceneLog(manager.gameObject, true));
            }
            else
            {
                Debug.LogError("Can't find the SceneManager of the current scene");
            }
        }
        
        [MenuItem("SCS/Log/Print detailed Scene Log in file", priority = 102)]
        public static void PrintDetailedSceneLogInFile()
        {
            SceneManager manager = GameObject.FindObjectOfType<SceneManager>();
            if (manager != null)
            {
                string directoryPath = Application.persistentDataPath + "/SceneLog/";
                string content = SceneLogger.GetSceneLog(manager.gameObject, true, true);
                DateTime now = DateTime.Now;
                string date = now.Day + "." + now.Month + "." + now.Year + " " + now.Hour + "h" + string.Format("{00:00}", now.Minute);
                string path = directoryPath + manager.gameObject.scene.name + " " + date + ".txt";
                Debug.Log("<color=#ff0000> Log path : " + "</color>" + path);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                File.WriteAllText(path, content);
                EditorUtility.RevealInFinder(path);
            }
            else
            {
                Debug.LogError("Can't find the SceneManager of the current scene");
            }
        }
        #endregion
    }
}

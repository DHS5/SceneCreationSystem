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
        public static SceneManager GetCurrentSceneManager()
        {
            return GameObject.FindObjectOfType<SceneManager>();
        }
        public static SceneVariablesSO GetCurrentSceneVariablesSO()
        {
            SceneManager manager = GetCurrentSceneManager();
            if (manager != null)
            {
                return manager.SceneVariablesSO;
            }
            return null;
        }

        [MenuItem("SCS/Refresh all SceneObjects in current scene")]
        public static void RefreshSceneObjects()
        {
            BaseSceneObject[] sceneObjects = GameObject.FindObjectsOfType<SceneObject>();

            if (sceneObjects.Length > 0)
            {
                foreach (var sceneObject in sceneObjects)
                {
                    sceneObject.Refresh();
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
                SceneObjectCreator.CreateSceneClock(menuCommand);
            }

            RefreshSceneObjects();

            Selection.activeObject = newSceneVars;
        }

        [MenuItem("SCS/Get/Interscene Variables Container", priority = 300)]
        public static void GetIntersceneVariablesContainer()
        {
            Selection.activeObject = SceneCreationSettings.instance.IntersceneVars;
        }
        [MenuItem("SCS/Get/SceneObject Settings", priority = 301)]
        public static void GetSceneObjectSettings()
        {
            Selection.activeObject = SceneCreationSettings.instance.SceneObjectSettings;
        }
        [MenuItem("SCS/Get/This Scene Variables Container", priority = 302)]
        public static void GetActiveSceneVariablesSO()
        {
            SceneVariablesSO sceneVariablesSO = GetCurrentSceneVariablesSO();
            if (sceneVariablesSO != null)
            {
                Selection.activeObject = sceneVariablesSO;
            }
            else
            {
                Debug.LogError("Can't find the SceneManager of the current scene");
            }
        }

        [MenuItem("SCS/Get/SceneObject Tag Database", priority = 305)]
        public static void GetSceneObjectTagDatabase()
        {
            Selection.activeObject = SceneObjectTagDatabase.Instance;
        }
        

        #region Log
        private static void DisplaySceneLog(bool detailed, bool showEmpty, bool color)
        {
            SceneManager manager = GameObject.FindObjectOfType<SceneManager>();
            if (manager != null)
            {
                Debug.Log(SceneLogger.GetSceneLog(manager.gameObject, detailed, showEmpty, !color));
            }
            else
            {
                Debug.LogError("Can't find the SceneManager of the current scene");
            }
        }

        #region Console Color
        [MenuItem("SCS/Log/Console/Color/SceneLog : Simple", priority = 100)]
        public static void DisplayColorSimpleSceneLog()
        {
            DisplaySceneLog(false, false, true);
        }
        [MenuItem("SCS/Log/Console/Color/SceneLog : Simple with empty", priority = 101)]
        public static void DisplayColorSimpleWEmptySceneLog()
        {
            DisplaySceneLog(false, true, true);
        }
        
        [MenuItem("SCS/Log/Console/Color/SceneLog : Detailed no empty", priority = 102)]
        public static void DisplayColorDetailedSceneLog()
        {
            DisplaySceneLog(true, false, true);
        }
        [MenuItem("SCS/Log/Console/Color/SceneLog : Detailed with empty", priority = 103)]
        public static void DisplayColorDetailedWEmptySceneLog()
        {
            DisplaySceneLog(true, true, true);
        }
        #endregion

        #region Console No color
        [MenuItem("SCS/Log/Console/No Color/SceneLog : Simple", priority = 100)]
        public static void DisplayNoColorSimpleSceneLog()
        {
            DisplaySceneLog(false, false, false);
        }
        [MenuItem("SCS/Log/Console/No Color/SceneLog : Simple with empty", priority = 101)]
        public static void DisplayNoColorSimpleWEmptySceneLog()
        {
            DisplaySceneLog(false, true, false);
        }
        
        [MenuItem("SCS/Log/Console/No Color/SceneLog : Detailed no empty", priority = 102)]
        public static void DisplayNoColorDetailedSceneLog()
        {
            DisplaySceneLog(true, false, false);
        }
        [MenuItem("SCS/Log/Console/No Color/SceneLog : Detailed with empty", priority = 103)]
        public static void DisplayNoColorDetailedWEmptySceneLog()
        {
            DisplaySceneLog(true, true, false);
        }
        #endregion

        #region In File
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
        #endregion

        #region References

        #endregion
    }
}

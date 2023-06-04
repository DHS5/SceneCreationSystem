using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Dhs5.SceneCreation
{
    [CustomEditor(typeof(SceneVariablesSO))]
    public class SceneVariablesSOEditor : Editor
    {
        SceneVariablesSO sceneVariablesSO;

        private void OnEnable()
        {
            sceneVariablesSO = target as SceneVariablesSO;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 1.5f);

            if (GUILayout.Button("Create Balancing Sheet"))
            {
                string path = AssetDatabase.GetAssetPath(sceneVariablesSO);
                path = path.Substring(0, path.LastIndexOf('/') + 1) + target.name + " BalancingSheets/";
                
                SceneBalancingSheetSO balancingSheet = ScriptableObject.CreateInstance<SceneBalancingSheetSO>();
                balancingSheet.sceneVariablesSO = sceneVariablesSO;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                AssetDatabase.CreateAsset(balancingSheet, path + GetUniqueName(path) + ".asset");
            }
        }

        #region Helper functions
        readonly string baseName = "BalancingSheet";

        private string GetUniqueName(string path)
        {
            List<string> names = new();
            string current;

            foreach (var name in Directory.GetFiles(path))
            {
                current = name.Remove(0, name.LastIndexOf('/') + 1);
                current = current.Substring(0, current.LastIndexOf('.'));
                names.Add(current);
            }

            if (names.Count < 1) 
            {
                return baseName + "1";
            }

            int index = 1;

            while (names.Contains(baseName + index))
            {
                index++;
            }
            return baseName + index;
        }

        #endregion
    }
}

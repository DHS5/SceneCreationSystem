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

            sceneVariablesSO.OnEditorEnable();
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneVars"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("complexSceneVars"));

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 1.5f);

            if (GUILayout.Button("Create Balancing Sheet"))
            {
                sceneVariablesSO.CreateNewBalancingSheet();
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneBalancingSheets"));
            EditorGUI.EndDisabledGroup();
        }
    }
}

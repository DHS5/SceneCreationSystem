using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using NUnit.Framework;
using UnityEditorInternal;

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

            CreateBalancingSheetList("sceneBalancingSheets", "Balancing Sheets");
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
                CreateBalancingSheetList("sceneBalancingSheets", "Balancing Sheets");
            }

            list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
            UnityEditor.EditorUtility.SetDirty(target);
        }

        ReorderableList list;
        private void CreateBalancingSheetList(string listPropertyName, string displayName)
        {
            serializedObject.Update();
            SerializedProperty textList = serializedObject.FindProperty(listPropertyName);

            list = new ReorderableList(serializedObject, textList, true, true, false, false)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, displayName);
                },

                drawElementCallback = (rect, index, active, focused) =>
                {
                    var element = textList.GetArrayElementAtIndex(index);

                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUI.PropertyField(rect, element, new GUIContent("Balancing Sheet " + index), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                },

                elementHeight = EditorGUIUtility.singleLineHeight
            };
        }
    }
}

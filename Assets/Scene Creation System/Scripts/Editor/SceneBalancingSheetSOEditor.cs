using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using NUnit.Framework;
using UnityEditorInternal;

namespace Dhs5.SceneCreation
{
    [CustomEditor(typeof(SceneBalancingSheetSO))]
    public class SceneBalancingSheetSOEditor : Editor
    {
        SceneBalancingSheetSO sceneBalancingSheetSO;

        private void OnEnable()
        {
            sceneBalancingSheetSO = target as SceneBalancingSheetSO;

            sceneBalancingSheetSO.ApplyTemplate();

            CreateReorderableList("balancingVars", sceneBalancingSheetSO.balancingVars, "Scene Variables overrides");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            EditorGUILayout.BeginVertical();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneVariablesSO"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 1.5f);

            list.DoLayoutList();
            
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            UnityEditor.EditorUtility.SetDirty(target);
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

        ReorderableList list;
        private void CreateReorderableList(string listPropertyName, List<BalancingVar> balancingVars, string displayName)
        {
            SerializedProperty textList = serializedObject.FindProperty(listPropertyName);
            list = new ReorderableList(serializedObject, textList, false, true, false, false)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, displayName);
                },

                drawElementCallback = (rect, index, active, focused) =>
                {
                    var element = textList.GetArrayElementAtIndex(index);

                    EditorGUI.indentLevel++;
                    EditorGUI.PropertyField(rect, element, new GUIContent(balancingVars[index].ID), true);
                    EditorGUI.indentLevel--;
                },

                elementHeightCallback = index =>
                {
                    var element = textList.GetArrayElementAtIndex(index);

                    return EditorGUI.GetPropertyHeight(element);
                }
            };
        }
        #endregion
    }
}

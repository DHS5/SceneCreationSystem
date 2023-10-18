using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using NUnit.Framework;
using UnityEditorInternal;
using System;

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
            CreateSceneVarList("sceneVars", "Scene Variables");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.LabelField("Inter-scene variables");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("intersceneVariablesSO"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(15f);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneVars"));
            EditorGUILayout.Space(5f);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("complexSceneVars"));

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight * 1.5f);

            //if (GUILayout.Button("Create Balancing Sheet"))
            //{
            //    sceneVariablesSO.CreateNewBalancingSheet();
            //    CreateBalancingSheetList("sceneBalancingSheets", "Balancing Sheets");
            //}

            balancingSheetList.DoLayoutList();
            EditorGUILayout.Space(10f);
            sceneVarList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
            UnityEditor.EditorUtility.SetDirty(target);
        }

        ReorderableList sceneVarList;
        private void CreateSceneVarList(string listPropertyName, string displayName)
        {
            serializedObject.Update();
            SerializedProperty textList = serializedObject.FindProperty(listPropertyName);

            sceneVarList = new ReorderableList(serializedObject, textList, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, displayName);
                },

                drawElementCallback = (rect, index, active, focused) =>
                {
                    var element = textList.GetArrayElementAtIndex(index);

                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(sceneVariablesSO.IsLinkAtIndex(index));
                    EditorGUI.PropertyField(rect, element, true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                },

                onAddDropdownCallback = (rect, list) =>
                {
                    var menu = new GenericMenu();
                    SceneVarType type;
                    foreach (var value in Enum.GetValues(typeof(SceneVarType)))
                    {
                        type = (SceneVarType)value;
                        menu.AddItem(new GUIContent(type.ToString()), false, AddOfType, type);
                    }
                    menu.ShowAsContext();
                },

                onRemoveCallback = list =>
                {
                    sceneVariablesSO.TryRemoveSceneVarAtIndex(list.index);
                },

                onCanRemoveCallback = list =>
                {
                    return sceneVariablesSO.CanRemoveAtIndex(list.index);
                },
                
                elementHeightCallback = index => EditorGUI.GetPropertyHeight(textList.GetArrayElementAtIndex(index)),
            };

            void AddOfType(object type)
            {
                if (type is SceneVarType t)
                {
                    sceneVariablesSO.AddSceneVarOfType(t);
                }
                else
                {
                    Debug.LogError("Error in adding of type " + type);
                }
            }
        }
        
        ReorderableList balancingSheetList;
        private void CreateBalancingSheetList(string listPropertyName, string displayName)
        {
            serializedObject.Update();
            SerializedProperty textList = serializedObject.FindProperty(listPropertyName);

            balancingSheetList = new ReorderableList(serializedObject, textList, true, true, true, false)
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
                    EditorGUI.PropertyField(rect, element, new GUIContent("Sheet " + (index + 1)), true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                },

                onAddCallback = rect =>
                {
                    sceneVariablesSO.CreateNewBalancingSheet();
                },

                elementHeight = EditorGUIUtility.singleLineHeight
            };
        }
    }
}

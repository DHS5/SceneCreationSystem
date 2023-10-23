using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CustomEditor(typeof(BaseSceneObject), true)]
    public class BaseSceneObjectEditor : Editor
    {
        GUIContent empty = new GUIContent("");

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), empty);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10f);

            EditorGUI.BeginDisabledGroup(target is not SceneManager);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneVariablesSO"), empty);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(5f);

            DrawPropertiesExcluding(serializedObject, "m_Script");
        }
    }
}

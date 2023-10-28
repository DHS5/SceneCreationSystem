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

        Color backgroundColor;
        Color foregroundColor;

        public override void OnInspectorGUI()
        {
            backgroundColor = new Color32(191, 247, 255, 255);
            foregroundColor = new Color(1f, 0.49f, 0f);

            GUIStyle headerStyle = new()
            {
                fontStyle = FontStyle.Bold,
            };
            headerStyle.normal.textColor = Color.white;

            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), empty);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(2f);

            Rect lineRect = EditorGUILayout.GetControlRect(false, 2f);
            lineRect.x -= 18f;
            lineRect.width += 25f;
            EditorGUI.DrawRect(lineRect, foregroundColor);
            lineRect.y += 54f;
            EditorGUI.DrawRect(lineRect, foregroundColor);

            Rect backgroundRect = EditorGUILayout.GetControlRect(false, 1f);
            backgroundRect.height = 50f;
            backgroundRect.width += 25f;
            backgroundRect.x -= 18f;
            backgroundRect.y -= 1f;
            EditorGUI.DrawRect(backgroundRect, backgroundColor);

            GUI.contentColor = foregroundColor;

            EditorGUI.BeginDisabledGroup(target is not SceneManager);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneVariablesSO"), empty);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(1f);

            Rect currentRect = EditorGUILayout.GetControlRect(false, 20f);

            // Tag
            EditorGUI.LabelField(new Rect
                (currentRect.x, currentRect.y, currentRect.width * 0.06f, currentRect.height)
                , new GUIContent("Tag", "Scene Object Tag of this object"), headerStyle);
            EditorGUI.LabelField(new Rect
                (currentRect.x + currentRect.width * 0.51f, currentRect.y, currentRect.width * 0.08f, currentRect.height)
                , new GUIContent("Layer", "Scene Object Layer of this object"), headerStyle);
            GUI.contentColor = Color.white;
            EditorGUI.PropertyField(new Rect
                (currentRect.x + currentRect.width * 0.06f, currentRect.y, currentRect.width * 0.43f, currentRect.height)
                , serializedObject.FindProperty("sceneObjectTag"), empty);

            

            //EditorGUILayout.Space(5f);

            EditorGUILayout.Space(15f);

            DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();
        }
    }
}

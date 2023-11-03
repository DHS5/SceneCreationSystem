using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CustomEditor(typeof(BaseSceneObject), true)]
    [CanEditMultipleObjects]
    public class BaseSceneObjectEditor : Editor
    {
        protected BaseSceneObject baseSceneObject;

        protected GUIContent empty = new GUIContent("");

        Color backgroundColor;
        Color foregroundColor;

        bool isManager = false;

        protected virtual bool DrawChildInspector => true;

        protected virtual void OnEnable()
        {
            baseSceneObject = target as BaseSceneObject;
            if (baseSceneObject is SceneManager) isManager = true;

            baseSceneObject.OnEditorEnable();
        }

        public override void OnInspectorGUI()
        {
            backgroundColor = new Color32(191, 247, 255, 255);
            foregroundColor = new Color(1f, 0.49f, 0f);

            GUIStyle headerStyle = new()
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            headerStyle.normal.textColor = Color.white;

            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), empty);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(2f);

            if (!isManager && baseSceneObject.SceneVariablesSO == null)
            {
                EditorGUILayout.HelpBox("You need to setup the SceneManager and its SceneVariablesSO", MessageType.Error);
            }
            else
            {
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

                Rect currentRect = EditorGUILayout.GetControlRect(false);

                if (GUI.Button(new Rect
                    (currentRect.x + currentRect.width * 0.89f, currentRect.y, currentRect.width * 0.05f, currentRect.height),
                    EditorGUIUtility.IconContent("d_ToolSettings")))
                {
                    EditorHelper.GetSceneObjectSettings();
                }
                if (GUI.Button(new Rect
                    (currentRect.x + currentRect.width * 0.95f, currentRect.y, currentRect.width * 0.05f, currentRect.height),
                    EditorGUIUtility.IconContent("d__Popup")))
                {
                    EditorHelper.GetSceneCreationSettings();
                }

                if (!isManager) GUI.contentColor = foregroundColor;

                EditorGUI.BeginDisabledGroup(!isManager);
                EditorGUI.PropertyField(new Rect
                    (currentRect.x, currentRect.y, currentRect.width * 0.88f, currentRect.height),
                    serializedObject.FindProperty("sceneVariablesSO"), empty);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space(1f);

                if (isManager) GUI.contentColor = foregroundColor;

                currentRect = EditorGUILayout.GetControlRect(false, 20f);

                // Tag Text
                EditorGUI.LabelField(new Rect
                    (currentRect.x, currentRect.y, currentRect.width * 0.06f, currentRect.height)
                    , new GUIContent("Tag", "Scene Object Tag of this object"), headerStyle);
                // Layer Text
                EditorGUI.LabelField(new Rect
                    (currentRect.x + currentRect.width * 0.51f, currentRect.y, currentRect.width * 0.08f, currentRect.height)
                    , new GUIContent("Layer", "Scene Object Layer of this object"), headerStyle);
                GUI.contentColor = Color.white;
                // Tag
                EditorGUI.PropertyField(new Rect
                    (currentRect.x + currentRect.width * 0.06f, currentRect.y, currentRect.width * 0.39f, currentRect.height)
                    , serializedObject.FindProperty("sceneObjectTag"), empty);
                // Layer
                EditorGUI.PropertyField(new Rect
                    (currentRect.x + currentRect.width * 0.59f, currentRect.y, currentRect.width * 0.41f, currentRect.height)
                    , serializedObject.FindProperty("sceneObjectLayer"), empty);

                // Tag Button
                if (GUI.Button(new Rect
                    (currentRect.x + currentRect.width * 0.455f, currentRect.y, currentRect.width * 0.05f, currentRect.height * 0.9f),
                    EditorGUIUtility.IconContent("d_FilterByLabel")))
                {
                    EditorHelper.GetSceneObjectTagDatabase();
                }
            }

            EditorGUILayout.Space(10f);

            //EditorGUILayout.LabelField(baseSceneObject.DisplayName, headerStyle);
            EditorGUI.DropShadowLabel(EditorGUILayout.GetControlRect(false, 20f), baseSceneObject.DisplayName);
            
            //EditorGUILayout.Space(5f);

            if (DrawChildInspector) DrawPropertiesExcluding(serializedObject, "m_Script");

            serializedObject.ApplyModifiedProperties();
        }
    }
}

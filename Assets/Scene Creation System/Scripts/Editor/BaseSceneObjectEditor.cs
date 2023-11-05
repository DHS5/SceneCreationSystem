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

        protected string[] pageNames = new string[] { "Default", "Scene Vars", "Dependency" };

        Color backgroundColor;
        Color foregroundColor;

        bool isManager = false;


        protected virtual void OnEnable()
        {
            baseSceneObject = target as BaseSceneObject;
            if (baseSceneObject is SceneManager) isManager = true;

            pageNames[0] = baseSceneObject.DisplayName;

            baseSceneObject.OnEditorEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            int header = Header();

            EditorGUILayout.Space(10f);

            //EditorGUILayout.LabelField(baseSceneObject.DisplayName, headerStyle);
            EditorGUI.DropShadowLabel(EditorGUILayout.GetControlRect(false, 20f),
                header == 0 ? baseSceneObject.DisplayName : pageNames[header]);

            EditorGUILayout.Space(5f);

            switch (header)
            {
                case 0:
                    DrawDefault();
                    break;
                case 1:
                    DrawSceneVars();
                    break;
                case 2:
                    DrawDependencies();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawDefault()
        {
            DrawPropertiesExcluding(serializedObject, "m_Script");
        }
        protected virtual void DrawSceneVars()
        {
            Editor editor = Editor.CreateEditor(baseSceneObject.SceneVariablesSO);
            editor.OnInspectorGUI();
        }

        protected virtual void DrawDependencies()
        {
            Rect r = EditorGUILayout.GetControlRect(false, 0f);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneDependency"), true);

            if (GUI.Button(new Rect(
                r.x + r.width * 0.44f, r.y + EditorGUIUtility.singleLineHeight * 1.7f, 25f, EditorGUIUtility.singleLineHeight * 0.9f),
                EditorGUIUtility.IconContent("d_RotateTool On")))
            {
                baseSceneObject.RefreshDependencies();
            }

            if (GUI.Button(new Rect(
                r.x + r.width * 0.95f, r.y + EditorGUIUtility.singleLineHeight * 1.7f, 25f, EditorGUIUtility.singleLineHeight * 0.9f),
                EditorGUIUtility.IconContent("d_RotateTool On")))
            {
                baseSceneObject.RefreshDependants();
            }
        }

        protected int Header()
        {
            return Header(pageNames);
        }
        protected int Header(string[] menuNames)
        {
            backgroundColor = new Color32(191, 247, 255, 255);
            foregroundColor = new Color(1f, 0.49f, 0f);

            GUIStyle headerStyle = new()
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
            };
            headerStyle.normal.textColor = Color.white;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), empty);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(2f);

            if (!isManager && baseSceneObject.SceneVariablesSO == null)
            {
                EditorGUILayout.HelpBox("You need to setup the SceneManager and its SceneVariablesSO", MessageType.Error);
                return 0;
            }
            else
            {
                Rect lineRect = EditorGUILayout.GetControlRect(false, 2f);
                lineRect.x -= 18f;
                lineRect.width += 25f;
                EditorGUI.DrawRect(lineRect, foregroundColor);
                lineRect.y += 76f;
                EditorGUI.DrawRect(lineRect, foregroundColor);

                Rect backgroundRect = EditorGUILayout.GetControlRect(false, 1f);
                backgroundRect.height = 72f;
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

                GUILayout.Space(2f);
                SerializedProperty pageProp = serializedObject.FindProperty("currentPage");
                pageProp.intValue = GUILayout.Toolbar(pageProp.intValue, menuNames);

                return baseSceneObject.SceneVariablesSO == null ? 0 : pageProp.intValue;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    public class SceneStateWindow : EditorWindow
    {
        [MenuItem("SCS/Runtime/Scene State")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SceneStateWindow));
        }


        private Vector2 currentPosition = Vector2.zero;

        private bool filterByType;
        private SceneVarType typeFilter;

        private bool displayStatics = true;
        private bool displayEvents;
        private bool displayRandoms;

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("Active only at runtime");
                return;
            }

            currentPosition = EditorGUILayout.BeginScrollView(currentPosition);
            EditorGUILayout.BeginVertical();

            EditorGUI.DropShadowLabel(EditorGUILayout.GetControlRect(false, 20f), "Scene Vars");

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Filters");
            // Type
            EditorGUILayout.BeginHorizontal();
            filterByType = EditorGUILayout.ToggleLeft("Filter by type", filterByType);
            if (filterByType)
            {
                typeFilter = (SceneVarType)EditorGUILayout.EnumPopup("Type Filter", typeFilter);
            }
            EditorGUILayout.EndHorizontal();
            // Statics
            displayStatics = EditorGUILayout.ToggleLeft("Display Statics", displayStatics);
            // Events
            displayEvents = EditorGUILayout.ToggleLeft("Display Events", displayEvents);
            // Events
            displayRandoms = EditorGUILayout.ToggleLeft("Display Randoms", displayRandoms);

            EditorGUILayout.Space();

            SceneVar sceneVar;
            foreach (var pair in SceneState.GetCurrentSceneVars())
            {
                sceneVar = pair.Value;
                if ((!filterByType || sceneVar.type == typeFilter) 
                    && (displayStatics || !sceneVar.IsStatic)
                    && (displayEvents || sceneVar.type != SceneVarType.EVENT)
                    && (displayRandoms || (!sceneVar.IsRandom && !sceneVar.IsLinkRandom)))
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(pair.Value.RuntimeCompleteString());

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}

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
            if (Application.isPlaying)
            {
                EditorWindow.GetWindow(typeof(SceneStateWindow));
            }
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.LabelField("Use only at runtime");
                return;
            }

            EditorGUILayout.BeginVertical();

            foreach (var pair in SceneState.GetCurrentSceneVars())
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(pair.Value.RuntimeCompleteString());

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}

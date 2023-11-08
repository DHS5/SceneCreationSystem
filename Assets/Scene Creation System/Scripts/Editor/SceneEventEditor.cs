using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CustomPropertyDrawer(typeof(BaseSceneEvent), true)]
    public class BaseSceneEventEditor : PropertyDrawer
    {
        protected SerializedProperty idProperty;
        protected SerializedProperty detailsProperty;
        protected SerializedProperty pageProperty;
        protected SerializedProperty conditionsProperty;
        protected SerializedProperty actionsProperty;
        protected SerializedProperty paramedEventProperty;
        protected SerializedProperty uEventProperty;

        protected string[] pageNames = new string[] { "Condition", "Scene Action", "UnityEvent", "Parametered Event" };

        protected float height;

        ReorderableList sceneParamedList;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            idProperty = property.FindPropertyRelative("eventID");
            detailsProperty = property.FindPropertyRelative("details");
            pageProperty = property.FindPropertyRelative("page");
            conditionsProperty = property.FindPropertyRelative("sceneConditions");
            actionsProperty = property.FindPropertyRelative("sceneActions");
            paramedEventProperty = property.FindPropertyRelative("sceneParameteredEvents");
            uEventProperty = property.FindPropertyRelative("unityEvent");

            if (sceneParamedList == null) sceneParamedList = CreateSceneParameteredEventsList(property);

            EditorGUI.BeginProperty(position, label, property);

            Rect r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, idProperty.stringValue);
            r.y += EditorGUIUtility.singleLineHeight * 1.5f;

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUI.PropertyField(r, idProperty);
                r.y += EditorGUI.GetPropertyHeight(idProperty);
                
                EditorGUI.PropertyField(r, detailsProperty, true);
                r.y += EditorGUI.GetPropertyHeight(detailsProperty);

                r.y += EditorGUIUtility.singleLineHeight;
                r.height = EditorGUIUtility.singleLineHeight * 2.2f;
                pageProperty.intValue = GUI.SelectionGrid(r, pageProperty.intValue, pageNames, 2);

                //height = r.y;

                r.y += EditorGUIUtility.singleLineHeight * 2.5f;
                r.height = EditorGUIUtility.singleLineHeight;

                height = r.y - position.y;

                DrawCurrentPage(r, property, pageProperty.intValue);

                EditorGUI.indentLevel--;
            }

            height += EditorGUIUtility.singleLineHeight * 0.5f;
            property.FindPropertyRelative("propertyHeight").floatValue = height;
            EditorGUI.EndProperty();
        }

        private void DrawCurrentPage(Rect r, SerializedProperty property, int pageIndex)
        {
            switch (pageIndex)
            {
                case 0:
                    {
                        EditorGUI.PropertyField(r, conditionsProperty, true);
                        height += EditorGUI.GetPropertyHeight(conditionsProperty);
                        break;
                    }
                case 1:
                    {
                        EditorGUI.PropertyField(r, actionsProperty, true);
                        height += EditorGUI.GetPropertyHeight(actionsProperty);
                        break;
                    }
                case 2:
                    {
                        EditorGUI.PropertyField(r, uEventProperty, true);
                        height += EditorGUI.GetPropertyHeight(uEventProperty);
                        break;
                    }
                case 3:
                    {
                        //EditorGUI.PropertyField(r, paramedEventProperty, true);
                        //height += EditorGUI.GetPropertyHeight(paramedEventProperty);
                        sceneParamedList.DoList(r);
                        height += Mathf.Max(sceneParamedList.GetHeight(), EditorGUIUtility.singleLineHeight * 5f);
                        break;
                    }
            }
        }

        private ReorderableList CreateSceneParameteredEventsList(SerializedProperty property)
        {
            return new ReorderableList(property.serializedObject, paramedEventProperty, true, true, true, true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Scene Parametered Events");
                },
                drawElementCallback = (rect, index, active, focused) =>
                {
                    EditorGUI.PropertyField(rect, paramedEventProperty.GetArrayElementAtIndex(index), true);
                },
                elementHeightCallback = index =>
                {
                    return EditorGUI.GetPropertyHeight(paramedEventProperty.GetArrayElementAtIndex(index));
                },
                elementHeight = EditorGUIUtility.singleLineHeight * 2.5f
            };
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? property.FindPropertyRelative("propertyHeight").floatValue 
                : EditorGUI.GetPropertyHeight(property, true);
        }
    }
}

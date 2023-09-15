using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CustomPropertyDrawer(typeof(SceneDependency))]
    public class SceneDependencyEditor : PropertyDrawer
    {
        SerializedProperty sceneVariablesSO;
        SerializedObject sceneVariablesObj;
        SceneVariablesSO sceneVarContainer;

        SerializedProperty tweenProperty;
        SerializedProperty sceneObjectsProperty;

        float propertyOffset;
        float propertyHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            propertyOffset = 0f;
            propertyHeight = 0f;

            tweenProperty = property.FindPropertyRelative("sceneVar");

            EditorGUI.BeginProperty(position, label, property);

            // SceneVariablesSO
            sceneVariablesSO = property.FindPropertyRelative("sceneVariablesSO");
            if (sceneVariablesSO.objectReferenceValue == null)
            {
                EditorGUI.LabelField(position, "SceneVariablesSO is not assigned !");
                EditorGUI.EndProperty();
                return;
            }
            // Get the SceneVariablesSO
            sceneVariablesObj = new SerializedObject(sceneVariablesSO.objectReferenceValue);
            sceneVarContainer = sceneVariablesObj.targetObject as SceneVariablesSO;
            if (sceneVarContainer == null)
            {
                EditorGUI.LabelField(position, "SceneVariablesSO is null !");
                EditorGUI.EndProperty();
                return;
            }


            // Tween & Foldout
            Rect tweenRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect foldoutRect = new Rect(position.x, position.y + 5, position.width, EditorGUIUtility.singleLineHeight);
            propertyOffset += EditorGUI.GetPropertyHeight(tweenProperty) + 5;
            propertyHeight += EditorGUI.GetPropertyHeight(tweenProperty) + 5;

            // Button
            Rect buttonRect = new Rect(position.x + position.width * 0.05f, position.y + propertyOffset, position.width * 0.9f, EditorGUIUtility.singleLineHeight);
            propertyOffset += EditorGUIUtility.singleLineHeight * 1.25f;
            if (property.isExpanded) propertyHeight += EditorGUIUtility.singleLineHeight * 1.25f;

            // List
            Rect listRect = new Rect(position.x + position.width * 0.05f, position.y + propertyOffset, position.width * 0.9f, EditorGUIUtility.singleLineHeight);

            if (property.isExpanded)
            {
                Rect colorRect = new Rect(position.x, position.y, position.width, propertyHeight);
                EditorGUI.DrawRect(colorRect, Color.grey);
            }

            EditorGUI.indentLevel++;
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, "");
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(tweenRect, tweenProperty);

            if (property.isExpanded)
            {
                SerializedProperty listProperty = property.FindPropertyRelative("sceneObjects");

                if (GUI.Button(buttonRect, "Get dependencies"))
                {
                    List<SceneObject> sceneObjects = SceneDependency.GetDependencies(
                        sceneVarContainer, tweenProperty.FindPropertyRelative("sceneVarUniqueID").intValue);

                    listProperty.ClearArray();

                    for (int i = 0; i < sceneObjects.Count; i++)
                    {
                        listProperty.InsertArrayElementAtIndex(i);
                        listProperty.GetArrayElementAtIndex(i).objectReferenceValue = sceneObjects[i];
                    }
                }

                ReorderableList list;
                
                if (listProperty.arraySize > 0)
                {
                    list = new ReorderableList(property.serializedObject, listProperty, false, true, false, false)
                    {
                        drawHeaderCallback = rect =>
                        {
                            EditorGUI.LabelField(rect, "Dependencies");
                        },

                        drawElementCallback = (rect, index, active, focused) =>
                        {
                            var element = listProperty.GetArrayElementAtIndex(index);

                            EditorGUI.indentLevel++;
                            EditorGUI.PropertyField(rect, element, new GUIContent("Dependency " + index), true);
                            EditorGUI.indentLevel--;
                        },

                        elementHeightCallback = index =>
                        {
                            var element = listProperty.GetArrayElementAtIndex(index);

                            return EditorGUI.GetPropertyHeight(element);
                        }
                    };

                    propertyHeight += list.GetHeight() - 15;
                    Rect secondColorRect = new Rect(position.x, position.y + propertyOffset, position.width, propertyHeight - propertyOffset);
                    EditorGUI.DrawRect(secondColorRect, Color.grey);
                    
                    EditorGUI.BeginDisabledGroup(true);
                    list.DoList(listRect);
                    EditorGUI.EndDisabledGroup();
                }
            }

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            // End
            EditorGUI.EndProperty();

            property.FindPropertyRelative("propertyHeight").floatValue = propertyHeight;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? property.FindPropertyRelative("propertyHeight").floatValue : EditorGUIUtility.singleLineHeight * 1.3f;
        }
    }
}

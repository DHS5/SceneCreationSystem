using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CustomPropertyDrawer(typeof(SceneDependency))]
    public class SceneDependencyEditor : PropertyDrawer
    {
        SerializedProperty tweenProperty;

        float propertyOffset;
        float propertyHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            propertyOffset = 0f;
            propertyHeight = 0f;

            tweenProperty = property.FindPropertyRelative("sceneVar");

            EditorGUI.BeginProperty(position, label, property);

            // Tween
            Rect tweenRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            propertyOffset += EditorGUI.GetPropertyHeight(tweenProperty);
            propertyHeight += EditorGUI.GetPropertyHeight(tweenProperty);

            Rect colorRect = new Rect(position.x, position.y, position.width, propertyHeight);
            EditorGUI.DrawRect(colorRect, Color.grey);

            EditorGUI.indentLevel++;

            EditorGUI.PropertyField(tweenRect, tweenProperty);

            EditorGUI.indentLevel--;
            // End
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 1.5f;
        }
    }
}

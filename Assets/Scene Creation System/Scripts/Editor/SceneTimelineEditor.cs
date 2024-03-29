using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dhs5.SceneCreation
{
    [CustomPropertyDrawer(typeof(SceneTimeline))]
    public class SceneTimelineEditor : PropertyDrawer
    {
        private float propertyOffset;
        
        private SerializedProperty idProperty;
        private SerializedProperty loopProperty;
        private SerializedProperty conditionProperty;
        private SerializedProperty timelineObjectsProperty;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            propertyOffset = 0;
            
            idProperty = property.FindPropertyRelative("ID");
            loopProperty = property.FindPropertyRelative("loop");
            conditionProperty = property.FindPropertyRelative("endLoopCondition");
            timelineObjectsProperty = property.FindPropertyRelative("steps");

            EditorGUI.BeginProperty(position, label, property);

            Rect foldoutPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, property.isExpanded ? "" : idProperty.stringValue);
            if (property.isExpanded)
            {
                Rect idPosition = new Rect(position.x + 15, position.y + propertyOffset, position.width - 15, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(idPosition, idProperty);
                propertyOffset += EditorGUIUtility.singleLineHeight * 1.2f;

                Rect loopPosition = new Rect(position.x, position.y + propertyOffset, position.width,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(loopPosition, loopProperty);
                propertyOffset += EditorGUIUtility.singleLineHeight * 1.2f;

                if (loopProperty.boolValue)
                {
                    Rect conditionPosition = new Rect(position.x, position.y + propertyOffset, position.width,
                        EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(conditionPosition, conditionProperty, new GUIContent("Loop End-condition"));
                    propertyOffset += EditorGUI.GetPropertyHeight(conditionProperty);
                }

                Rect timelineObjPosition = new Rect(position.x, position.y + propertyOffset, position.width,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(timelineObjPosition, timelineObjectsProperty);
                propertyOffset += EditorGUI.GetPropertyHeight(timelineObjectsProperty);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            idProperty = property.FindPropertyRelative("ID");
            loopProperty = property.FindPropertyRelative("loop");
            conditionProperty = property.FindPropertyRelative("endLoopCondition");
            timelineObjectsProperty = property.FindPropertyRelative("steps");

            return property.isExpanded ? 
                EditorGUIUtility.singleLineHeight * 2.4f + EditorGUI.GetPropertyHeight(timelineObjectsProperty)
                    + (loopProperty.boolValue ? EditorGUI.GetPropertyHeight(conditionProperty) : 0)
                    : EditorGUIUtility.singleLineHeight * 1.3f;
        }
    }
}

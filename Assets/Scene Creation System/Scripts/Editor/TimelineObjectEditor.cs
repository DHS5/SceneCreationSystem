using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dhs5.SceneCreation
{
    [CustomPropertyDrawer(typeof(TimelineObject))]
    public class TimelineObjectEditor : PropertyDrawer
    {
        private float propertyOffset;
        
        private SerializedProperty loopProperty;
        private SerializedProperty startConditionProperty;
        private SerializedProperty endConditionProperty;
        private SerializedProperty eventsProperty;
        //private SerializedProperty timelineEventsProperty;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            propertyOffset = 0;
            
            loopProperty = property.FindPropertyRelative("loop");
            startConditionProperty = property.FindPropertyRelative("startCondition");
            endConditionProperty = property.FindPropertyRelative("endLoopCondition");
            eventsProperty = property.FindPropertyRelative("sceneEvents");
            //timelineEventsProperty = property.FindPropertyRelative("sceneTimelineEvents");

            EditorGUI.BeginProperty(position, label, property);

            //Rect foldoutPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            //property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, label.text.Replace("Element", "Step"));
            //propertyOffset += EditorGUIUtility.singleLineHeight;
            if (true)//property.isExpanded)
            {
                Rect startConditionPosition = new Rect(position.x, position.y + propertyOffset, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(startConditionPosition, startConditionProperty, new GUIContent("Trigger condition"));
                propertyOffset += EditorGUI.GetPropertyHeight(startConditionProperty) + 3f;

                Rect loopPosition = new Rect(position.x, position.y + propertyOffset, position.width,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(loopPosition, loopProperty);
                propertyOffset += EditorGUIUtility.singleLineHeight + 3f;

                if (loopProperty.boolValue)
                {
                    Rect conditionPosition = new Rect(position.x, position.y + propertyOffset, position.width,
                        EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(conditionPosition, endConditionProperty, new GUIContent("Loop End-condition"));
                    propertyOffset += EditorGUI.GetPropertyHeight(endConditionProperty);
                }

                Rect sceneEventsPosition = new Rect(position.x, position.y + propertyOffset, position.width,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(sceneEventsPosition, eventsProperty);
                propertyOffset += EditorGUI.GetPropertyHeight(eventsProperty);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            loopProperty = property.FindPropertyRelative("loop");
            startConditionProperty = property.FindPropertyRelative("startCondition");
            endConditionProperty = property.FindPropertyRelative("endLoopCondition");
            eventsProperty = property.FindPropertyRelative("sceneEvents");

            return EditorGUIUtility.singleLineHeight + 6f + EditorGUI.GetPropertyHeight(startConditionProperty)
                    + EditorGUI.GetPropertyHeight(eventsProperty)
                    + (loopProperty.boolValue ? EditorGUI.GetPropertyHeight(endConditionProperty) : 0);
        }
    }
}

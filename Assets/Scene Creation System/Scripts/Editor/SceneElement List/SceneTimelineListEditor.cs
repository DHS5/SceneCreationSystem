using Codice.CM.SEIDInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Dhs5.SceneCreation
{
    [CustomPropertyDrawer(typeof(SceneTimelineList))]
    public class SceneTimelineListEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.FindPropertyRelative("propertyHeight").floatValue;
        }


        SerializedProperty listProp;

        SerializedProperty timelineIndexProp;
        SerializedProperty stepIndexProp;
        
        SerializedProperty currentTimelineProp;
        SerializedProperty stepsProp;
        SerializedProperty currentStepProp;


        SerializedProperty idProp;
        SerializedProperty loopProp;
        SerializedProperty loopEndProp;

        int currentTimelineIndex;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            listProp = property.FindPropertyRelative("list");

            timelineIndexProp = property.FindPropertyRelative("currentTimeline");
            stepIndexProp = property.FindPropertyRelative("currentStep");
            

            EditorGUI.BeginProperty(position, label, property);

            Rect r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label);
            r.y += EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                Indent(10f);

                // Display Timeline Choice
                if (GUI.Button(new Rect(r.x + (r.width - 52f), r.y, 25f, r.height),
                    EditorGUIUtility.IconContent("d_Toolbar Plus")))
                {
                    AddTimeline();
                }

                if (listProp.arraySize == 0)
                {
                    EditorGUI.LabelField(r, "No Timeline yet");
                    r.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    Indent(-10f);
                    End();
                    return;
                }

                if (GUI.Button(new Rect(r.x + (r.width - 25f), r.y, 25f, r.height),
                    EditorGUIUtility.IconContent("d_Toolbar Minus")))
                {
                    RemoveTimeline(timelineIndexProp.intValue);
                }

                (string[] options, int[] values) = GetTimelineDisplayOptions(listProp);
                timelineIndexProp.intValue = EditorGUI.IntPopup(new Rect(r.x, r.y, r.width - 55f, r.height),
                    timelineIndexProp.intValue, options, values);
                if (currentTimelineIndex != timelineIndexProp.intValue)
                {
                    stepIndexProp.intValue = 0;
                    currentTimelineIndex = timelineIndexProp.intValue;
                }

                currentTimelineProp = listProp.GetArrayElementAtIndex(timelineIndexProp.intValue);
                stepsProp = currentTimelineProp.FindPropertyRelative("steps");

                r.y += EditorGUIUtility.singleLineHeight * 1.25f;

                // Display Timeline Params
                idProp = currentTimelineProp.FindPropertyRelative("ID");
                loopProp = currentTimelineProp.FindPropertyRelative("loop");
                loopEndProp = currentTimelineProp.FindPropertyRelative("endLoopCondition");

                Indent(10f);

                EditorGUI.PropertyField(r, idProp);
                r.y += EditorGUI.GetPropertyHeight(idProp);
                
                EditorGUI.PropertyField(r, loopProp);
                r.y += EditorGUI.GetPropertyHeight(loopProp);

                if (loopProp.boolValue)
                {
                    EditorGUI.PropertyField(r, loopEndProp);
                    r.y += EditorGUI.GetPropertyHeight(loopEndProp);
                }

                r.y += EditorGUIUtility.singleLineHeight * 0.5f;

                Indent(-10f);

                // Display Step Choice
                if (stepIndexProp.intValue > 0 && 
                    GUI.Button(new Rect(r.x + (r.width - 106f), r.y, 25f, r.height),
                    EditorGUIUtility.IconContent("HoverBar_Up")))
                {
                    MoveStep(true, stepIndexProp.intValue);
                }
                if (stepIndexProp.intValue < stepsProp.arraySize - 1 && 
                    GUI.Button(new Rect(r.x + (r.width - 79f), r.y, 25f, r.height),
                    EditorGUIUtility.IconContent("HoverBar_Down")))
                {
                    MoveStep(false, stepIndexProp.intValue);
                }
                
                if (GUI.Button(new Rect(r.x + (r.width - 52f), r.y, 25f, r.height),
                    EditorGUIUtility.IconContent("d_Toolbar Plus")))
                {
                    AddStep();
                }

                if (stepsProp.arraySize == 0)
                {
                    EditorGUI.LabelField(r, "No Step yet");
                    r.y += EditorGUIUtility.singleLineHeight * 1.25f;
                    Indent(-10f);
                    End();
                    return;
                }

                if (GUI.Button(new Rect(r.x + (r.width - 25f), r.y, 25f, r.height),
                    EditorGUIUtility.IconContent("d_Toolbar Minus")))
                {
                    RemoveStep(stepIndexProp.intValue);
                    if (stepsProp.arraySize < 1)
                    {
                        Indent(-10f);
                        End();
                        return;
                    }
                }

                (string[] steps, int[] stepValues) = GetStepsDisplayOptions(stepsProp);
                stepIndexProp.intValue = EditorGUI.IntPopup(new Rect(r.x, r.y, r.width - 110f, r.height),
                    stepIndexProp.intValue, steps, stepValues);

                currentStepProp = stepsProp.GetArrayElementAtIndex(stepIndexProp.intValue);

                r.y += EditorGUIUtility.singleLineHeight * 1.25f;

                Indent(10f);

                EditorGUI.PropertyField(r, currentStepProp, true);
                r.y += EditorGUI.GetPropertyHeight(currentStepProp);
                Indent(-10f);

                Indent(-10f);
            }

            End();

            void End()
            {
                property.FindPropertyRelative("propertyHeight").floatValue = r.y - position.y;
                EditorGUI.EndProperty();
            }
            void Indent(float value)
            {
                r.x += value;
                r.width -= value;
            }
        }

        private void AddTimeline()
        {
            listProp.InsertArrayElementAtIndex(listProp.arraySize);
            listProp.GetArrayElementAtIndex(listProp.arraySize - 1).FindPropertyRelative("ID").stringValue = "Timeline " + listProp.arraySize;
            listProp.GetArrayElementAtIndex(listProp.arraySize - 1).FindPropertyRelative("loop").boolValue = false;
            listProp.GetArrayElementAtIndex(listProp.arraySize - 1).FindPropertyRelative("steps").ClearArray();
            timelineIndexProp.intValue = listProp.arraySize - 1;
            stepIndexProp.intValue = 0;
        }
        private void RemoveTimeline(int index)
        {
            listProp.DeleteArrayElementAtIndex(index);
            timelineIndexProp.intValue = Mathf.Clamp(index, 0, listProp.arraySize - 1);
            stepIndexProp.intValue = 0;
        }

        private (string[], int[]) GetTimelineDisplayOptions(SerializedProperty property)
        {
            string id;
            string[] options = new string[property.arraySize];
            int[] values = new int[property.arraySize];
            for (int i = 0; i < options.Length; i++)
            {
                id = property.GetArrayElementAtIndex(i).FindPropertyRelative("ID").stringValue;
                options[i] = string.IsNullOrEmpty(id) ? ("Element " + i) : (i + ": " + id);
                values[i] = i;
            }
            return (options, values);
        }

        private void AddStep()
        {
            stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
            stepIndexProp.intValue = stepsProp.arraySize - 1;
        }
        private void RemoveStep(int index)
        {
            stepsProp.DeleteArrayElementAtIndex(index);
            stepIndexProp.intValue = Mathf.Clamp(index, 0, stepsProp.arraySize - 1);
        }
        private void MoveStep(bool up, int index)
        {
            int newIndex = up ? index - 1 : index + 1;
            stepsProp.MoveArrayElement(index, newIndex);
            stepIndexProp.intValue = newIndex;
        }
        private (string[], int[]) GetStepsDisplayOptions(SerializedProperty property)
        {
            string[] options = new string[property.arraySize];
            int[] values = new int[property.arraySize];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = "Step " + i;
                values[i] = i;
            }
            return (options, values);
        }
    }
}

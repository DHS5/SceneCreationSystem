using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Events;
using UnityEditor;
using System.Reflection;
using System;
using System.Text;

namespace Dhs5.SceneCreation
{
    [CustomPropertyDrawer(typeof(SceneParameteredEvent))]
    public class SceneParameteredEventEditor : PropertyDrawer
    {
        SerializedProperty objProperty;
        SerializedProperty componentProperty;
        SerializedProperty tokenProperty;
        SerializedProperty actionProperty;

        int methodIndex;
        float propertyOffset;
        float propertyHeight;

        GUIContent empty = new("");
        string[] noCompList = new string[1] { "No component" };
        string[] noFuncList = new string[1] { "No function" };

        BindingFlags fieldBF = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance;

        #region Functions
        private bool ValidParameters(ParameterInfo[] parameters)
        {
            foreach (var param in parameters)
            {
                if (param.ParameterType != typeof(int)
                    && param.ParameterType != typeof(float)
                    && param.ParameterType != typeof(bool)
                    && param.ParameterType != typeof(string))
                    return false;
            }
            return true;
        }

        private string MethodName(MethodInfo method)
        {
            StringBuilder sb = new();
            sb.Append(method.Name);

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                sb.Append(" (");
                for (int i = 0; i < parameters.Length - 1; i++)
                {
                    sb.Append(ParameterName(parameters[i]));
                    sb.Append(", ");
                }
                sb.Append(ParameterName(parameters[parameters.Length - 1]));
                sb.Append(")");
            }
            return sb.ToString();
        }

        private string ParameterName(ParameterInfo param)
        {
            StringBuilder sb = new();
            switch (param.ParameterType.Name)
            {
                case "String":
                    sb.Append("string");
                    break;
                case "Int32":
                    sb.Append("int");
                    break;
                case "Single":
                    sb.Append("float");
                    break;
                case "Boolean":
                    sb.Append("bool");
                    break;
            }
            sb.Append(" ");
            sb.Append(param.Name);

            return sb.ToString();
        }

        private string GetComponentName(Component component)
        {
            string[] strings = component.GetType().Name.Split('.');
            return strings[strings.Length - 1];
        }
        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            propertyOffset = 0;
            propertyHeight = 0;

            componentProperty = property.FindPropertyRelative("component");
            actionProperty = property.FindPropertyRelative("action");
            tokenProperty = property.FindPropertyRelative("metadataToken");
            objProperty = property.FindPropertyRelative("obj");

            EditorGUI.BeginProperty(position, label, property);

            if (property.isExpanded)
            {
                Rect backgroundRect1 = new Rect(position.x - 10, position.y, position.width + 11, property.FindPropertyRelative("propertyHeight").floatValue);
                EditorGUI.DrawRect(backgroundRect1, new Color(0.22f, 0.22f, 0.22f));
            }
            // FOLDOUT
            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, property.isExpanded ? empty : label, EditorStyles.foldoutHeader);
            propertyOffset += EditorGUIUtility.singleLineHeight * 0.25f;
            propertyHeight += EditorGUIUtility.singleLineHeight * 0.25f;

            if (property.isExpanded)
            {
                Rect objRect = new Rect(position.x + 20, position.y + propertyOffset, (position.width - 20) * 0.3f, EditorGUIUtility.singleLineHeight);
                Rect compRect = new Rect(position.x + 20 + (position.width - 20) * 0.31f, position.y + propertyOffset, (position.width - 20) * 0.69f, EditorGUIUtility.singleLineHeight);
                propertyOffset += EditorGUIUtility.singleLineHeight * 1.2f;
                propertyHeight += EditorGUIUtility.singleLineHeight * 1.2f;
                Rect methodsRect = new Rect(position.x, position.y + propertyOffset, position.width, EditorGUIUtility.singleLineHeight);
                propertyOffset += EditorGUIUtility.singleLineHeight * 1.2f;
                propertyHeight += EditorGUIUtility.singleLineHeight * 1.2f;

                EditorGUI.PropertyField(objRect, objProperty, new GUIContent(""));
                // Object
                UnityEngine.Object obj = objProperty.objectReferenceValue;
                UnityEngine.Object target = null;
                Component component = null;
                MethodInfo[] methods = null;

                if (obj == null)
                {
                    EditorGUI.Popup(compRect, 0, noCompList);
                    EditorGUI.Popup(methodsRect, 0, noFuncList);
                    EditorGUI.EndProperty();
                    propertyHeight += EditorGUIUtility.singleLineHeight * 0.25f;
                    property.FindPropertyRelative("propertyHeight").floatValue = propertyHeight;
                    return;
                }

                if (obj is Component cmp)
                {
                    obj = cmp.gameObject;
                }
                // GameObject components
                if (obj is GameObject go)
                {
                    int compIndex = 0;
                    Component[] goComponents = go.GetComponents(typeof(Component));
                    List<string> compNames = new();
                    int j = 0;
                    foreach (Component comp in goComponents)
                    {
                        if (comp == componentProperty.objectReferenceValue) compIndex = j;//(UnityEngine.Object)
                        j++;

                        string compName = GetComponentName(comp);
                        int i = 2;
                        string compNameTemp = compName;
                        while (compNames.Contains(compNameTemp))
                        {
                            compNameTemp = compName + i;
                            i++;
                        }
                        compNames.Add(compNameTemp);
                    }

                    compIndex = EditorGUI.Popup(compRect, compIndex, compNames.ToArray());
                    component = goComponents[compIndex];
                    target = component;
                    componentProperty.objectReferenceValue = component;
                    methods = component.GetType().GetMethods();
                }
                else if (obj is ScriptableObject so)
                {
                    target = so;
                    methods = so.GetType().GetMethods();
                }
                else
                {
                    EditorGUI.Popup(compRect, 0, noCompList);
                    EditorGUI.Popup(methodsRect, 0, noFuncList);
                    property.FindPropertyRelative("propertyHeight").floatValue = propertyHeight;
                    EditorGUI.EndProperty();
                    return;
                }
                
                // Methods
                List<MethodInfo> methodInfos = new();
                List<string> methodNames = new();
                foreach (MethodInfo method in methods)
                {
                    if (method.IsPublic && !method.IsAbstract && !method.IsGenericMethod && !method.IsConstructor && !method.IsAssembly
                        && !method.IsFamily && !method.ContainsGenericParameters && !method.IsSpecialName && method.ReturnType == typeof(void)
                        && ValidParameters(method.GetParameters()) && method.GetCustomAttribute(typeof(PreserveAttribute)) != null)
                    {
                        methodNames.Add(MethodName(method));
                        methodInfos.Add(method);
                    }
                }
                if (methodInfos.Count == 0)
                {
                    EditorGUI.Popup(methodsRect, 0, noFuncList);
                    property.FindPropertyRelative("propertyHeight").floatValue = propertyHeight;
                    EditorGUI.EndProperty();
                    return;
                }

                methodIndex = methodInfos.FindIndex(m => m.MetadataToken == tokenProperty.intValue);
                if (methodIndex == -1) methodIndex = 0;
                //_______________________________________

                methodIndex = EditorGUI.Popup(methodsRect, methodIndex, methodNames.ToArray());

                tokenProperty.intValue = methodInfos[methodIndex].MetadataToken;
                ParameterInfo[] parameters = methodInfos[methodIndex].GetParameters();
                BaseEventAction.Argument[] parameterValues = new BaseEventAction.Argument[parameters.Length];
                
                // ---------------
                SerializedProperty varTweenProperty;
                for (int i = 0; i < parameters.Length; i++)
                {
                    Rect valueRect = new Rect(position.x, position.y + propertyOffset, position.width, EditorGUIUtility.singleLineHeight);

                    varTweenProperty = property.FindPropertyRelative("varTween" + i);
                    EditorGUI.PropertyField(valueRect, varTweenProperty, new GUIContent(parameters[i].Name));
                    switch (parameters[i].ParameterType.Name)
                    {
                        case nameof(System.Single):
                            property.FindPropertyRelative("varType" + i).enumValueIndex = (int)SceneVarType.FLOAT;
                            parameterValues[i] = new(0f);
                            break;
                        case nameof(System.Boolean):
                            property.FindPropertyRelative("varType" + i).enumValueIndex = (int)SceneVarType.BOOL;
                            parameterValues[i] = new(false);
                            break;
                        case nameof(System.Int32):
                            property.FindPropertyRelative("varType" + i).enumValueIndex = (int)SceneVarType.INT;
                            parameterValues[i] = new(5);
                            break;
                        case nameof(System.String):
                            property.FindPropertyRelative("varType" + i).enumValueIndex = (int)SceneVarType.STRING;
                            parameterValues[i] = new("");
                            break;
                    }
                    propertyOffset += EditorGUI.GetPropertyHeight(varTweenProperty);
                    propertyHeight += EditorGUI.GetPropertyHeight(varTweenProperty);
                }

                FieldInfo objField = property.serializedObject.targetObject.GetType().GetField(property.propertyPath);
                FieldInfo actionField = typeof(SceneParameteredEvent).GetField("action");

                if (actionField != null && objField != null && !EditorApplication.isPlaying)
                {
                    BaseEventAction baseEventAction = new(methodInfos[methodIndex].Name, target, parameterValues);
                    actionField.SetValue(objField.GetValue(property.serializedObject.targetObject), baseEventAction);
                }
                else if (objField == null && !EditorApplication.isPlaying)
                {
                    string path = property.propertyPath;
                    string fieldName = path.Substring(0, path.IndexOf('.'));
                    string tryIndex1 = path.Substring(path.IndexOf('[') + 1, path.IndexOf(']') - path.IndexOf('[') - 1);
                    int index1 = int.Parse(tryIndex1);
                    string tryIndex2 = path.Substring(path.LastIndexOf('[') + 1, path.LastIndexOf(']') - path.LastIndexOf('[') - 1);
                    int index2 = int.Parse(tryIndex2);

                    object temp = property.serializedObject.targetObject.GetType().GetField(fieldName, fieldBF).GetValue(property.serializedObject.targetObject);
                    if (temp is IList list) temp = list[index1];
                    //else temp = property.serializedObject.targetObject.GetType().GetField(fieldName).GetValue(property.serializedObject.targetObject);

                    SceneParameteredEvent paramedEvent = (temp.GetType().GetField("sceneParameteredEvents", fieldBF).GetValue(temp) as List<SceneParameteredEvent>)[index2];
                    //SceneParameteredEvent paramedEvent = (property.serializedObject.targetObject.GetType().GetField(fieldName).GetValue(property.serializedObject.targetObject) as List<SceneEvent>)[index1].sceneParameteredEvents[index2];
                    BaseEventAction baseEventAction = new(methodInfos[methodIndex].Name, target, parameterValues);
                    paramedEvent.action = baseEventAction;
                }
            }

            EditorGUI.EndProperty();

            property.FindPropertyRelative("propertyHeight").floatValue = propertyHeight;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? property.FindPropertyRelative("propertyHeight").floatValue : EditorGUIUtility.singleLineHeight;
        }
    }
}

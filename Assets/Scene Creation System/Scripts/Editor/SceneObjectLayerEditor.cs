using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dhs5.SceneCreation
{
    [CustomPropertyDrawer(typeof(SceneObjectLayer))]
    public class SceneObjectLayerEditor : PropertyDrawer
    {
        SerializedProperty valueProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            valueProperty = property.FindPropertyRelative("_value");

            EditorGUI.BeginProperty(position, label, property);


            if (EditorGUI.DropdownButton(position, new GUIContent(SceneObjectLayer.LayerToName(valueProperty.intValue)), FocusType.Passive))
            {
                List<string> list = SceneObjectLayer.Layers;
                GenericMenu menu = new();
                for (int i = 0; i < list.Count; i++)
                {
                    menu.AddItem(new GUIContent(list[i]), false, Choose, i);
                }
                menu.ShowAsContext();
            }

            void Choose(object index)
            {
                valueProperty.intValue = (int)index;
            }

            EditorGUI.EndProperty();
        }
    }
}

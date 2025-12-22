using UnityEditor;
using UnityEngine;

namespace TileProperties
{
    public class TPStringDrawer : ITPDrawer
    {
        public void Draw(Rect rect, Rect label_rect, Rect variable_name_rect, Rect variable_value_rect,
            SerializedProperty element)
        {
            SerializedProperty name_property = element.FindPropertyRelative("VariableName");
            SerializedProperty value_property = element.FindPropertyRelative("Value");

            EditorGUI.LabelField(label_rect, "String");
            EditorGUI.PropertyField(variable_name_rect, name_property, GUIContent.none);
            EditorGUI.PropertyField(variable_value_rect, value_property, GUIContent.none);
        }

        public TPVariableType GetVariable(TilePropertiesList list, int list_index)
        {
            return (TPStringVariable)list.TileProperties[list_index];
        }
    }
}

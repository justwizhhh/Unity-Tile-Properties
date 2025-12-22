using UnityEditor;
using UnityEngine;

namespace TileProperties
{
    public interface ITPDrawer
    {
        public void Draw(
            Rect rect, Rect label_rect, Rect variable_name_rect, Rect variable_value_rect,
            SerializedProperty element);

        public TPVariableType GetVariable(TilePropertiesList list, int list_index);
    }
}
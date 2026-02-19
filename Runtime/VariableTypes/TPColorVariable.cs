using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPColorVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public Color Value;

        public string GetVariableTypeName()
        {
            return "Color";
        }

        public object GetDefaultValue()
        {
            return Color.black;
        }

        public string GetVariableName()
        {
            return VariableName;
        }

        public void SetVariableName(string new_name)
        {
            VariableName = new_name;
        }

        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object new_value)
        {
            Value = (Color)new_value;
        }
    }
}

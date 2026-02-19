using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPRectIntVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public RectInt Value;

        public string GetVariableTypeName()
        {
            return "RectInt";
        }

        public object GetDefaultValue()
        {
            return new RectInt();
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
            Value = (RectInt)new_value;
        }
    }
}
using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPRectVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public Rect Value;

        public string GetVariableTypeName()
        {
            return "Rect";
        }

        public object GetDefaultValue()
        {
            return new Rect();
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
            Value = (Rect)new_value;
        }
    }
}
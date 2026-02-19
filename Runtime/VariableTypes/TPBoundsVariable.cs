using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPBoundsVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public Bounds Value;

        public string GetVariableTypeName()
        {
            return "Bounds";
        }

        public object GetDefaultValue()
        {
            return new Bounds();
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
            Value = (Bounds)new_value;
        }
    }
}
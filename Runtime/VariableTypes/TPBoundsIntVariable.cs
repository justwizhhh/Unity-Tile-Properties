using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPBoundsIntVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public BoundsInt Value;

        public string GetVariableTypeName()
        {
            return "BoundsInt";
        }

        public object GetDefaultValue()
        {
            return new BoundsInt();
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
            Value = (BoundsInt)new_value;
        }
    }
}
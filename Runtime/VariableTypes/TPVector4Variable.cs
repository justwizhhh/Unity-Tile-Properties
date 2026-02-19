using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPVector4Variable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public Vector4 Value;

        public string GetVariableTypeName()
        {
            return "Vector4";
        }

        public object GetDefaultValue()
        {
            return new Vector4(0, 0, 0, 0);
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
            Value = (Vector4)new_value;
        }
    }
}

using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPMaterialVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public Material Value;

        public string GetVariableTypeName()
        {
            return "Material";
        }

        public object GetDefaultValue()
        {
            return null;
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
            Value = (Material)new_value;
        }
    }
}
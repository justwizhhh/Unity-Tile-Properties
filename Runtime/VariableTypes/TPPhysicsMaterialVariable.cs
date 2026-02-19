using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPPhysicsMaterialVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public PhysicsMaterial Value;

        public string GetVariableTypeName()
        {
            return "PhysicsMaterial";
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
            Value = (PhysicsMaterial)new_value;
        }
    }
}
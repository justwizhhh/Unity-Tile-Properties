using UnityEngine;

namespace TileProperties
{
    public struct TPGameObjectVariable : TPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public GameObject Value;

        public string GetVariableTypeName()
        {
            return "GameObject";
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
            Value = (GameObject)new_value;
        }
    }
}

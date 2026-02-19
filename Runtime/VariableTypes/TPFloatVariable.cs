using System;
using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPFloatVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public float Value;

        public string GetVariableTypeName()
        {
            return "Float";
        }

        public object GetDefaultValue()
        {
            return 0.0f;
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
            Value = (float)new_value;
        }
    }
}

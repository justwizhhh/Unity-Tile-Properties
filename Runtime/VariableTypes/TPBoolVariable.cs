using System;
using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPBoolVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public bool Value;

        public string GetVariableTypeName()
        {
            return "Boolean";
        }

        public object GetDefaultValue()
        {
            return false;
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
            Value = (bool)new_value;
        }
    }
}

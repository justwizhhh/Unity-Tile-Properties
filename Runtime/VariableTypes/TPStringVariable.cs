using System;
using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPStringVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public string Value;

        public string GetVariableTypeName()
        {
            return "String";
        }

        public object GetDefaultValue()
        {
            return "";
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
            Value = (string)new_value;
        }
    }
}

using System;
using UnityEngine;

namespace TileProperties
{
    public struct TPCharVariable : TPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public char Value;

        public string GetVariableTypeName()
        {
            return "Character";
        }

        public object GetDefaultValue()
        {
            return '\0';
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
            Value = Convert.ToChar(new_value);
        }
    }
}

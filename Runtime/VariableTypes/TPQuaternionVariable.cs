using System.Linq.Expressions;
using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPQuaternionVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public Quaternion Value;

        public string GetVariableTypeName()
        {
            return "Quaternion";
        }

        public object GetDefaultValue()
        {
            return new Quaternion(0, 0, 0, 0);
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
            Value = (Quaternion)new_value;
        }
    }
}

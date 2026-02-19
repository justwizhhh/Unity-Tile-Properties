using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    internal struct TPVector3IntVariable : ITPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public Vector3Int Value;

        public string GetVariableTypeName()
        {
            return "Vector3Int";
        }

        public object GetDefaultValue()
        {
            return new Vector3Int(0, 0);
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
            Value = (Vector3Int)new_value;
        }
    }
}

using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    public struct TPTextureVariable : TPVariableType
    {
        [SerializeField] public string VariableName;

        [SerializeField] public Texture Value;

        public string GetVariableTypeName()
        {
            return "Texture";
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
            Value = (Texture)new_value;
        }
    }
}

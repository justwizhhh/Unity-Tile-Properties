using UnityEngine;

[System.Serializable]
public struct TPColorVariable : TPVariableType
{
    [SerializeField] public string VariableName;

    [SerializeField] public Color Value;

    public string GetVariableTypeName()
    {
        return "Color";
    }

    public object GetDefaultValue()
    {
        return Color.black;
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
        Value = (Color)new_value;
    }
}

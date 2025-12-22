using UnityEngine;

public struct TPVector2IntVariable : TPVariableType
{
    [SerializeField] public string VariableName;

    [SerializeField] public Vector2Int Value;

    public string GetVariableTypeName()
    {
        return "Vector2Int";
    }

    public object GetDefaultValue()
    {
        return new Vector2Int(0, 0);
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
        Value = (Vector2Int)new_value;
    }
}

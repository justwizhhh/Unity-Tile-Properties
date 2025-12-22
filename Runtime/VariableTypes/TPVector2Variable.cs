using System;
using UnityEngine;

public struct TPVector2Variable : TPVariableType
{
    [SerializeField] public string VariableName;

    [SerializeField] public Vector2 Value;

    public string GetVariableTypeName()
    {
        return "Vector2";
    }

    public object GetDefaultValue()
    {
        return new Vector2(0, 0);
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
        Value = (Vector2)new_value;
    }
}

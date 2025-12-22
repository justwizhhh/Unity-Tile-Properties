using System;
using UnityEngine;

public struct TPVector3Variable : TPVariableType
{
    [SerializeField] public string VariableName;

    [SerializeField] public Vector3 Value;

    public string GetVariableTypeName()
    {
        return "Vector3";
    }

    public object GetDefaultValue()
    {
        return new Vector3(0, 0);
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
        Value = (Vector3)new_value;
    }
}

using System;
using UnityEngine;

[System.Serializable]
public struct TPBoolVariable : TPVariableType
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
        Value = Convert.ToBoolean(new_value);
    }
}

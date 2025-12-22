using System;
using UnityEngine;

[System.Serializable]
public struct TPIntVariable : TPVariableType
{
    [SerializeField] public string VariableName;

    [SerializeField] public int Value;

    public string GetVariableTypeName()
    {
        return "Integer";
    }

    public object GetDefaultValue()
    {
        return 0;
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
        Value = Convert.ToInt16(new_value);
    }
}

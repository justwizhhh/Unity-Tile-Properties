using UnityEngine;

[System.Serializable]
public struct TPParticlesVariable : TPVariableType
{
    [SerializeField] public string VariableName;

    [SerializeField] public ParticleSystem Value;

    public string GetVariableTypeName()
    {
        return "ParticleSystem";
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
        Value = (ParticleSystem)new_value;
    }
}
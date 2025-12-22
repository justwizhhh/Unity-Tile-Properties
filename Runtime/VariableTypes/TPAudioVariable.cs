using UnityEngine;

[System.Serializable]
public struct TPAudioVariable : TPVariableType
{
    [SerializeField] public string VariableName;

    [SerializeField] public AudioClip Value;

    public string GetVariableTypeName()
    {
        return "AudioClip";
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
        Value = (AudioClip)new_value;
    }
}

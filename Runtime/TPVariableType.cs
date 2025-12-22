namespace TileProperties
{
    public interface TPVariableType
    {
        // Editor information

        public string GetVariableTypeName();

        public object GetDefaultValue();

        // Variable information

        public string GetVariableName();
        public void SetVariableName(string new_name);

        public object GetValue();
        public void SetValue(object new_value);
    }
}

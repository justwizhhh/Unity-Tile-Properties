
namespace TileProperties
{
    /// <summary>
    /// A tile property variable which stores an object variable, as well as a display name for that variable, as well as the name of the type of the variable.
    /// </summary>
    public interface ITPVariableType
    {
        // -------------------------
        //
        // Editor information
        //
        // -------------------------
        public string GetVariableTypeName();

        public object GetDefaultValue();

        // -------------------------
        //
        // Variable information
        //
        // -------------------------
        public string GetVariableName();
        public void SetVariableName(string new_name);

        public object GetValue();
        public void SetValue(object new_value);
    }
}

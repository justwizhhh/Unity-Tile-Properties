
namespace TileProperties
{
    /// <summary>
    /// A tile property variable which stores an object variable, as well as a display name for that variable, as well as the name of the type of the variable. <br/><br/>
    /// This is used by the plugin to convert from variables stored in property list ScriptableObjects, into generic variable types the rest of the code uses. Objects of type 'TPVariableType' do NOT need to be used by any other script in the game! 
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

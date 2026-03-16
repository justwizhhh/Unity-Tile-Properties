using UnityEngine;

namespace TileProperties
{
    /// <summary>
    /// A struct that stores info about how a Tile Property List is viewed while a 'TilePropertySettings' object is open in the editor.
    /// </summary>

    [System.Serializable]
    public struct TilePropertySettingsEntry
    {
        /// <summary> Is this property list's tiles visible in the editor? </summary>
        public bool IsVisible;

        /// <summary> The property list object reference in the game. </summary>
        public TilePropertiesList PropertyList;
        /// <summary> The texture icon that will be drawn as a gizmo in the scene view for this property list. </summary>
        public Sprite DebugIcon;
        /// <summary> The colour overlay of the texture icon that will be drawn in the scene view. </summary>
        public Color DebugColor;
    }
}

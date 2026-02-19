using UnityEngine;

namespace TileProperties
{
    [System.Serializable]
    public struct TilePropertySettingsEntry
    {
        public bool IsVisible;
        
        public TilePropertiesList PropertyList;
        public Sprite DebugIcon;
        public Color DebugColor;
    }
}

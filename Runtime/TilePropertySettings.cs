using System.Collections.Generic;
using UnityEngine;

namespace justWiz.TileProperties
{
    /// <summary>
    /// Scriptable object for changing how Tile Property Lists are previewed in the editor. <br/><br/>
    /// Individual Tile Properties can be viewed in the scene view using gizmos that this class can enable and disable for each individual property list.
    /// </summary>

    [CreateAssetMenu(fileName = "NewTilePropertySettings", menuName = "2D/Tile Property Settings", order = 5)]
    [System.Serializable]
    public class TilePropertySettings : ScriptableObject
    {
        public List<TilePropertySettingsEntry> SettingsEntries = new List<TilePropertySettingsEntry>();
    }
}

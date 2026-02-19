using System.Collections.Generic;
using UnityEngine;

namespace TileProperties
{
    [CreateAssetMenu(fileName = "NewTilePropertySettings", menuName = "2D/Tile Property Settings", order = 5)]
    [System.Serializable]
    public class TilePropertySettings : ScriptableObject
    {
        public List<TilePropertySettingsEntry> SettingsEntries = new List<TilePropertySettingsEntry>();
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileProperties
{
    /// <summary>
    /// A ScriptableObject which stores a list of tile property values (<see cref="ITPVariableType"/>), 
    /// as well as a list of tile object references that are affected by these properties at runtime (<see cref="TileBase"/>).
    /// </summary>
    [CreateAssetMenu(fileName = "NewTilePropertyList", menuName = "2D/Tile Property List", order = 4)]
    [System.Serializable]
    public class TilePropertiesList : ScriptableObject
    {
        public List<TileBase> AffectedTiles = new List<TileBase>();

        [SerializeReference]
        private List<ITPVariableType> tileProperties;
        public List<ITPVariableType> TileProperties
        {
            get
            {
                if (tileProperties == null)
                {
                    tileProperties = new List<ITPVariableType>();
                }
                return tileProperties;
            }
            set { }
        }
    }
}

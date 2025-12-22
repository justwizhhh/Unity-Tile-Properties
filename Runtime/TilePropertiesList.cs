using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTilePropertyList", menuName = "2D/Tile Property List", order = 4)]
[System.Serializable]
public class TilePropertiesList : ScriptableObject
{
    public List<TileBase> AffectedTiles = new List<TileBase>();

    [SerializeReference]
    private List<TPVariableType> tileProperties;
    public List<TPVariableType> TileProperties
    {
        get 
        {
            if (tileProperties == null)
            {
                tileProperties = new List<TPVariableType>();
            }
            return tileProperties;
        }
        set
        {

        }
    }
}

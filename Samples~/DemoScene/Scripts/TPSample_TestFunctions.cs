using System.Collections.Generic;
using TileProperties;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileProperties
{
    public class TPSample_TestFunctions : MonoBehaviour
    {
        [Header("Button Test Parameters")]
        public Tilemap FloorTilemap;

        [Space(10)]
        public TileBase Test1_AffectedTile;
        public string Test1_AffectedTileProperty;
        public float Test1_NewTilePropertyValue;

        [Space(10)]
        public TileBase Test2_AffectedTile;
        public string Test2_AffectedTileProperty;
        public float Test2_NewTilePropertyValue;

        [Space(10)]
        public string Test3_PropertyList;
        public TileBase Test3_NewTile;

        [Space(10)]
        public string Test4_PropertyList;
        public TileBase Test4_RemovedTile;

        [Space(10)]
        public List<string> Test5_WipedPropertyListNames = new List<string>();

        private TilePropertiesManager current_tile_properties;

        private void Start()
        {
            current_tile_properties = FindFirstObjectByType<TilePropertiesManager>();
        }

        private void Awake()
        {

        }

        public void Test1_ChangeTileProperty()
        {
            if (current_tile_properties != null && current_tile_properties.PropertyListsLoaded)
            {
                current_tile_properties.SetTileProperty(
                    Test1_AffectedTile,
                    Test1_AffectedTileProperty,
                    Test1_NewTilePropertyValue, true, true);
            }
        }

        public void Test2_ChangeTileProperty()
        {
            if (current_tile_properties != null && current_tile_properties.PropertyListsLoaded)
            {
                current_tile_properties.SetTileProperty(
                    Test2_AffectedTile,
                    Test2_AffectedTileProperty,
                    Test2_NewTilePropertyValue, true, true);
            }
        }

        public void Test3_AddAffectedTile()
        {
            if (current_tile_properties != null && current_tile_properties.PropertyListsLoaded)
            {
                current_tile_properties.AddAffectedTile(Test3_NewTile, Test3_PropertyList);
            }
        }

        public void Test4_RemoveAffectedTile()
        {
            if (current_tile_properties != null && current_tile_properties.PropertyListsLoaded)
            {
                current_tile_properties.RemoveAffectedTile(Test4_RemovedTile, Test4_PropertyList);
            }
        }

        public void Test5_WipeAllTileProperties()
        {
            if (current_tile_properties != null && current_tile_properties.PropertyListsLoaded)
            {
                foreach (string list_name in Test5_WipedPropertyListNames)
                {
                    current_tile_properties.ClearTileProperties(list_name);
                }
            }
        }
    }
}

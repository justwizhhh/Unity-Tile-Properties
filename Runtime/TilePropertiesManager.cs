using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

namespace TileProperties
{
    public class TilePropertiesManager : MonoBehaviour
    {
        private List<TilePropertiesList> tile_properties_lists = new List<TilePropertiesList>();

        private void Awake()
        {
            GetAllTilePropertiesLists();
        }


        public bool DoesTileExistWithProperty(TileBase tile, ref TilePropertiesList included_list)
        {
            if (tile_properties_lists.Count != 0)
            {
                included_list = tile_properties_lists.Find(x => x.AffectedTiles.Contains(tile));
                return (included_list != null);
            }
            else
            {
                Debug.LogError("No tile property lists can be found! Have you assigned your lists with the correct group/label?");
                included_list = null;
                return false;
            }
        }

        public object GetTileProperty(TileBase tile, string property_name)
        {
            TilePropertiesList included_list = null;
            DoesTileExistWithProperty(tile, ref included_list);
            if (included_list != null)
            {
                TPVariableType property = included_list.TileProperties.Find(x => x.GetVariableName() == property_name);
                if (property != null)
                {
                    return property.GetValue();
                }
                else
                {
                    Debug.LogError("Property of name" + property_name + "' does not exist in this list!");
                }
            }
            else
            {
                Debug.LogError("'" + tile.name + "' cannot be found in any tile property list!");
            }

            return null;
        }

        public void SetTileProperty(TileBase tile, string property_name, object new_value)
        {
            if (tile_properties_lists.Count != 0)
            {
                TilePropertiesList included_list = tile_properties_lists.Find(x => x.AffectedTiles.Contains(tile));
                if (included_list != null)
                {
                    TPVariableType property = included_list.TileProperties.Find(x => x.GetVariableName() == property_name);
                    if (property != null)
                    {
                        property.SetValue(new_value);
                    }
                }
            }
        }

        public void ClearTileProperties(TileBase tile)
        {
            TilePropertiesList included_list = null;
            DoesTileExistWithProperty(tile, ref included_list);
            if (included_list != null)
            {
                included_list.TileProperties.Clear();
            }
        }

        private void GetAllTilePropertiesLists()
        {
            Addressables.LoadAssetsAsync<TilePropertiesList>("TilePropertiesList").Completed += OnLoadTilePropertiesLists;
        }

        private void OnLoadTilePropertiesLists(
            AsyncOperationHandle<IList<TilePropertiesList>> handle)
        {
            // Store a copy of each property list to allow for runtime editing 
            foreach (var list in handle.Result.ToList())
            {
                var new_list = Instantiate(list);
                tile_properties_lists.Add(new_list);
            }

            //tile_properties_lists = handle.Result.ToList();
        }
    }
}
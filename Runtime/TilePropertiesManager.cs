using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Tilemaps;

namespace TileProperties
{
    /// <summary>
    ///   Singleton object that keeps track of every tile property list in the game and passes property info to other objects. 
    /// </summary>
    /// <remarks>
    ///   This is where most tile-property-related activity happens during runtime, and where you can find essential getters and setters to access Tile Property Lists. This can be given its own object or attached to an already-existing tilemap object in the scene.
    /// </remarks>
    public class TilePropertiesManager : MonoBehaviour
    {
        // ------------------------------------
        //
        // This component is attached to any Tilemap object in the scene, and is used for storing instances of
        // all current Tile Property Lists in the game
        //
        // ------------------------------------

        public static TilePropertiesManager Instance { get; private set; }

        /// <summary> Event callback for when all tile property lists have finished being loaded into the scene. </summary>
        public event Action OnPropertyListsLoad;
        /// <summary> Enabled once all tile property lists have been loaded into scene. </summary>
        /// <remarks> Tile properties have to be loaded in from a list of addressable objects, so do NOT try to access tile property values until this is set to 'true'!</remarks>
        public bool PropertyListsLoaded { get; private set; }

        private readonly List<TilePropertiesList> tile_properties_lists = new();

        private void Awake()
        {
            Instance = this;

            GetAllTilePropertiesLists();
        }

        // ------------------------------------
        //
        // Public functions
        //
        // ------------------------------------

        // ====================================
        // Bool utility functions
        // ====================================

        /// <summary>
        ///   Takes a reference to a tile, and checks if it has been assigned to any tile property list.<br/><br/>
        ///   See also: <seealso cref="DoesTilePropertyListExist"/>
        /// </summary>
        /// <param name="tile"> The tile object reference to check for. </param>
        /// <param name="included_list"> The property list that the tile has been assigned to, if any are found. </param>
        /// <param name="throw_on_property_find"> Should this throw an error log if no associated list can be found for this tile? </param>
        public bool DoesTileExistWithProperty(TileBase tile, ref TilePropertiesList included_list, bool throw_on_property_find = true)
        {
            if (tile_properties_lists.Count != 0)
            {
                included_list = tile_properties_lists.Find(x => x.AffectedTiles.Find(y => y.name == tile.name));
                if (included_list == null && throw_on_property_find)
                {
                    Debug.LogError("'" + tile.name + "' cannot be found in any tile property list!");
                }
                return (included_list != null);
            }
            else
            {
                Debug.LogError("No tile property lists can be found! Have you assigned/built your lists with the correct group/label?");
                included_list = null;
                return false;
            }
        }

        /// <summary>
        ///   Takes a reference to a name of a tile property list, and checks if it exists in the project somewhere.<br/><br/>
        ///   See also: <seealso cref="DoesTileExistWithProperty"/>
        /// </summary>
        /// <param name="list_name"> The name of the property list to check for. </param>
        /// <param name="included_list"> The property list that the tile has been assigned to, if any are found. </param>
        /// <param name="throw_on_list_find"> Should this throw an error log if no list can be found of this name? </param>
        public bool DoesTilePropertyListExist(string list_name, ref TilePropertiesList included_list, bool throw_on_list_find = true)
        {
            if (tile_properties_lists.Count != 0)
            {
                included_list = tile_properties_lists.Find(x => x.name.Contains(list_name));
                if (included_list == null && throw_on_list_find)
                {
                    Debug.LogError("Tile property list of name '" + list_name + "' cannot be found in the project!");
                }
                return (included_list != null);
            }
            else
            {
                Debug.LogError("No tile property lists can be found! Have you assigned your lists with the correct group/label?");
                included_list = null;
                return false;
            }
        }

        // ====================================
        // 'AffectedTiles' access functions
        // ====================================

        /// <summary>
        ///   Returns a 'TileBase' object reference to a tile that is affected by a specific tile property list. <br/><br/>
        ///   See also: <seealso cref="GetAllAffectedTiles"/>
        /// </summary>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        /// <param name="index"> The list index for the requested tile. </param>
        public TileBase GetAffectedTile(string property_list_name, int index)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, true);
            if (included_list != null)
            {
                return included_list.AffectedTiles[index];
            }

            return null;
        }

        /// <summary>
        ///   Returns a list of all 'TileBase' objects affected by a specific tile property list.
        /// </summary>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        public List<TileBase> GetAllAffectedTiles(string property_list_name)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, true);
            if (included_list != null)
            {
                return included_list.AffectedTiles;
            }

            return null;
        }

        /// <summary>
        ///   Sets the value of a specific 'TileBase' object reference in the list of affected tiles for a specific tile property list.
        /// </summary>
        /// <param name="tile"> The soon-to-be replaced tile object reference from a Tilemap. </param>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        /// <param name="index"> The list index for the replaced tile. </param>
        public void SetAffectedTile(TileBase tile, string property_list_name, int index)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, true);
            if (included_list != null)
            {
                included_list.AffectedTiles[index] = tile;
            }
        }

        /// <summary>
        ///   Add a new 'TileBase' object reference to the list of affected tiles for a specific tile property list.
        /// </summary>
        /// <param name="new_tile"> A soon-to-be added tile object reference from a Tilemap. </param>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        public void AddAffectedTile(TileBase new_tile, string property_list_name)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, true);
            if (included_list != null)
            {
                included_list.AffectedTiles.Add(new_tile);
            }
        }

        /// <summary>
        ///   Removes a reference to a 'TileBase' object, with the same name as 'new_tile', in a specific tile property list.<br/><br/>
        ///   See also: <seealso cref="ClearAllAffectedTiles"/>
        /// </summary>
        /// <param name="old_tile"> A soon-to-be removed tile object reference from a Tilemap. </param>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        public void RemoveAffectedTile(TileBase old_tile, string property_list_name)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, true);
            if (included_list != null)
            {
                included_list.AffectedTiles.Remove(included_list.AffectedTiles.Find(x => x.name == old_tile.name));
            }
        }

        /// <summary>
        ///   Removes a reference to a 'TileBase' object, at position 'index', in a specific tile property list.<br/><br/>
        ///   See also: <seealso cref="ClearAllAffectedTiles"/>
        /// </summary>
        /// <param name="index"> The list index for the removed tile. </param>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        public void RemoveAffectedTile(int index, string property_list_name)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, true);
            if (included_list != null)
            {
                included_list.AffectedTiles.Remove(included_list.AffectedTiles[index]);
            }
        }

        /// <summary>
        ///   Clears all references to 'TileBase' objects inside of a specific tile property list.
        /// </summary>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        public void ClearAllAffectedTiles(string property_list_name)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, true);
            if (included_list != null)
            {
                included_list.AffectedTiles.Clear();
            }
        }

        // ====================================
        // 'TileProperties' access functions
        // ====================================

        /// <summary>
        ///   Returns the generic unboxed value of a tile property variable, based on a reference to a tile object.
        /// </summary>
        /// <param name="tile"> The tile object reference from a Tilemap. </param>
        /// <param name="property_name"> The name of the property to search the property list(s) for. </param>
        /// <param name="property_type"> The variable type of the property to search the property list(s) for. </param>
        /// <param name="throw_on_tile_find"> Should this throw an error log if no tile can be found with this property? </param>
        /// <param name="throw_on_property_find"> Should this throw an error log if no property can be found in the list attached to this tile? </param>
        public object GetTileProperty(TileBase tile, string property_name, Type property_type,
            bool throw_on_tile_find = false, bool throw_on_property_find = false)
        {
            TilePropertiesList included_list = null;
            DoesTileExistWithProperty(tile, ref included_list, throw_on_tile_find);
            if (included_list != null)
            {
                return _getTileProperty(included_list, property_name, property_type, throw_on_property_find);
            }

            if (property_type.IsValueType)
            {
                return Activator.CreateInstance(property_type);
            }
            return null;
        }

        /// <summary>
        ///   Returns the generic unboxed value of a tile property variable, based on a reference to a tile property list's name.
        /// </summary>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        /// <param name="property_name"> The name of the property to search the current property list for. </param>
        /// <param name="property_type"> The variable type of the property to search the property list(s) for. </param>
        /// <param name="throw_on_list_find"> Should this throw an error log if no list can be found with this name? </param>
        /// <param name="throw_on_property_find"> Should this throw an error log if no property can be found in the list attached to this tile? </param>
        public object GetTileProperty(string property_list_name, string property_name, Type property_type,
            bool throw_on_list_find = false, bool throw_on_property_find = false)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, throw_on_list_find);
            if (included_list != null)
            {
                return _getTileProperty(included_list, property_name, property_type, throw_on_property_find);
            }

            if (property_type.IsValueType)
            {
                return Activator.CreateInstance(property_type);
            }
            return null;
        }

        /// <summary>
        ///   Sets the value of a tile property variable, based on a reference to a tile object.
        /// </summary>
        /// <param name="tile"> The tile object reference from a Tilemap. </param>
        /// <param name="property_name"> The name of the property to search the property list(s) for. </param>
        /// <param name="new_value"> The new object value to set for this property in the list. </param>
        /// <param name="throw_on_tile_find"> Should this throw an error log if no tile can be found with this property? </param>
        /// <param name="throw_on_property_find"> Should this throw an error log if no property can be found in the list attached to this tile? </param>
        public void SetTileProperty(TileBase tile, string property_name, object new_value,
            bool throw_on_tile_find = false, bool throw_on_property_find = false)
        {
            TilePropertiesList included_list = null;
            DoesTileExistWithProperty(tile, ref included_list, throw_on_tile_find);
            if (included_list != null)
            {
                _setTileProperty(included_list, property_name, new_value, throw_on_property_find);
            }
        }

        /// <summary>
        ///   Sets the value of a tile property variable, based on a reference to a tile property list's name.
        /// </summary>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        /// <param name="property_name"> The name of the property to search the property list(s) for. </param>
        /// <param name="new_value"> The new object value to set for this property in the list. </param>
        /// <param name="throw_on_list_find"> Should this throw an error log if no list can be found with this name? </param>
        /// <param name="throw_on_property_find"> Should this throw an error log if no property can be found in the list attached to this tile? </param>
        public void SetTileProperty(string property_list_name, string property_name, object new_value,
            bool throw_on_list_find = false, bool throw_on_property_find = false)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, throw_on_list_find);
            if (included_list != null)
            {
                _setTileProperty(included_list, property_name, new_value, throw_on_property_find);
            }
        }

        /// <summary>
        ///   Add a new tile property variable to a specific tile property list.
        /// </summary>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        /// <param name="new_property_name"> The string name of the soon-to-be added tile property. </param>
        /// <param name="new_property_value"> The object value that will be stored in the soon-to-be added property. </param>
        /// <param name="throw_on_list_find"> Should this throw an error log if no list can be found with this name? </param>
        public void AddTileProperty(string property_list_name,
            string new_property_name, object new_property_value,
            bool throw_on_list_find = false)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, throw_on_list_find);
            if (included_list != null)
            {
                ITPVariableType property = included_list.TileProperties.Find(x => x.GetVariableName() == new_property_name);
                if (property == null)
                {
                    Type variable_type = TPVariableTypeRegistry.GetTPVariableType(new_property_value.GetType());
                    ITPVariableType new_property = (ITPVariableType)Activator.CreateInstance(variable_type);

                    new_property.SetVariableName(new_property_name);
                    new_property.SetValue(new_property_value);

                    included_list.TileProperties.Add(new_property);
                }
                else
                {
                    Debug.LogError("Property of name '" + new_property_name + "' already exists this list!");
                }
            }
        }

        /// <summary>
        ///   Removes a tile property from a specific tile object.<br/><br/>
        ///   See also: <seealso cref="ClearTileProperties"/>
        /// </summary>
        /// <param name="tile"> The tile object reference from a Tilemap. </param>
        /// <param name="property_name"> The name of the soon-to-be removed tile property. </param>
        /// <param name="throw_on_tile_find"> Should this throw an error log if no tile can be found with this property? </param>
        /// <param name="throw_on_property_find"> Should this throw an error log if no property can be found in the list attached to this tile? </param>
        public void RemoveTileProperty(TileBase tile, string property_name, 
            bool throw_on_tile_find = false, bool throw_on_property_find = false)
        {
            TilePropertiesList included_list = null;
            DoesTileExistWithProperty(tile, ref included_list, throw_on_tile_find);
            if (included_list != null)
            {
                _removeTileProperty(included_list, property_name, throw_on_property_find);
            }
        }

        /// <summary>
        ///   Removes a tile property from a specific tile property list reference.<br/><br/>
        ///   See also: <seealso cref="ClearTileProperties"/>
        /// </summary>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        /// <param name="property_name"> The string name of the soon-to-be removed tile property. </param>
        /// <param name="throw_on_list_find"> Should this throw an error log if no list can be found with this name? </param>
        /// <param name="throw_on_property_find"> Should this throw an error log if no property can be found in the list attached to this tile? </param>
        public void RemoveTileProperty(string property_list_name, string property_name, 
            bool throw_on_list_find = false, bool throw_on_property_find = false)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, throw_on_list_find);
            if (included_list != null)
            {
                _removeTileProperty(included_list, property_name, throw_on_property_find);
            }
        }

        /// <summary>
        ///   Takes a reference for a tile object, and removes all tile properties assigned to it.
        /// </summary>
        /// <param name="tile"> The tile object reference from a Tilemap. </param>
        public void ClearTileProperties(TileBase tile)
        {
            TilePropertiesList included_list = null;
            DoesTileExistWithProperty(tile, ref included_list, true);
            if (included_list != null)
            {
                included_list.TileProperties.Clear();
            }
        }

        /// <summary>
        ///   Takes a reference for a tile property list's name, and removes all properties assigned to that list.
        /// </summary>
        /// <param name="property_list_name"> The name of a 'TilePropertiesList' ScriptableObject in the Editor. </param>
        public void ClearTileProperties(string property_list_name)
        {
            TilePropertiesList included_list = null;
            DoesTilePropertyListExist(property_list_name, ref included_list, true);
            if (included_list != null)
            {
                included_list.TileProperties.Clear();
            }
        }

        // ------------------------------------
        //
        // Private utility functions
        //
        // ------------------------------------

        // ====================================
        // 'TileProperties' modifying functions
        // ====================================

        private object _getTileProperty(TilePropertiesList included_list, string property_name, Type property_type,
            bool throw_on_property_find)
        {
            ITPVariableType property = included_list.TileProperties.Find(x => x.GetVariableName() == property_name);
            if (property != null)
            {
                return property.GetValue();
            }
            else
            {
                if (throw_on_property_find)
                {
                    Debug.LogError("Property of name '" + property_name + "' does not exist in this list!");
                }

                if (property_type.IsValueType)
                {
                    return Activator.CreateInstance(property_type);
                }
                return null;
            }
        }

        private void _setTileProperty(TilePropertiesList included_list, string property_name, object new_value,
            bool throw_on_property_find)
        {
            ITPVariableType property = included_list.TileProperties.Find(x => x.GetVariableName() == property_name);
            if (property != null)
            {
                property.SetValue(new_value);
            }
            else
            {
                if (throw_on_property_find)
                {
                    Debug.LogError("Property of name '" + property_name + "' does not exist in this list!");
                }
            }
        }

        private void _removeTileProperty(TilePropertiesList included_list, string property_name,
            bool throw_on_property_find)
        {
            ITPVariableType property = included_list.TileProperties.Find(x => x.GetVariableName() == property_name);
            if (property != null)
            {
                included_list.TileProperties.Remove(property);
            }
            else
            {
                if (throw_on_property_find)
                {
                    Debug.LogError("Property of name '" + property_name + "' does not exist in this list!");
                }
            }
        }

        // ====================================
        // Property list loading
        // ====================================

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
                var clone = Instantiate(list);
                clone.name = list.name + "(Clone)";
                tile_properties_lists.Add(clone);
            }

            // Invoke event once all tile property lists have been found in the game
            PropertyListsLoaded = true;
            OnPropertyListsLoad?.Invoke();
        }
    }
}

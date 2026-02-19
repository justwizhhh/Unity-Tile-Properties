using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace TileProperties
{
    internal class TPAddressableProcessor : AssetPostprocessor
    {
        // --------------------------------------------
        //
        // Utility class for using Tile Property List files with Addressables inside of the Unity editor
        //
        // --------------------------------------------

        public static string addressable_group_name = "Tile Properties";

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings != null)
            {
                // Automatically assign a Tile Properties List to an addressable group for other objects to find
                foreach (var asset_path in importedAssets)
                {
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(asset_path);
                    if (obj != null)
                    {
                        if (obj is TilePropertiesList)
                        {
                            AddToTPAddressableGroup(asset_path, obj.name, settings);
                            CreateSettingsFile(asset_path);

                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                // Check to see if any Tile Properties Lists have been renamed, so they can also be renamed in the addressable file list
                foreach (var asset_path in movedAssets)
                {
                    string guid = AssetDatabase.AssetPathToGUID(asset_path);
                    AddressableAssetEntry entry = settings.FindAssetEntry(guid);

                    if (entry != null)
                    {
                        string new_asset_name = System.IO.Path.GetFileNameWithoutExtension(asset_path);
                        entry.SetAddress(new_asset_name);
                    }
                }
            }
            else
            {
                Debug.LogError("Addressable settings could not be found!");
            }
        }

        private static void AddToTPAddressableGroup(string asset_path, string address, AddressableAssetSettings settings)
        {
            // Find (or create) group for tile properties
            AddressableAssetGroup group = settings.FindGroup(addressable_group_name);
            if (group == null)
            {
                group = settings.CreateGroup(addressable_group_name, false, false, true, null);
            }

            // Check if this list is already included
            bool exists_in_group = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(asset_path)) != null;
            if (exists_in_group)
            {
                return;
            }

            var list_entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(asset_path), group);
            list_entry.SetLabel("TilePropertiesList", true);
            list_entry.address = address;
        }

        private static void CreateSettingsFile(string asset_path)
        {
            // Check to see if the project contains any settings files. If not, create one automatically
            string[] settings_files = AssetDatabase.FindAssets("t:TilePropertySettings");
            if (settings_files.Length == 0)
            {
                string new_asset_path = asset_path.Remove(asset_path.LastIndexOf("/")) + "/NewTilePropertySettings.asset";
                
                TilePropertySettings new_settings_file = ScriptableObject.CreateInstance<TilePropertySettings>();
                AssetDatabase.CreateAsset(new_settings_file, new_asset_path);
            }
        }
    }
}

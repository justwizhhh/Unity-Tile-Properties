using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class TPAddressableProcessor : AssetPostprocessor
{
    // Automatically assign a Tile Properties List to an addressable group for other objects to find
    
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        foreach (var asset_path in importedAssets)
        {
            var obj = AssetDatabase.LoadAssetAtPath<Object>(asset_path);
            if (obj != null)
            {
                if (obj is TilePropertiesList)
                {
                    AddToTPAddressableGroup(asset_path, obj.name);
                }
            }
            else
            {
                continue;
            }
        }
    }

    private static void AddToTPAddressableGroup(string asset_path, string address)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
        if (settings != null)
        {
            // Find (or create) group for tile properties
            AddressableAssetGroup group = settings.FindGroup("Tile Properties");
            if (group == null)
            {
                group = settings.CreateGroup("Tile Properties", false, false, true, null);
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
        else
        {
            Debug.LogError("Addressable settings could not be found!");
        }
    }
}

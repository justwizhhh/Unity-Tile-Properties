using System;
using System.Collections.Generic;
using System.Linq;
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

        private static List<Tuple<string, string>> addressable_assignees = new List<Tuple<string, string>>();

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings != null)
            {
                // If this is a Tile Property List, mark it so that it can be added to an addressable group later
                foreach (var asset_path in importedAssets)
                {
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(asset_path);
                    if (obj != null)
                    {
                        if (obj is TilePropertiesList)
                        {
                            addressable_assignees.Add(Tuple.Create(asset_path, obj.name));
                            CreateSettingsFile(asset_path);
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

            if (addressable_assignees.Count > 0)
            {
                // Only apply lists to addressable group after everything else has (hopefully) been imported
                AssetDatabase.Refresh();
                EditorApplication.delayCall += AddToTPAddressableGroup;
            }
        }

        private static void AddToTPAddressableGroup()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings != null)
            {
                foreach (Tuple<string, string> tp_list in addressable_assignees)
                {
                    // Find (or create) group for tile properties
                    AddressableAssetGroup group = settings.FindGroup(addressable_group_name);
                    if (group == null)
                    {
                        group = settings.CreateGroup(addressable_group_name, false, false, true, null);
                    }

                    // Check if this list is already included
                    bool exists_in_group = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(tp_list.Item1)) != null;
                    if (exists_in_group)
                    {
                        return;
                    }

                    var list_entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(tp_list.Item1), group);
                    list_entry.SetLabel("TilePropertiesList", true);
                    list_entry.address = tp_list.Item2;
                }
            }

            addressable_assignees.Clear();
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

                AssetDatabase.Refresh();
            }
        }
    }
}

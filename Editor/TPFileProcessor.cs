using UnityEditor;
using UnityEngine;

namespace TileProperties
{
    internal class TPAddressableProcessor : AssetPostprocessor
    {
        // --------------------------------------------
        //
        // Utility class for creating and deleting Tile Property List files inside of the Unity editor
        //
        // --------------------------------------------

        public static string addressable_group_name = "Tile Properties";

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // If this is a Tile Property List, mark it so that it can be added to an addressable group later
            foreach (var asset_path in importedAssets)
            {
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(asset_path);
                if (obj != null)
                {
                    if (obj is TilePropertiesList)
                    {
                        CreateSettingsFile(asset_path);
                    }
                }
                else
                {
                    continue;
                }
            }
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

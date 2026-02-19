using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace TileProperties
{
    [InitializeOnLoad]
    internal static class TPSettingsDrawer
    {
        private static List<TilePropertySettings> settings_objects = new List<TilePropertySettings>();

        private static List<List<List<Vector3>>> affected_tiles_pos = new List<List<List<Vector3>>>();
        private static List<Vector3> already_rendered_tiles_pos = new List<Vector3>();

        private static bool gizmos_enabled = true;
        private static Texture2D default_texture_icon;

        static TPSettingsDrawer()
        {
            // Update preview gizmos when the editor is first loaded in,
            // as well as whenever the scene gets reloaded through the editor
            SceneView.duringSceneGui += OnSceneGUI;
            SceneView.beforeSceneGui += GetGizmosEnabled;

            EditorApplication.delayCall += OnEditorLoaded;
            EditorSceneManager.sceneOpened += OnEditorSceneManagerSceneOpened;
            EditorSceneManager.sceneSaved += OnEditorSceneManagerSceneSaved;
            EditorApplication.playModeStateChanged += OnExitPlayMode;

            AssemblyReloadEvents.afterAssemblyReload += UpdateTileObjectRefs;
        }

        private static void OnEditorLoaded()
        {
            UpdateTileObjectRefs();
            SceneView.RepaintAll();
        }

        private static void GetGizmosEnabled(SceneView sceneView)
        {
            // Only draw tile previews if the user has enabled gizmos visibility
            // NOTE: this code might not be super efficient, without an event callback for the editor Gizmos button being clicked...
            if (sceneView.drawGizmos != gizmos_enabled)
            {
                gizmos_enabled = sceneView.drawGizmos;
            }
        }

        private static void OnSceneGUI(SceneView sceneView) 
        {
            if (sceneView.camera)
            {
                DrawPreviewGizmos();
            }
        }

        private static void OnEditorSceneManagerSceneSaved(Scene scene)
        {
            UpdateTileObjectRefs();
            SceneView.RepaintAll();
        }

        private static void OnEditorSceneManagerSceneOpened(Scene scene, OpenSceneMode mode)
        {
            UpdateTileObjectRefs();
            SceneView.RepaintAll();
        }

        private static void OnExitPlayMode(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredEditMode || change == PlayModeStateChange.ExitingPlayMode)
            {
                UpdateTileObjectRefs();
                SceneView.RepaintAll();
            }
        }

        public static void UpdateTileObjectRefs()
        {
            // Clear the current cache of tile object references
            settings_objects.Clear();
            foreach (var entries in affected_tiles_pos)
            {
                foreach (var positions in entries)
                {
                    positions.Clear();
                }
                entries.Clear();
            }
            affected_tiles_pos.Clear();

            // Find all settings SO files in the project
            string[] settings_names = AssetDatabase.FindAssets("t:TilePropertySettings");
            foreach (string name in settings_names)
            {
                string path = AssetDatabase.GUIDToAssetPath(name);
                TilePropertySettings settings_obj = AssetDatabase.LoadAssetAtPath<TilePropertySettings>(path);
                settings_objects.Add(settings_obj);
            }

            // Add every affected tile for every settings entry for every settings file into a large list of tile positions
            for (int i = 0; i < settings_objects.Count; i++)
            {
                affected_tiles_pos.Add(new List<List<Vector3>>());
                List<Tilemap> all_tilemaps = Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None).ToList();

                foreach (Tilemap tilemap in all_tilemaps)
                {
                    foreach (var tile_pos in tilemap.cellBounds.allPositionsWithin)
                    {
                        TileData tile_data = new TileData();

                        if (tilemap.GetTile(tile_pos) != null)
                        {
                            tilemap.GetTile(tile_pos).GetTileData(tile_pos, tilemap, ref tile_data);
                            
                            for (int j = 0; j < settings_objects[i].SettingsEntries.Count; j++)
                            {
                                if (settings_objects[i].SettingsEntries[j].PropertyList != null)
                                {
                                    affected_tiles_pos[i].Add(new List<Vector3>());

                                    var list_entry = settings_objects[i].SettingsEntries[j];
                                    var affected_tiles = list_entry.PropertyList.AffectedTiles;

                                    for (int k = 0; k < affected_tiles.Count; k++)
                                    {
                                        Tile affected_tile = affected_tiles[k] as Tile;

                                        if (affected_tile == null)
                                            continue;

                                        if (affected_tile.sprite == tile_data.sprite)
                                        {
                                            Vector3 new_pos = tilemap.GetCellCenterWorld(tile_pos);
                                            if (!affected_tiles_pos[i][j].Contains(new_pos))
                                            {
                                                affected_tiles_pos[i][j].Add(new_pos);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void DrawPreviewGizmos()
        {
            if (gizmos_enabled)
            {
                Handles.BeginGUI();

                if (default_texture_icon == null)
                {
                    default_texture_icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
                        "Assets/Editor/Icons/TilePropertyPreview Icon.png");
                }

                // Render a gizmo icon for each and every tile position retrieved from "UpdateTileObjectRefs()"
                for (int i = 0; i < affected_tiles_pos.Count; i++)
                {
                    for (int j = 0; j < affected_tiles_pos[i].Count; j++)
                    {
                        affected_tiles_pos[i][j].Reverse();
                        if (j < settings_objects[i].SettingsEntries.Count)
                        {
                            var entry = settings_objects[i].SettingsEntries[j];
                            if (entry.IsVisible || entry.DebugColor.a == 0)
                            {
                                for (int k = 0; k < affected_tiles_pos[i][j].Count; k++)
                                {
                                    // Find the position of where to render the icon
                                    Vector3 tile = affected_tiles_pos[i][j][k];

                                    if (!already_rendered_tiles_pos.Contains(tile))
                                    {
                                        Vector2 tile_pos_min = tile - (Vector3.one / 2);
                                        Vector2 tile_pos_max = tile + (Vector3.one / 2);

                                        Vector2 screen_pos_min = HandleUtility.WorldToGUIPoint(tile_pos_min);
                                        Vector2 screen_pos_max = HandleUtility.WorldToGUIPoint(tile_pos_max);

                                        float width = Mathf.Abs(screen_pos_max.x - screen_pos_min.x);
                                        float height = Mathf.Abs(screen_pos_max.y - screen_pos_min.y);

                                        Rect tile_rect = new Rect(
                                            Mathf.Min(screen_pos_min.x, screen_pos_max.x),
                                            Mathf.Min(screen_pos_min.y, screen_pos_max.y),
                                            width,
                                            height);

                                        // Load in a custom colour if one has been assigned
                                        Color texture_color = entry.DebugColor;

                                        // Load in a custom texture if one has been assigned
                                        Sprite sprite = entry.DebugIcon;
                                        Texture2D texture_icon;
                                        if (sprite != null)
                                            texture_icon = sprite.texture;
                                        else
                                            texture_icon = default_texture_icon;

                                        GUI.DrawTexture(
                                            tile_rect, texture_icon, ScaleMode.ScaleToFit, true, 1f,
                                            texture_color, 0, 0);

                                        already_rendered_tiles_pos.Add(tile);
                                    }
                                }
                            }
                        }
                    }
                }

                already_rendered_tiles_pos.Clear();
                Handles.EndGUI();
            }
        }
    }
}


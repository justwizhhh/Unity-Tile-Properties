using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
namespace TileProperties
{
    [CustomEditor(typeof(TilePropertiesList))]
    public class TPListInspector : Editor
    {
        private TilePropertiesList list;
        private SerializedProperty serialized_list;

        private ReorderableList affected_tiles;
        private ReorderableList tile_properties;

        private int control_id;
        private bool tile_preview_active;
        private List<Vector3> affected_tile_objects = new List<Vector3>();

        private struct NewPropertyParams
        {
            public string Name;
            public string Path;
        }

        public void OnEnable()
        {
            list = (TilePropertiesList)target;

            serialized_list = serializedObject.FindProperty("tileProperties");

            // --------------------------------------------
            //
            // Create a default list for affected tiles
            //
            // --------------------------------------------

            affected_tiles = new ReorderableList(
                list.AffectedTiles,
                typeof(TileBase),
                true, true, true, true);

            affected_tiles.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Affected Tiles");
            };

            affected_tiles.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty tiles_list = serializedObject.FindProperty("AffectedTiles");
                SerializedProperty tile = tiles_list.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(rect, tile, GUIContent.none);
            };

            affected_tiles.onAddCallback = (ReorderableList list) =>
            {
                control_id = GUIUtility.GetControlID(FocusType.Keyboard);
                EditorGUIUtility.ShowObjectPicker<TileBase>(null, false, "t:TileBase", control_id);
            };

            // --------------------------------------------
            //
            // Create a customisable list for tile properties
            //
            // --------------------------------------------

            tile_properties = new ReorderableList(
                list.TileProperties,
                typeof(TPVariableType),
                true, true, true, true);

            tile_properties.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Tile Properties");
            };

            tile_properties.elementHeightCallback = (int index) =>
            {
                float padding = 6f;
                return (EditorGUIUtility.singleLineHeight * 2) + padding;
            };

            tile_properties.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                float lineHeight = EditorGUIUtility.singleLineHeight;

                // Draw property icon
                float icon_padding = (lineHeight * 2) + 5;
                GUI.DrawTexture(
                    new Rect(rect.x, rect.y + 4, lineHeight * 2, lineHeight * 2),
                    (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Gizmos/TileProperty Icon.png", typeof(Texture2D)),
                    ScaleMode.ScaleToFit);

                // Set up default values for how much space a tile property should take up in the list
                Rect label_rect = new Rect(
                    icon_padding + rect.x, rect.y,
                    rect.width / 4, lineHeight);
                Rect variable_name_rect = new Rect(
                    icon_padding + rect.x, rect.y + lineHeight,
                    rect.width / 4, lineHeight);
                Rect variable_value_rect = new Rect(
                    icon_padding + rect.x + (rect.width / 4) + 10, rect.y + lineHeight,
                    rect.width / 1.6f, lineHeight);

                SerializedProperty element = serialized_list.GetArrayElementAtIndex(index);
                var drawer = TPDrawerRegistry.GetDrawer(element);

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                drawer.Draw(
                    rect, label_rect, variable_name_rect, variable_value_rect,
                    element);
            };

            tile_properties.onAddDropdownCallback = (Rect button_rect, ReorderableList list) =>
            {
                var menu = new GenericMenu();
                var guids = AssetDatabase.FindAssets("", new[] { "Assets/Scripts/TileProperties/VariableTypes" });
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    string item_name = Path.GetFileNameWithoutExtension(path);

                    // Only show buttons for adding 'tile property' variables 
                    Assembly asm = typeof(TPVariableType).Assembly;
                    System.Type item_type = asm.GetType(item_name);
                    if (item_type == null
                        || item_type.GetInterface(nameof(ITPDrawer)) != null)
                    {
                        continue;
                    }

                    // Populate the drop-down menu with an button to add this new variable type
                    string item_display_name = item_type.ToString().Substring(2);
                    item_display_name.Replace("Variable", "");
                    menu.AddItem(new GUIContent(item_display_name),
                    false, clickHandler,
                    new NewPropertyParams() { Name = Path.GetFileNameWithoutExtension(path), Path = path });
                }

                menu.ShowAsContext();


            };

            tile_preview_active = false;
            SceneView.duringSceneGui += DrawPreviewGizmos;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DrawPreviewGizmos;
        }

        private void clickHandler(object click_target)
        {
            var data = (NewPropertyParams)click_target;
            var updated_list = list.TileProperties;

            // Assign default values to our new tile property variable
            Assembly asm = typeof(TPVariableType).Assembly;
            System.Type type = asm.GetType(data.Name);
            var new_list_entry = (TPVariableType)System.Activator.CreateInstance(type);
            new_list_entry.SetVariableName("New" + new_list_entry.GetVariableTypeName());
            new_list_entry.SetValue(new_list_entry.GetDefaultValue());

            // Add to the list
            updated_list.Add(new_list_entry);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        private void UpdateTileObjectRefs()
        {
            affected_tile_objects.Clear();
            List<Tilemap> tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None).ToList();

            foreach (Tilemap tilemap in tilemaps)
            {
                for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++)
                {
                    for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++)
                    {
                        for (int z = tilemap.cellBounds.min.z; z < tilemap.cellBounds.max.z; z++)
                        {
                            TileData tile_data = new TileData();

                            Vector3Int tile_pos = new Vector3Int(x, y, z);
                            if (tilemap.GetTile(tile_pos) != null)
                            {
                                tilemap.GetTile(tile_pos).GetTileData(tile_pos, tilemap, ref tile_data);
                                for (int i = 0; i < affected_tiles.list.Count; i++)
                                {
                                    TileBase affected_tile_base = affected_tiles.list[i] as TileBase;
                                    Tile affected_tile = affected_tile_base as Tile;

                                    if (affected_tile.sprite == tile_data.sprite)
                                    {
                                        Vector3 new_pos = tilemap.CellToWorld(tile_pos);
                                        if (!affected_tile_objects.Contains(new_pos))
                                        {
                                            affected_tile_objects.Add(new_pos);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawPreviewGizmos(SceneView scene_view)
        {
            if (tile_preview_active)
            {
                Handles.BeginGUI();

                Texture2D texture_icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Gizmos/TilePropertyPreview Icon.png");

                foreach (var tile in affected_tile_objects)
                {
                    Vector2 tile_pos_min = tile;
                    Vector2 tile_pos_max = tile + Vector3.one;

                    Vector2 screen_pos_min = HandleUtility.WorldToGUIPoint(tile_pos_min);
                    Vector2 screen_pos_max = HandleUtility.WorldToGUIPoint(tile_pos_max);

                    float width = Mathf.Abs(screen_pos_max.x - screen_pos_min.x);
                    float height = Mathf.Abs(screen_pos_max.y - screen_pos_min.y);

                    Rect tile_rect = new Rect(
                        Mathf.Min(screen_pos_min.x, screen_pos_max.x),
                        Mathf.Min(screen_pos_min.y, screen_pos_max.y),
                        width,
                        height);

                    GUI.DrawTexture(tile_rect, texture_icon, ScaleMode.ScaleToFit, true);
                }

                Handles.EndGUI();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Update affected tiles
            affected_tiles.DoLayoutList();

            var evt = Event.current;
            if (evt.type == EventType.ExecuteCommand)
            {
                if (evt.commandName == "ObjectSelectorClosed"
                    && EditorGUIUtility.GetObjectPickerControlID() == control_id)
                {
                    TileBase picked_tile = EditorGUIUtility.GetObjectPickerObject() as TileBase;
                    if (picked_tile != null)
                    {
                        list.AffectedTiles.Add(picked_tile);
                    }
                }
            }

            // Update tile properties
            tile_properties.DoLayoutList();

            // Update gizmo for affected tiles preview
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(
                (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Gizmos/TilePropertyPreviewToggle Icon.png", typeof(Texture2D)),
                GUILayout.Width(32), GUILayout.Height(32));
            EditorGUILayout.LabelField(
                "Display Affected Tiles in Scene View",
                GUILayout.Height(32));
            tile_preview_active = EditorGUILayout.Toggle(
                tile_preview_active,
                GUILayout.Height(32));
            EditorGUILayout.EndHorizontal();

            if (tile_preview_active)
            {
                UpdateTileObjectRefs();
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}
#endif
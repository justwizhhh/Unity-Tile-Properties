using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileProperties
{
    [CustomEditor(typeof(TilePropertiesList))]
    public class TPListInspector : Editor
    {
        // ------------------------------------
        //
        // Editor interface class for all Tile Property List objects in the project
        //
        // ------------------------------------

        private TilePropertiesList list;
        private SerializedProperty serialized_list;

        private ReorderableList affected_tiles;
        private ReorderableList tile_properties;

        private int control_id;

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
            // Create a default list for affected tile objects
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

            affected_tiles.elementHeightCallback = (int index) =>
            {
                float line_height = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;
                float padding = line_height * 0.25f;

                float icon_height = (line_height * 2);

                return icon_height + padding * spacing;
            };

            affected_tiles.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty tiles_list = serializedObject.FindProperty("AffectedTiles");
                SerializedProperty tile = tiles_list.GetArrayElementAtIndex(index);

                TileBase deserialized_tile = (TileBase)tile.objectReferenceValue;
                TileData tile_data = new TileData();
                deserialized_tile.GetTileData(Vector3Int.zero, null, ref tile_data);

                float line_height = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;
                float padding = line_height * 0.25f;

                // Draw icon to preview the tile reference's sprite
                Sprite sprite = tile_data.sprite;
                Rect tile_uv = new Rect(
                    sprite.textureRect.x / sprite.texture.width,
                    sprite.textureRect.y / sprite.texture.height,
                    sprite.textureRect.width / sprite.texture.width,
                    sprite.textureRect.height / sprite.texture.height);

                float aspect = sprite.textureRect.width / sprite.textureRect.height;
                float icon_height = line_height * 2;
                float icon_width = icon_height * aspect;

                GUI.DrawTextureWithTexCoords(
                    new Rect(rect.x + 4, rect.y + 4, icon_width, icon_height),
                    tile_data.sprite.texture,
                    tile_uv);

                // Draw property field for tile reference next to the preview icon
                float x = rect.x + (padding * 2) + icon_width;
                float width = rect.width - (padding * 2) - icon_width;

                Rect tile_rect = new Rect(
                    x, rect.y + spacing + (line_height * 0.5f),
                    width, (line_height * 1.25f) - spacing);

                EditorGUI.PropertyField(tile_rect, tile, GUIContent.none);
            };

            affected_tiles.onAddCallback = (ReorderableList list) =>
            {
                control_id = GUIUtility.GetControlID(FocusType.Keyboard);
                EditorGUIUtility.ShowObjectPicker<TileBase>(null, false, null, control_id);
            };

            affected_tiles.onChangedCallback += (ReorderableList list) =>
            {
                TPSettingsDrawer.UpdateTileObjectRefs();
            };

            // --------------------------------------------
            //
            // Create a customisable list for tile properties
            //
            // --------------------------------------------

            tile_properties = new ReorderableList(
                list.TileProperties,
                typeof(ITPVariableType),
                true, true, true, true);

            tile_properties.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Tile Properties");
            };

            tile_properties.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = serialized_list.GetArrayElementAtIndex(index);
                SerializedProperty value_property = element.FindPropertyRelative("Value");

                float line_height = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;
                float padding = line_height * 0.25f;

                float height = 
                    line_height + 
                    EditorGUI.GetPropertyHeight(value_property, true) +
                    padding * spacing;

                return height;
            };

            tile_properties.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = serialized_list.GetArrayElementAtIndex(index);
                SerializedProperty name_property = element.FindPropertyRelative("VariableName");
                SerializedProperty value_property = element.FindPropertyRelative("Value");

                float line_height = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;
                float padding = line_height * 0.25f;

                rect.y += padding;

                // Draw property icon
                float icon_size = line_height * 2;
                GUI.DrawTexture(
                    new Rect(rect.x + 4, rect.y + 4, icon_size, icon_size),
                    (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Icons/TileProperty Icon.png", typeof(Texture2D)),
                    ScaleMode.ScaleToFit);

                // Set up default values for how much space a tile property should take up in the list editor
                float property_x = rect.x + padding + icon_size;
                float property_width = rect.width - icon_size - spacing;

                float property_name_width = (property_width * 0.35f) - spacing;
                float property_value_width = (property_width * 0.65f) - (spacing * 2);
                float property_value_height = EditorGUI.GetPropertyHeight(value_property, true);

                Rect label_rect = new Rect(
                    property_x, rect.y,
                    property_width, line_height);
                Rect variable_name_rect = new Rect(
                    property_x, label_rect.yMax + spacing,
                    property_name_width, line_height);
                Rect variable_value_rect = new Rect(
                    variable_name_rect.xMax + (spacing * 2), variable_name_rect.y,
                    property_value_width, property_value_height);

                // Draw the property in the list
                EditorGUI.BeginChangeCheck();

                string label_name = element.type.Substring(19, element.type.Length - 20);
                label_name = string.Concat(label_name.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                EditorGUI.LabelField(label_rect, label_name);

                EditorGUI.PropertyField(variable_name_rect, name_property, GUIContent.none);
                EditorGUI.PropertyField(variable_value_rect, value_property, GUIContent.none, true);

                // After a property has been changed, scan the list for any duplicate property names in the list
                if (EditorGUI.EndChangeCheck())
                {
                    List<string> property_names = new List<string>();
                    for (int i = 0; i < serialized_list.arraySize; i++)
                    {
                        SerializedProperty property = serialized_list.GetArrayElementAtIndex(i);
                        SerializedProperty new_property_name = property.FindPropertyRelative("VariableName");
                        property_names.Add(new_property_name.stringValue);
                    }

                    if (property_names.Count != property_names.Distinct().Count())
                    {
                        Debug.LogWarning("Duplicate variable names detected in '" + list.name + "'! This could cause unexpected behaviour!");
                    }
                }
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
                    Assembly asm = typeof(ITPVariableType).Assembly;
                    System.Type item_type = asm.GetType("TileProperties." + item_name);
                    if (item_type == null)
                    {
                        continue;
                    }

                    // Populate the drop-down menu with an button to add this new variable type
                    string item_display_name = item_type.ToString().Substring(17);
                    item_display_name = item_display_name.Replace("Variable", "");
                    menu.AddItem(new GUIContent(item_display_name),
                        false, clickHandler,
                        new NewPropertyParams() { Name = Path.GetFileNameWithoutExtension(path), Path = path });
                }

                menu.ShowAsContext();
            };

            tile_properties.onChangedCallback += (ReorderableList list) =>
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);

                TPSettingsDrawer.UpdateTileObjectRefs();
            };
        }

        private void clickHandler(object click_target)
        {
            var data = (NewPropertyParams)click_target;
            var updated_list = list.TileProperties;

            // Assign default values to our new tile property variable
            Assembly asm = typeof(ITPVariableType).Assembly;
            System.Type type = asm.GetType("TileProperties." + data.Name);
            var new_list_entry = (ITPVariableType)System.Activator.CreateInstance(type);
            new_list_entry.SetVariableName("New" + new_list_entry.GetVariableTypeName());
            new_list_entry.SetValue(new_list_entry.GetDefaultValue());

            // Add new property variable to the list
            updated_list.Add(new_list_entry);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        // --------------------------------------------
        //
        // Apply final interface to Unity inspector
        //
        // --------------------------------------------

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Update affected tiles
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.LabelField("This list stores all of the tile object references that will be affected by the property data below.");
            GUILayout.Space(4);
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
                        // Discard tile if property already exists
                        if (list.AffectedTiles.Find(x => x.name == picked_tile.name))
                        {
                            Debug.LogWarning("Tile named '" +  picked_tile.name + "' already exists in this list!");
                        }

                        // Discard tile if it is not a generic "TileBase" object
                        else if (picked_tile is AutoTile)
                        {
                            Debug.LogWarning("AutoTiles are not supported inside a Tile Property List! Please only use generic TileBase object references inside 'Affected Tiles'.");
                        }
                        else if (picked_tile is RuleTile 
                            || picked_tile is HexagonalRuleTile
                            || picked_tile is IsometricRuleTile
                            || picked_tile is RuleOverrideTile
                            || picked_tile is AdvancedRuleOverrideTile)
                        {
                            Debug.LogWarning("Rule Tiles are not supported inside a Tile Property List! Please only use generic TileBase object references inside 'Affected Tiles'.");
                        }
                        else
                        {
                            list.AffectedTiles.Add(picked_tile);

                            TPSettingsDrawer.UpdateTileObjectRefs();
                        }
                    }
                }
            }

            // Update tile properties
            EditorGUILayout.LabelField("This list stores all of the property data that will be attached to the tile objects above during gameplay.");
            GUILayout.Space(4);
            tile_properties.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}

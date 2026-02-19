using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditorInternal;
using UnityEngine;

namespace TileProperties
{
    [CustomEditor(typeof(TilePropertySettings))]
    public class TPSettingsInspector : Editor
    {
        // ------------------------------------
        //
        // Editor interface class for all Tile Property Settings objects in the project
        //
        // ------------------------------------

        private SerializedProperty settings_prop;

        private ReorderableList settings_entries;

        private static Texture2D enable_button_icon;
        private static Texture2D disable_button_icon;
        private static Texture2D build_button_icon;

        private void OnEnable()
        {
            settings_prop = serializedObject.FindProperty("SettingsEntries");

            settings_entries = new ReorderableList(
                serializedObject,
                settings_prop,
                true, true, true, true);

            settings_entries.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Tile Property Previews");
            };

            // --------------------------------------------
            //
            // Create a customisable list of all tile property lists to display as gizmos
            //
            // --------------------------------------------

            settings_entries.elementHeightCallback = (int index) =>
            {
                float line_height = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;
                float padding = line_height * 0.25f;

                float icon_height = (line_height * 2);

                return icon_height + padding * spacing;
            };

            settings_entries.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty entry = settings_prop.GetArrayElementAtIndex(index);

                SerializedProperty entry_is_visible = entry.FindPropertyRelative("IsVisible");

                SerializedProperty entry_list = entry.FindPropertyRelative("PropertyList");
                SerializedProperty entry_icon = entry.FindPropertyRelative("DebugIcon");
                SerializedProperty entry_color = entry.FindPropertyRelative("DebugColor");

                float line_height = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;
                float padding = line_height * 0.25f;

                rect.y += padding;

                // Draw visibility toggle field
                Rect visible_rect = new Rect(
                    rect.x, rect.y + spacing + (line_height * 0.5f),
                    (EditorGUI.GetPropertyHeight(entry_is_visible, true) / 2) + padding, line_height);

                EditorGUI.PropertyField(visible_rect, entry_is_visible, GUIContent.none);

                // Draw property icon
                float icon_size = line_height * 2;
                GUI.DrawTexture(
                    new Rect(visible_rect.xMax + 6, rect.y + 2, icon_size, icon_size),
                    (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Editor/Icons/TilePropertiesList Icon.png", typeof(Texture2D)),
                    ScaleMode.ScaleToFit);

                // Draw all other properties for settings entry
                float list_x = visible_rect.xMax + (padding * 2) + icon_size;

                float width = rect.width - visible_rect.width - (padding * 2) - icon_size - spacing;
                float list_width = (width * 0.55f) - spacing - padding;
                float property_width = (width * 0.45f) - spacing;

                Rect list_rect = new Rect(
                    list_x, rect.y + spacing + (line_height * 0.5f),
                    list_width, line_height);
                Rect icon_name_rect = new Rect(
                    list_rect.xMax + padding, rect.y,
                    property_width, line_height);
                Rect color_value_rect = new Rect(
                    list_rect.xMax + padding, icon_name_rect.yMax + spacing,
                    property_width, line_height);

                EditorGUI.BeginChangeCheck();

                EditorGUI.PropertyField(list_rect, entry_list, GUIContent.none);
                EditorGUI.PropertyField(icon_name_rect, entry_icon, GUIContent.none);
                EditorGUI.PropertyField(color_value_rect, entry_color, GUIContent.none);

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);

                    // Refresh tile drawing list in order to not render tiles from old cached property lists
                    TPSettingsDrawer.UpdateTileObjectRefs();
                }
            };

            settings_entries.onAddCallback = (ReorderableList list) =>
            {
                // Apply default values to the new settings entry in the list
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize += 1;

                SerializedProperty new_entry = list.serializedProperty.GetArrayElementAtIndex(index);

                serializedObject.ApplyModifiedProperties();

                new_entry.FindPropertyRelative("IsVisible").boolValue = true;
                new_entry.FindPropertyRelative("DebugColor").colorValue = Color.white;

                serializedObject.ApplyModifiedProperties();
            };

            settings_entries.onChangedCallback += (ReorderableList list) =>
            {
                serializedObject.ApplyModifiedProperties();

                TPSettingsDrawer.UpdateTileObjectRefs();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Preview icons for tile properties are rendered in list order (highest is rendered with top priority over everything else).");
            settings_entries.DoLayoutList();

            // Enable/disable toggle buttons
            if (enable_button_icon == null)
            {
                enable_button_icon = (Texture2D)AssetDatabase.LoadAssetAtPath(
                    "Assets/Editor/Icons/TilePropertyEnablePreview Icon.png", typeof(Texture2D));
            }
            GUIContent enable_button = new GUIContent("Enable All Previews", enable_button_icon);
            if (disable_button_icon == null)
            {
                disable_button_icon = (Texture2D)AssetDatabase.LoadAssetAtPath(
                    "Assets/Editor/Icons/TilePropertyDisablePreview Icon.png", typeof(Texture2D));
            }
            GUIContent disable_button = new GUIContent("Disable All Previews", disable_button_icon);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(enable_button))
            {
                EnableAllPreviews();
            }
            if (GUILayout.Button(disable_button))
            {
                DisableAllPreviews();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(16);

            // Build addressable buttons
            if (build_button_icon == null)
            {
                build_button_icon = (Texture2D)AssetDatabase.LoadAssetAtPath(
                    "Assets/Editor/Icons/TilePropertyAddressable Icon.png", typeof(Texture2D));
            }
            GUIContent build_button = new GUIContent("Build All Tile Property Files", build_button_icon);
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.LabelField("Use the button below to update the addressable groups for your tile property lists, every time you change/update them!");
            GUILayout.Space(4);
            if (GUILayout.Button(build_button))
            {
                BuildAddressables();
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }

        // --------------------------------------------
        //
        // Private functions for custom settings behaviour
        //
        // --------------------------------------------

        private void EnableAllPreviews()
        {
            // Enable previews for every property list in the settings file
            if (settings_prop.arraySize != 0)
            {
                for (int i = 0; i < settings_prop.arraySize; i++)
                {
                    SerializedProperty entry = settings_prop.GetArrayElementAtIndex(i);
                    SerializedProperty entry_list = entry.FindPropertyRelative("IsVisible");

                    entry_list.boolValue = true;
                }
            }
            else
            {
                Debug.LogWarning("No property list previews can be found to enable!");
            }
        }

        private void DisableAllPreviews()
        {
            // Disable previews for every property list in the settings file
            if (settings_prop.arraySize != 0)
            {
                for (int i = 0; i < settings_prop.arraySize; i++)
                {
                    SerializedProperty entry = settings_prop.GetArrayElementAtIndex(i);
                    SerializedProperty entry_list = entry.FindPropertyRelative("IsVisible");

                    entry_list.boolValue = false;
                }
            }
            else
            {
                Debug.LogWarning("No property list previews can be found to disable!");
            }
        }

        private void BuildAddressables()
        {
            string[] tile_properties = AssetDatabase.FindAssets("t:TilePropertiesList");
            Debug.Log(tile_properties.Length);
            if (tile_properties.Length != 0)
            {
                // Automatically build the addressable assets for every Tile Property List in the project, and only for those assets
                var build_settings = AddressableAssetSettingsDefaultObject.Settings;
                var original_group_states = new Dictionary<AddressableAssetGroup, bool>();

                foreach (AddressableAssetGroup group in build_settings.groups)
                {
                    var group_schema = group.GetSchema<BundledAssetGroupSchema>();
                    if (group_schema != null)
                    {
                        original_group_states[group] = group_schema.IncludeInBuild;
                        group_schema.IncludeInBuild = group.name == TPAddressableProcessor.addressable_group_name;

                        EditorUtility.SetDirty(group_schema);
                    }
                }

                AddressableAssetSettings.BuildPlayerContent();

                // Reset all other addressable groups to be "build-able" again
                foreach (var state_value in original_group_states)
                {
                    var group_schema = state_value.Key.GetSchema<BundledAssetGroupSchema>();
                    if (group_schema != null)
                    {
                        group_schema.IncludeInBuild = state_value.Value;

                        EditorUtility.SetDirty(group_schema);
                    }
                }
            }
            else
            {
                Debug.LogError("No tile property lists can be found to build addressable groups for in this project!");
            }
        }
    }
}

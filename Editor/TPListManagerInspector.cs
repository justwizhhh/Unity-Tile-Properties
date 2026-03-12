using UnityEditor;
using UnityEngine;

namespace TileProperties.Editor
{
    [InitializeOnLoad]
    [CustomEditor(typeof(TilePropertiesManager))]
    public class TPListManagerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(2);
            EditorGUILayout.LabelField("This list contains every Tile Property List that the manager script will have access to.");
            GUILayout.Space(4);
            EditorGUILayout.LabelField("It is heavily advised to use DontDestroyOnLoad() on this object to keep property lists across multiple scenes!");
            GUILayout.Space(8);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}

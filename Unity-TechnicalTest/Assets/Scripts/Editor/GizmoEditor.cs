using UnityEngine;
using UnityEditor;
using technical.test.editor;

public class GizmoEditor : EditorWindow {
    
    private GizmoAsset gizmoAsset;

    [MenuItem("Window/Custom/Informative Gizmo")]
    public static void ShowWindow() {
        GetWindow<GizmoEditor>("Gizmo Editor");
        GetWindow<GizmoEditor>().GetSelectedAsset();
    }

    /* We want to know the selected GizmoAsset in order to update
     * the content of our custom Gizmo Editor Window accordingly */
    void GetSelectedAsset() {
        Object[] selection = Selection.GetFiltered(typeof(GizmoAsset), SelectionMode.Assets);
        if (selection.Length > 0) {
            if (selection[0] == null)
                return;

            gizmoAsset = (GizmoAsset)selection[0];
        }
    }

    private void OnGUI() {
        
        if (gizmoAsset == null) {
            return;
        }

        /* I am using SerializedObject and SerializedProperty instead
         * of putting a getter and a setter in the script GizmoAsset
         * to simplify property modifications */
        SerializedObject serializedObject = new SerializedObject(gizmoAsset);
        SerializedProperty gizmos = serializedObject.FindProperty("_gizmos");
        
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Text");
        EditorGUILayout.LabelField("Position");
        EditorGUILayout.EndHorizontal();
        
        for (int i = 0; i < gizmos.arraySize; i++) {

            SerializedProperty gizmoName = gizmos.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
            SerializedProperty gizmoPosition = gizmos.GetArrayElementAtIndex(i).FindPropertyRelative("Position");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(gizmoName, GUIContent.none);
            EditorGUILayout.PropertyField(gizmoPosition, GUIContent.none); 
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    void OnSelectionChange() { GetSelectedAsset(); Repaint(); }
}

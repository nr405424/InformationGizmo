using UnityEngine;
using UnityEditor;
using technical.test.editor;

[CustomEditor(typeof(GizmoAsset))]
public class CustomInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Open Gizmo Editor")) {
            GizmoEditor.ShowWindow();
        }
    }
}

using UnityEngine;
using UnityEditor;
using technical.test.editor;

public class GizmoEditor : EditorWindow {

    #region Variables
    // References
    private static GizmoEditor _instance = null;
    private static SerializedObject gizmoAsset;
    private static SerializedProperty gizmos;
    private GizmoAsset gizmoAssetReference;
    
    // GUI Management
    private bool[] gizmoSelectedState;
    private Vector3[] gizmoInitialPos;
    private static bool gizmoPosInitialized = false;
    private bool resetIsClicked = false;
    private bool deleteIsClicked = false;
    #endregion
    
    [MenuItem("Window/Custom/Informative Gizmo")]
    public static void ShowWindow() {
        _instance = GetWindow<GizmoEditor>("Gizmo Editor");
        _instance.GetSelectedAsset();
    }

    /* We want to know the selected GizmoAsset in order to update
     * the content of our custom Gizmo Editor Window accordingly. */
    void GetSelectedAsset() {
        Object[] selection = Selection.GetFiltered(typeof(GizmoAsset), SelectionMode.Assets);

        if (selection.Length > 0) {
            if (selection[0] == null) {
                return;
            }

            gizmoAssetReference = (GizmoAsset)selection[0];

            // This will allow us to access the selected GizmoAsset's InformationGizmo.
            gizmoAsset = new SerializedObject(gizmoAssetReference);
            gizmos = gizmoAsset.FindProperty("_gizmos");

            // We will need this to know which gizmo is currently selected.
            gizmoSelectedState = new bool[gizmos.arraySize];

            if (!gizmoPosInitialized) {
                /* We want to save the gizmos' initial positions so that we can reset them later on if needed.
                 * Thus, we want their positions to be initialized only once.*/
                gizmoInitialPos = new Vector3[gizmos.arraySize];
                for (int i = 0; i < gizmos.arraySize; i++) {
                    gizmoInitialPos[i] = gizmos.GetArrayElementAtIndex(i).FindPropertyRelative("Position").vector3Value;
                }
                gizmoPosInitialized = true;
            }
            

        }

    }

    void OnSelectionChange() {
        GetSelectedAsset();
        Repaint();
    }

    #region GUI Events
    private void OnGUI() {
        
        if (gizmoAssetReference == null) {
            return;
        }
        
        EditorGUILayout.BeginVertical();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Text");
        EditorGUILayout.LabelField("Position");
        EditorGUILayout.EndHorizontal();

        if (gizmos != null) {
            for (int i = 0; i < gizmos.arraySize; i++) {
                
                SerializedProperty gizmoName = gizmos.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
                SerializedProperty gizmoPosition = gizmos.GetArrayElementAtIndex(i).FindPropertyRelative("Position");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(gizmoName, GUIContent.none);
                EditorGUILayout.PropertyField(gizmoPosition, GUIContent.none);
                if (GUILayout.Button("Edit")) {
                    Edit(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            gizmoAsset.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical();

        // We want the editor window to update whenever we make gizmo modifications.
        Repaint();
    }
    
    void OnEnable() {
        // Remove delegate listener if it has previously been assigned.
        SceneView.onSceneGUIDelegate -= OnSceneGUI;

        // Add (or re-add) the delegate.
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy() {
        /* When the window is destroyed, remove the delegate
         * so that it will no longer do any drawing.*/
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView) {

        if ((gizmoAssetReference == null) || (gizmos ==  null)) {
            return;
        }

        GUIStyle labelStyle = new GUIStyle { fontStyle = FontStyle.Bold };

        for (int i = 0; i < gizmos.arraySize; i++) {
            
            SerializedProperty gizmoName = gizmos.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
            SerializedProperty gizmoPosition = gizmos.GetArrayElementAtIndex(i).FindPropertyRelative("Position");

            // Display gizmos

            Handles.color = Color.white;
            Handles.SphereHandleCap(i, gizmoPosition.vector3Value, Quaternion.identity, 10f, EventType.Repaint);

            Handles.color = Color.black;
            Handles.Label(gizmoPosition.vector3Value + new Vector3(0, 15, 0), gizmoName.stringValue, labelStyle);
            Handles.DrawLine(gizmoPosition.vector3Value, gizmoPosition.vector3Value + new Vector3(0, 10, 0));
            
            // Move selected gizmo

            #region Alternative gizmo selection method
            /* Comment the Handles.SphereHandleCap line above and uncomment the code below
             *to make spheres selectable directly via Scene View*/

            //if (Handles.Button(gizmoPosition.vector3Value, Quaternion.identity, 10f, 3f, Handles.SphereHandleCap)) {
            //    for (int j = 0; j < gizmoSelectedState.Length; j++) {
            //        gizmoSelectedState[j] = false;
            //    }

            //    gizmoSelectedState[i] = true;
            //}
            #endregion


            if (gizmoSelectedState[i]) {
                gizmoPosition.vector3Value = Handles.PositionHandle(gizmoPosition.vector3Value, Quaternion.identity);

                #region Generic menu to reset or delete gizmo
                
                if ((Event.current.type == EventType.MouseUp) && (Event.current.button == 1)) {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Reset Position"), false, ContextCallback, "reset");
                    menu.AddItem(new GUIContent("Delete Gizmo"), false, ContextCallback, "delete");
                    menu.ShowAsContext();

                    Event.current.Use();
                }

                if (resetIsClicked) {
                    gizmoPosition.vector3Value = new Vector3(gizmoInitialPos[i].x, gizmoInitialPos[i].y, gizmoInitialPos[i].z);
                }
                
                resetIsClicked = false;

                if (deleteIsClicked) {
                    gizmos.DeleteArrayElementAtIndex(i);
                }

                deleteIsClicked = false;
                #endregion
            }
            
        }

        gizmoAsset.ApplyModifiedProperties();
    }
    #endregion

    #region Functions

    /// <summary>
    /// Allows us to select a gizmo.
    /// </summary>
    /// <param name="i">Represents the selected state of the gizmo at index i in gizmos.</param>
    private void Edit(int i) {
        for (int j = 0; j < gizmoSelectedState.Length; j++) {
            gizmoSelectedState[j] = false;
        }
        gizmoSelectedState[i] = true;

        if (SceneView.sceneViews.Count > 0) {
            SceneView sceneView = (SceneView)SceneView.sceneViews[0];
            sceneView.Repaint();
        }
    }

    /// <summary>
    /// The function called upon when a generic menu button is clicked.
    /// </summary>
    /// <param name="obj">Allows us to know which button is clicked.</param>
    private void ContextCallback(object obj) {
        string str = obj.ToString();
        switch (str) {
            case ("reset"):
                resetIsClicked = true;
                break;

            case ("delete"):
                deleteIsClicked = true;
                break;

            default: break;
        }
    }
    #endregion
}

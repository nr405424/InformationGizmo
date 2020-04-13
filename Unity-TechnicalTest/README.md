# InformationGizmo

/!\ The delivery commit is on the develop branch and is not merged with the master branch.

ASSUMPTIONS

- I assume that existing files in the initial project (GizmoAsset.cs and GizmoInformation.cs)
should not be modified.

Requirement A.
- I assume that the custom Gizmo Editor window works like the inspector :
	* it won't show anything unless we click on a GizmoAsset scriptable object.
	* it will automatically show the corresponding contents of the selected GizmoAsset scriptable object
	  when opening the window via custom inspector.

Requirement C.
- I assume that the gizmo spheres are only visible when the editor window is open, and they disappear
  as soon as the editor window is closed.

Requirement D.
- I assume that we can only select a gizmo through clicking on the button edit. To allow direct selection
  via Scene View, cf. GizmoEditor.cs

Requirement G.
- I assume that I can reset or delete a gizmo only when it is selected.

COMMENTS

Seeing as it is my first time working on editor scripting, I feel very proud to have respected almost all
of the requirements despite having some complications. I was unable to finish the last requirement (undoing
changes using ctrl z).

I mainly struggled on :
- understanding about how certain things work such as SceneView, Handles, OnSceneGUI in a short amount
of time.
- deciding on how to approach a certain problem : for example, I had to understand how Gizmo/Handle Controls
work and figure out if they would be useful in making a gizmo interactable.
- making the gizmos selectable, as well as rendering them in the beginning, as I did not know of
the existence of Handles in Unity.
- undoing the changes when pressing ctrl z. I tried using the Undo class but it doesn't seem to work on
the editor window. Instead, it seems to affect other viewports such as the Inspector window.
- optimizing the code because I prioritized meeting all the requirements first.
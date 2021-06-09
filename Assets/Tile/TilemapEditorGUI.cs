using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TilemapEditor))]
public class TilemapEditorGUI : Editor
{
    private Vector3 center;
    private GameObject objectUnder;
    private bool isDragging;
    private Vector3 lastPosition;
    TilemapEditor myScript;
    int toolbarInt = 0;
    string[] toolbarStrings = {"Add tile", "Delete"};
    private void OnEnable() {

        myScript = (TilemapEditor)target;
        isDragging = false;

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

        switch (toolbarInt)
        {
            case 0:
            myScript.currentTool = Tools.Add;
            break;

            case 1:
            myScript.currentTool = Tools.Delete;
            break;
        }
        if(GUILayout.Button("Clear")){
            myScript.Reset();
        }
    }
    private void OnSceneGUI() {
        HandleUtility.AddDefaultControl(0);
        if(Event.current.type == EventType.KeyDown){
            if (Event.current.keyCode == (KeyCode.A))
            {
                myScript.currentLayer+=1;
            }else if (Event.current.keyCode == KeyCode.E){
                myScript.currentLayer-=1;
            }
        }
        Plane plane = new Plane(Vector3.up, Vector3.up * myScript.currentLayer);
        Ray ray = HandleUtility.GUIPointToWorldRay( Event.current.mousePosition );
        Vector3 posGrid = new Vector3(0,0,0);
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 pos = ray.GetPoint(enter);
            posGrid = new Vector3((int)pos.x ,myScript.currentLayer ,(int)pos.z);
            center = posGrid;
            SceneView.RepaintAll();
        }
        Handles.DrawWireCube(center,new Vector3(1f,1f,1f));
        if(Event.current.type == EventType.MouseDown){
            if(Event.current.button == 0){
                myScript.ClickEditor(posGrid);
                EditorUtility.SetDirty( target );
                isDragging = true;
                center = lastPosition;
            }
        }else if(Event.current.type == EventType.MouseUp){
            if(Event.current.button == 0){
                isDragging = false;
            }
        }else if(Event.current.type == EventType.MouseDrag){
            if(Event.current.button == 0){
                if(center!= lastPosition){
                    myScript.ClickEditor(posGrid);
                    EditorUtility.SetDirty( target );
                }
            }
        }
    }
}
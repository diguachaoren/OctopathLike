using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TilemapEditor))]
public class TilemapEditorGUI : Editor
{
    private Vector3 center;
    private GameObject objectUnder;
    TilemapEditor myScript;
    private void OnEnable() {

        myScript = (TilemapEditor)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TilemapEditor myScript = (TilemapEditor)target;
        if(GUILayout.Button("Base")){
            myScript.ChangeObject(0);
        }
        if(GUILayout.Button("Corner")){
            myScript.ChangeObject(1);
        }
        if(GUILayout.Button("LineEnd")){
            myScript.ChangeObject(2);
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
            }else if (Event.current.keyCode == KeyCode.R){
                myScript.rota.eulerAngles += new Vector3(0,90,0);
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
                myScript.BuildObject(posGrid);
                EditorUtility.SetDirty( target );
            }
            else if(Event.current.button == 1){
                myScript.DeleteObject(posGrid);
                EditorUtility.SetDirty( target );
            }
        }
    }
}
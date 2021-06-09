using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[System.Serializable]
public class TilemapLayer{
    [SerializeField, HideInInspector]
    public SerializableDictionary<Vector3Int, GameObject> tiles = new SerializableDictionary<Vector3Int, GameObject> ();
    [SerializeField, HideInInspector]
    private Vector3Int immediateOffsets;
    public void UpdateObject(TilePreset presets){
        foreach(GameObject go in tiles.Values){
            int neightbours = 0;
            var Direction = Vector3Int.zero;
            Vector3Int tilePos = new Vector3Int((int)go.transform.position.x, (int)go.transform.position.y, (int)go.transform.position.z);
            if(tiles.ContainsKey(new Vector3Int(tilePos.x + 1,tilePos.y, tilePos.z-1))){ neightbours++;}
            if(tiles.ContainsKey(new Vector3Int(tilePos.x + 1,tilePos.y, tilePos.z))){ neightbours++;}
            if(tiles.ContainsKey(new Vector3Int(tilePos.x + 1,tilePos.y, tilePos.z+1))){ neightbours++;}
            if(tiles.ContainsKey(new Vector3Int(tilePos.x,tilePos.y, tilePos.z - 1))){ neightbours++;}
            if(tiles.ContainsKey(new Vector3Int(tilePos.x,tilePos.y, tilePos.z + 1))){ neightbours++;}
            if(tiles.ContainsKey(new Vector3Int(tilePos.x - 1,tilePos.y, tilePos.z-1))){ neightbours++;}
            if(tiles.ContainsKey(new Vector3Int(tilePos.x - 1,tilePos.y, tilePos.z))){ neightbours++;}
            if(tiles.ContainsKey(new Vector3Int(tilePos.x - 1,tilePos.y, tilePos.z+1))){ neightbours++;}
            switch(neightbours){
                case 0:
                    go.GetComponent<MeshFilter>().mesh = presets.Base.GetComponent<MeshFilter>().sharedMesh;
                    go.GetComponent<MeshCollider>().sharedMesh  = presets.Base.GetComponent<MeshFilter>().sharedMesh;
                    break;
                case 1:
                    go.GetComponent<MeshFilter>().mesh = presets.LineEnd.GetComponent<MeshFilter>().sharedMesh;
                    go.GetComponent<MeshCollider>().sharedMesh  = presets.LineEnd.GetComponent<MeshFilter>().sharedMesh;
                    break;
                case 2:
                    go.GetComponent<MeshFilter>().mesh = presets.Base.GetComponent<MeshFilter>().sharedMesh;
                    go.GetComponent<MeshCollider>().sharedMesh  = presets.Base.GetComponent<MeshFilter>().sharedMesh;
                    break;
                case 3:
                    go.GetComponent<MeshFilter>().mesh = presets.Corner.GetComponent<MeshFilter>().sharedMesh;
                    go.GetComponent<MeshCollider>().sharedMesh  = presets.Corner.GetComponent<MeshFilter>().sharedMesh;
                    break;
                default:
                    go.GetComponent<MeshFilter>().mesh = presets.Base.GetComponent<MeshFilter>().sharedMesh;
                    go.GetComponent<MeshCollider>().sharedMesh  = presets.Base.GetComponent<MeshFilter>().sharedMesh;
                    break;
            }
        }
    }
}
[System.Serializable]
public class TilemapEditor : MonoBehaviour 
{
    public GameObject tile;
    public GameObject layerGO;
    public TilePreset preset;
    public Vector3 spawnPoint;
    public int currentLayer;
    public Quaternion rota;
    [SerializeField, HideInInspector]
    public SerializableDictionary<int, TilemapLayer> layers = new SerializableDictionary<int, TilemapLayer> ();
    [SerializeField, HideInInspector]
    public SerializableDictionary<int, Transform> layerContainers = new SerializableDictionary<int, Transform> ();

    private void Awake(){
        foreach(Transform cur in layerContainers.Values){
            CombineMesh(cur.gameObject);
            cur.GetComponent<MeshRenderer>().material = preset.material;
        }
    }
    private void CombineMesh(GameObject obj){
        //Temporarily set position to zero to make matrix math easier
        Vector3 position = obj.transform.position;
        obj.transform.position = Vector3.zero;

        //Get all mesh filters and combine
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        Debug.Log(combine.Length);
        int i = 1;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        obj.transform.gameObject.SetActive(true);

        //Return to original position
        obj.transform.position = position;

        //Add collider to mesh (if needed)
        obj.AddComponent<MeshCollider>();
    }
    public void BuildObject(Vector3 pos)
    {
        if(!layers.ContainsKey(currentLayer)){
            var newLayer = Instantiate(layerGO, Vector3.zero, Quaternion.identity, this.transform);
            newLayer.name = "Layer" + currentLayer;
            layerContainers.Add(currentLayer,newLayer.transform);
            layers.Add(currentLayer,new TilemapLayer());
        }
        GameObject newTile = Instantiate(tile,new Vector3(pos.x,currentLayer,pos.z), rota, layerContainers[currentLayer]);
        if(layers[currentLayer].tiles.ContainsKey(new Vector3Int((int)pos.x,currentLayer,(int)pos.z))){
            DeleteObject(new Vector3Int((int)pos.x,currentLayer,(int)pos.z));
            layers[currentLayer].tiles[new Vector3Int((int)pos.x,currentLayer,(int)pos.z)] = newTile;
        }else{
            layers[currentLayer].tiles.Add(new Vector3Int((int)pos.x,currentLayer,(int)pos.z), newTile);
        }
        layers[currentLayer].UpdateObject(preset);
    }

    public void DeleteObject(Vector3 pos){
        if(layers[currentLayer].tiles.ContainsKey(new Vector3Int((int)pos.x,currentLayer,(int)pos.z))){
            try{
                DestroyImmediate(layers[currentLayer].tiles[new Vector3Int((int)pos.x,currentLayer,(int)pos.z)]);
                layers[currentLayer].tiles.Remove(new Vector3Int((int)pos.x,currentLayer,(int)pos.z));
            }catch{}
            finally{            
                layers[currentLayer].UpdateObject(preset);
            }
        }

    }

    public void ChangeObject(int index){
        switch(index){
            case 0:
                tile = preset.Base;
                break;
            case 1:
                tile = preset.Corner;
                break;
            case 2:
                tile = preset.LineEnd;
                break;
        }
    }
    public void Reset(){
        layers = new SerializableDictionary<int, TilemapLayer> ();
        layerContainers = new SerializableDictionary<int, Transform> ();
        int childs = transform.childCount;
        for (int i = childs - 1; i >= 0; i--){
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

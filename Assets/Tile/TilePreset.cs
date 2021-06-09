using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TilePreset", menuName = "OctopathLike/TilePreset", order = 0)]
public class TilePreset : ScriptableObject {
    public GameObject Corner;
    public GameObject Base;
    public GameObject LineEnd;
    public Material material;
}

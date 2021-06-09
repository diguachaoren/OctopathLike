using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private Transform camera;
    // Start is called before the first frame update
    void Awake()
    {
        camera = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(new Vector3(camera.position.x, transform.position.y, camera.position.z));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour {
    public GameObject prefab;
    public List<GameObject> objects = new List<GameObject>();

    public void ListEm()
    {
        foreach (Transform t in prefab.transform)
        {
            GameObject go = t.gameObject;
            go.transform.parent = null;
            go.transform.rotation = Quaternion.Euler(0, 0, 0);
            go.transform.position = Vector3.zero;
            go.AddComponent<MeshCollider>();
            go.GetComponent<MeshCollider>().sharedMesh = go.GetComponent<MeshFilter>().mesh;
            objects.Add(go);
        }
    } 
}

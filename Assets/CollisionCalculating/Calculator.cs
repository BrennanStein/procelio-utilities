using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    public GameObject prefab;
    public List<GameObject> objects = new List<GameObject>();

    public void ListEm()
    {
        var list = prefab.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < 10;/* list.Length;*/ ++i)
        {
            GameObject go = list[i].gameObject;
            go.transform.parent = null;
            go.transform.rotation = Quaternion.Euler(0, 0, 0);
            go.transform.position = Vector3.zero;
            go.transform.localScale = new Vector3(1.01f,1.01f,1.01f);
            objects.Add(go);
        }
    }
    public void OutputEnum()
    {
        string z = Application.persistentDataPath;
        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(z + "/ENUM.txt"))
        {
            file.WriteLine("enum CollisionPrimitives {");
            foreach (var thing in objects)
            {
                file.WriteLine(thing.name + ",");
            }
            file.WriteLine("}");
        }
        Debug.Log("PRINTED");
    }

    public void Start()
    {
        ListEm();
        OutputEnum();
        StartCoroutine(CheckCollisions());
    }

    public IEnumerator CheckCollisions()
    {
        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(Application.persistentDataPath + "/collisions.txt"))
        {
            file.WriteLine("bool[][] data = new bool[][] {");
            Debug.Log(objects.Count);
            for (int i = 0; i < objects.Count; ++i)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("/*" + objects[i].name + "*/ { ");
                for (int j = 0; j < objects.Count; ++j)
                {
                    if (i == j)
                    {
                        sb.Append("true, ");
                        continue;
                    }
                    objects[i].SetActive(true);
                    objects[i].AddComponent<CollideTrack>();

                    objects[i].AddComponent<MeshCollider>().convex = true;
                    objects[i].AddComponent<Rigidbody>().isKinematic = true;
                    objects[i].GetComponent<MeshCollider>().isTrigger = true;
                    objects[i].GetComponent<MeshCollider>().sharedMesh = objects[i].GetComponent<MeshFilter>().mesh;
                    objects[j].SetActive(true);
                    objects[j].transform.position = new Vector3(10, 10, 10);
                    objects[j].AddComponent<CollideTrack>();

                    objects[j].AddComponent<MeshCollider>().convex = true;
                    objects[j].AddComponent<Rigidbody>().isKinematic = true;
                    objects[j].GetComponent<MeshCollider>().isTrigger = true;
                    objects[j].GetComponent<MeshCollider>().sharedMesh = objects[j].GetComponent<MeshFilter>().mesh;
                    objects[j].transform.position = Vector3.zero;
                    yield return new WaitForSeconds(.05f);
                    if (objects[i].GetComponent<CollideTrack>().col)
                    {
                        sb.Append("true, ");
                    }
                    else
                        sb.Append("false,");
                    objects[i].SetActive(false);
                    objects[j].SetActive(false);
                    DestroyImmediate(objects[i].GetComponent<CollideTrack>());
                    DestroyImmediate(objects[i].GetComponent<MeshCollider>());
                    DestroyImmediate(objects[j].GetComponent<CollideTrack>());
                    DestroyImmediate(objects[j].GetComponent<MeshCollider>());
                    DestroyImmediate(objects[i].GetComponent<Rigidbody>());
                    DestroyImmediate(objects[j].GetComponent<Rigidbody>());
                }
                sb.Append("},");
                file.WriteLine(sb.ToString());
            }
            file.WriteLine("};");
        }
        Debug.Log("CHECKED:)");
    }
}

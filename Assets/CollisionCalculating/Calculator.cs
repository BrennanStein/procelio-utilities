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
        for (int i = 0; i < list.Length; ++i)
        {
            GameObject go = list[i].gameObject;
            go.transform.parent = null;
            go.transform.rotation = Quaternion.Euler(0, 0, 0);
            go.transform.position = Vector3.zero;
            go.transform.localScale = new Vector3(1.01f, 1.01f, 1.01f);

            objects.Add(go);
            go.SetActive(false);
        }
        Debug.Log(list.Length + " entries");
    }
    public void OutputEnum()
    {
        string z = Application.persistentDataPath;
        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(z + "/ENUM.txt"))
        {
            file.WriteLine("enum ColPriv {");
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
        HashSet<int> types = new HashSet<int> { 1, 2, 3, 4 };
    }
    Vector3[] vs = new Vector3[] { new Vector3(0.01f, 0, 0), new Vector3(-0.01f, 0, 0), new Vector3(0, 0.01f, 0), new Vector3(0, -0.01f, 0), new Vector3(0, 0, 0.01f), new Vector3(0, 0, -0.01f) };
    public IEnumerator CheckCollisions()
    {
        int countt = 0;
        Debug.Log("Time: " + Time.time);
        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(Application.persistentDataPath + "/collisions.txt"))
        {
            file.WriteLine("HashSet<int> collisions = new HashSet<int> {");
            StringBuilder sb = new StringBuilder();
            Debug.Log(objects.Count);
            for (int i = 0; i < objects.Count; ++i)
            {

                countt++;
                // sb.Append("" + ((1 << 8) | 1)); // *know* that first thing (w/ cube) is true -- can fencepost commas
                for (int j = 1; j < objects.Count; ++j)
                {
                    countt++;
                    if (countt % 20 == 0)
                    {
                        Debug.Log("tick");

                    }
                    if (sb.Length > 50)
                    {
                        file.WriteLine(sb.ToString());
                        sb.Length = 0;
                    }
                    if (i == j) continue;
                    bool col = true;
                    foreach (Vector3 v in vs)
                    {
                        objects[i].SetActive(true);
                        objects[i].AddComponent<CollideTrack>();

                        objects[i].AddComponent<MeshCollider>().convex = true;
                        objects[i].AddComponent<Rigidbody>().isKinematic = true;
                        objects[i].GetComponent<Renderer>().material.color = Random.ColorHSV();
                        objects[i].GetComponent<MeshCollider>().isTrigger = true;
                        objects[i].GetComponent<MeshCollider>().sharedMesh = objects[i].GetComponent<MeshFilter>().mesh;
                        objects[i].transform.position = Vector3.zero;

                        objects[j].SetActive(true);
                        objects[j].transform.position = new Vector3(10, 10, 10);
                        objects[j].AddComponent<CollideTrack>();
                        objects[j].GetComponent<Renderer>().material.color = Random.ColorHSV();
                        objects[j].AddComponent<MeshCollider>().convex = true;
                        objects[j].AddComponent<Rigidbody>().isKinematic = true;
                        objects[j].GetComponent<MeshCollider>().isTrigger = true;
                        objects[j].GetComponent<MeshCollider>().sharedMesh = objects[j].GetComponent<MeshFilter>().mesh;
                        objects[j].transform.position = v;
                        yield return new WaitForFixedUpdate();


                        if (!objects[i].GetComponent<CollideTrack>().col)
                        {
                            //   Debug.Log("UNHIT");
                            col = false;
                            objects[i].SetActive(false);
                            objects[j].SetActive(false);
                            DestroyImmediate(objects[i].GetComponent<CollideTrack>());
                            DestroyImmediate(objects[i].GetComponent<MeshCollider>());
                            DestroyImmediate(objects[j].GetComponent<CollideTrack>());
                            DestroyImmediate(objects[j].GetComponent<MeshCollider>());
                            DestroyImmediate(objects[i].GetComponent<Rigidbody>());
                            DestroyImmediate(objects[j].GetComponent<Rigidbody>());
                            break;
                        }
                        objects[i].SetActive(false);
                        objects[j].SetActive(false);
                        DestroyImmediate(objects[i].GetComponent<CollideTrack>());
                        DestroyImmediate(objects[i].GetComponent<MeshCollider>());
                        DestroyImmediate(objects[j].GetComponent<CollideTrack>());
                        DestroyImmediate(objects[j].GetComponent<MeshCollider>());
                        DestroyImmediate(objects[i].GetComponent<Rigidbody>());
                        DestroyImmediate(objects[j].GetComponent<Rigidbody>());

                    }
                    if (!col)
                    {
                        sb.Append(", " + ((i << 8) | j));
                    }
                }
            }
            sb.Append("};");

            file.WriteLine(sb.ToString());
        }
        Debug.Log("Time: " + Time.time);
        Debug.Log("CHECKED:)");
    }
}

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
        //  StartCoroutine(CheckCollisions());
        StartCoroutine(CalculateMeshRotations());
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
                        objects[i].GetComponent<MeshCollider>().isTrigger = true;
                        objects[i].GetComponent<MeshCollider>().sharedMesh = objects[i].GetComponent<MeshFilter>().mesh;
                        objects[i].transform.position = Vector3.zero;

                        objects[j].SetActive(true);
                        objects[j].transform.position = new Vector3(10, 10, 10);
                        objects[j].AddComponent<CollideTrack>();

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
                        yield return new WaitForEndOfFrame();
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

    private string GetClosest(MeshFilter m, Quaternion rot)
    {
        GameObject g = m.gameObject;
        g.transform.rotation = rot; ;
        g.transform.position = Vector3.zero;
        foreach (var thing in objects)
        {
            if (thing.name == g.name) continue;
            if (g.name == "PX_PZ_NY_COVER" && rot.Equals(Quaternion.Euler(0, 0, 90)))
            {
                Debug.Log("A");
            }
            thing.transform.position = Vector3.zero;
            thing.transform.rotation = Quaternion.identity;
            List<Vector3> verts = new List<Vector3>(m.mesh.vertices);
            bool valid = true;
            if (g.name == "PX_PZ_NY_COVER" && thing.name == "PX_PZ_PY_COVER" && rot.Equals(Quaternion.Euler(0, 0, 90)))
            {
                string s = "";
                foreach (var z in verts)
                    s += z + " ";
                Debug.Log(s);
                var a = m.mesh.vertices;
                s = "";
                foreach (var z in a)
                    s += (rot*z) + " ";
                Debug.Log(s);
            }
            int counttt = 0;
            
            foreach (var v in m.mesh.vertices)
            {
                Vector3 actual = g.transform.TransformPoint(v);
                if (verts.Count == 0)
                {
                    valid = false;
                    if (g.name == "PX_PZ_NY_COVER" && thing.name == "PX_PZ_PY_COVER" && rot.Equals(Quaternion.Euler(0, 0, 90)))
                    {
                        Debug.Log("QQQ");
                    }
                    break;
                }
                int len = verts.Count;
                int q = 0;
                bool hit = false;
                for (int i = 0; i < verts.Count; ++i, ++q)
                {
                    if (Vector3.Distance(actual, thing.transform.TransformPoint(verts[i])) < 0.004)
                    {
                        hit = true;
                    }
                   
                }
                if (g.name == "PX_PZ_NY_COVER" && thing.name == "PX_PZ_PY_COVER" && rot.Equals(Quaternion.Euler(0, 0, 90)))
                    Debug.Log(verts.Count + "  " + q);
            }
            
            if (verts.Count != 0)
            {
                if (g.name == "PX_PZ_NY_COVER" && thing.name == "PX_PZ_PY_COVER" && rot.Equals(Quaternion.Euler(0, 0, 90)))
                {
                    Debug.Log("NOOO "+counttt+"  ");
                    string s = "";
                    foreach (var z in verts)
                        s += z + " ";
                    Debug.Log(s);
                }
                valid = false;
            }
            if (valid)
                return thing.name;
        }
        return "";
    }
    public IEnumerator CalculateMeshRotations()
    {
        using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(Application.persistentDataPath + "/rotations.txt"))
        {
            for (int i = 0; i < objects.Count; ++i)
            {
                string z = objects[i].name;
                file.WriteLine("private static ColPriv Rotate" + z.ToUpper() + "(int x, int y, int z) {");

                file.WriteLine("  switch(z) {");
                file.WriteLine("    case 1: return Rotate" + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(0, 0, 90)) + "(x, y, 0);");
                file.WriteLine("    case 2: return Rotate" + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(0, 0, 180)) + "(x, y, 0);");
                file.WriteLine("    case 3: return Rotate" + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(0, 0, 270)) + "(x, y, 0);");
                file.WriteLine("  }");

                file.WriteLine("  switch(x) {");
                file.WriteLine("    case 1: return Rotate" + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(90, 0, 0)) + "(0, y, 0);");
                file.WriteLine("    case 2: return Rotate" + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(180, 0, 0)) + "(0, y, 0);");
                file.WriteLine("    case 3: return Rotate" + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(270, 0, 0)) + "(0, y, 0);");
                file.WriteLine("  }");

                file.WriteLine("  switch(y) {");
                file.WriteLine("    case 1: return ColPriv." + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(0, 90, 0)) + ";");
                file.WriteLine("    case 2: return ColPriv." + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(0, 180, 0)) + ";");
                file.WriteLine("    case 3: return ColPriv." + GetClosest(objects[i].GetComponent<MeshFilter>(), Quaternion.Euler(0, 270, 0)) + ";");
                file.WriteLine("  }");
                file.WriteLine("  return ColPriv." + z.ToUpper() + ";");
                file.WriteLine("}");
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("X");
        }
        Debug.Log("DONE");
    }
}
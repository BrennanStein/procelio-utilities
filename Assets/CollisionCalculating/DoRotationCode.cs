using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoRotationCode : MonoBehaviour
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

    private bool match(Vector3[] i, Vector3[] j)
    {
        if (i.Length != j.Length) return false;
        float sum = 0;
        foreach (Vector3 d1 in i)
        {
            float s = float.MaxValue;
            foreach (Vector3 d2 in j)
                s = Mathf.Min(s, Vector3.Distance(d1, d2));
            sum += s;
        }
        return sum < .1f;
    }

    private bool match(Vector3[] i, Matrix4x4 iTrans, Vector3[] j)
    {
        Vector3[] i2 = new Vector3[i.Length];
        for (int q = 0; q < i.Length; ++q)
            i2[q] = iTrans * i[q];
        return match(i2, j);
    }

    private GameObject matches(GameObject i1, Matrix4x4 tran)
    {
        Vector3[] iVec = i1.GetComponent<MeshFilter>().sharedMesh.vertices;
        foreach (var go in objects)
        {
            if (match(iVec, tran, go.GetComponent<MeshFilter>().sharedMesh.vertices))
                return go;
        }
        Debug.LogError(i1.name + " " + tran);
        return null;
    }

    void printfull(GameObject i1, System.IO.StreamWriter o)
    {
        o.WriteLine("private static ColPriv Rotate" + i1.name + "(int x, int y, int z) {");
        o.WriteLine("    switch(z) {");
        o.WriteLine("        case 1: return Rotate" + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90))).name + "(x, y, 0);");
        o.WriteLine("        case 2: return Rotate" + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180))).name + "(x, y, 0);");
        o.WriteLine("        case 3: return Rotate" + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 270))).name + "(x, y, 0);");
        o.WriteLine("    }");
        o.WriteLine("    switch(x) {");
        o.WriteLine("         case 1: return Rotate" + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(90, 0, 0))).name + "(0, y, 0);");
        o.WriteLine("         case 2: return Rotate" + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(180, 0, 0))).name + "(0, y, 0);");
        o.WriteLine("         case 3: return Rotate" + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(270, 0, 0))).name + "(0, y, 0);");
        o.WriteLine("    }");
        o.WriteLine("    switch(y) {");
        o.WriteLine("        case 1: return ColPriv." + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(0, 90, 0))).name + ";");
        o.WriteLine("        case 2: return ColPriv." + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0))).name + ";");
        o.WriteLine("        case 3: return ColPriv." + matches(i1, Matrix4x4.Rotate(Quaternion.Euler(0, 270, 0))).name + ";");
        o.WriteLine("    }");
        o.WriteLine("    return ColPriv." + i1.name + ";");
        o.WriteLine("    }");
    }

    private IEnumerator exportAll(System.IO.StreamWriter str)
    {
        foreach (var obj in objects)
        {
            printfull(obj, str);
            yield return new WaitForFixedUpdate();
            Debug.Log("EXPORTED " + obj.name);
        }
        str.Close();

    }

    void Start()
    {
        ListEm();
        string z = Application.persistentDataPath;
        System.IO.StreamWriter file =
           new System.IO.StreamWriter(z + "/rotations.txt");
        StartCoroutine(exportAll(file));

      
    }
}
using BotData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBuilder : MonoBehaviour
{


    ColPriv partType = (ColPriv)0;
    Vector3Int position = Vector3Int.zero;
    List<KeyValuePair<Vector3Int, ColPriv>> shapes = new List<KeyValuePair<Vector3Int, ColPriv>>();
    Dictionary<Vector3Int, List<GameObject>> manifest = new Dictionary<Vector3Int, List<GameObject>>();
    GameObject ghost;
    public Material whitetrans;
    public Material bluetrans;
    public Material buildtrans;

    public GameObject cameraObj;
    public float distance;

    void Start()
    {
      //  LoadShapesFromGameObjects();
        Cursor.lockState = CursorLockMode.Locked;
        ResetGhost();
        cameraObj = Camera.main.gameObject;
        distance = 10;
    }
    void ResetGhost()
    {
        Destroy(ghost);
        ghost = Instantiate(GameObject.Find("" + partType));
        ghost.transform.SetParent(null);
        ghost.SetActive(true);
        ghost.GetComponent<MeshRenderer>().material = buildonly ? buildtrans : bluetrans;
        if (buildonly)
        {
            partType = (ColPriv) ((int)partType | (int)ColPrivAlterations.IGNORE_IN_BUILD_MODE);
        } else
        {
            partType = (ColPriv)((int)partType & ~(int)ColPrivAlterations.IGNORE_IN_BUILD_MODE);
        }
    }

    bool buildonly = false;

    void Update()
    {
        distance += Input.GetAxis("Mouse ScrollWheel");
        Vector3 ppp = cameraObj.transform.position + cameraObj.transform.forward * distance;
        position = new Vector3Int((int)ppp.x, (int)ppp.y, (int)ppp.z);
        ghost.transform.position = position;
        ghost.transform.rotation = Quaternion.identity;
        if (Input.GetKeyDown(KeyCode.RightBracket) || Input.GetKeyDown(KeyCode.V))
        {
            partType = (ColPriv)(((int)partType + 1) % (int)ColPriv.COUNT);
            ResetGhost();
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetKeyDown(KeyCode.C))
        {
            partType = (ColPriv)(((int)partType - 1 + (int)ColPriv.COUNT) % (int)ColPriv.COUNT);
            ResetGhost();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
            position.x++;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            position.x--;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            position.z++;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            position.z--;
        if (Input.GetKeyDown(KeyCode.Period))
            position.y++;
        if (Input.GetKeyDown(KeyCode.Comma))
            position.y--;
        if (Input.GetKeyDown(KeyCode.Quote))
        {
            buildonly = !buildonly;
            ResetGhost();
        }
        if (Input.GetKeyDown(KeyCode.BackQuote)) { partType = ColPriv.CUBE; ResetGhost(); }          // `: CUBE
        if (Input.GetKeyDown(KeyCode.Alpha1)) { partType = ColPriv.PX_NY_PRISM; ResetGhost(); }   // 1: PRISM
        if (Input.GetKeyDown(KeyCode.Alpha2)) { partType = ColPriv.PX_PZ_NY_TETRA; ResetGhost(); }// 2: TETRA
        if (Input.GetKeyDown(KeyCode.Alpha3)) { partType = ColPriv.PX_PY_PZ_INNER; ResetGhost(); }// 3: INNER
        if (Input.GetKeyDown(KeyCode.Alpha4)) { partType = ColPriv.PX_SLAB; ResetGhost(); }       // 4: HALFSLAB
        if (Input.GetKeyDown(KeyCode.Alpha5)) { partType = ColPriv.PX_NY_COVER; ResetGhost(); }   // 5: PRISM COVER
        if (Input.GetKeyDown(KeyCode.Alpha6)) { partType = ColPriv.PX_PZ_NY_COVER; ResetGhost(); }// 6: TETRA COVER
        if (Input.GetKeyDown(KeyCode.Alpha7)) { partType = ColPriv.PX_PZ_NY_INCOV; ResetGhost(); }// 7: INNER COVER
        if (Input.GetKeyDown(KeyCode.Alpha8)) { partType = ColPriv.X_PILLAR; ResetGhost(); }      // 8: MISC RECTANGLES
        if (Input.GetKeyDown(KeyCode.Alpha9)) { partType = ColPriv.X_RIDGE_NY; ResetGhost(); }    // 9: CENTERED RIDGE
        if (Input.GetKeyDown(KeyCode.Alpha0)) { partType = ColPriv.PX_PZ_FACENY_SLOPESIDE; ResetGhost(); } // 0: SLOPESIDE

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
        {
            Debug.LogError("SAVED MODEL");
            Output();
        }
        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.S))
        {
            Debug.LogError("Saving from gameobjects");
            LoadShapesFromGameObjects();
            Output();
        }

        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Mouse1))
        {
            for (int i = 0; i < shapes.Count; ++i)
                if (shapes[i].Key.Equals(position))
                {
                    shapes.RemoveAt(i);
                    foreach (var thing in manifest[position])
                        Destroy(thing);
                    manifest.Remove(position);
                    break;
                }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            foreach (var t in shapes)
                if (t.Key == position && !Input.GetKey(KeyCode.LeftControl))
                    return;
            shapes.Add(new KeyValuePair<Vector3Int, ColPriv>(position, partType));
            GameObject g = Instantiate(ghost);
            g.GetComponent<MeshRenderer>().material = whitetrans;
            if (!manifest.ContainsKey(position))
                manifest[position] = new List<GameObject>();
            manifest[position].Add(g);
        }
    }

    private void LoadShapesFromGameObjects()
    {
        shapes.Clear();
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
            if (go.activeInHierarchy && (go != ghost))
            {
                if (go.name.IndexOf("(Clone)") < 0) continue;
                string name = go.name.Substring(0, go.name.IndexOf("(Clone)"));
                ColPriv type = (ColPriv)Enum.Parse(typeof(ColPriv), name, true);
                var posKey = new Vector3Int((int)(go.transform.position.x ),
                    (int)( go.transform.position.y), (int)( go.transform.position.z));
                foreach (var t in shapes)
                    continue;
                shapes.Add(new KeyValuePair<Vector3Int, ColPriv>(posKey, type));
            }
        Debug.LogError("SaveSuccess");
    }

    string convertColPriv(ColPriv cp)
    {
        if (cp >= ColPriv.CUBE && cp <= ColPriv.COUNT)
            return "ColPriv." + cp;

        else return ""+(int)cp;

    }
    void Output()
    {
        
        using (System.IO.StreamWriter file =
          new System.IO.StreamWriter(Application.persistentDataPath + "/colliderdata.txt"))
        {
            string toOut = "";
            toOut += ("KeyValuePair<ColPriv, int>[] array = {");
            for (int i = 0; i < shapes.Count; ++i)
            {
                if (i != 0) { toOut += ", "; } // Suck it, traditional fencepost structure
                
                toOut += ("new KeyValuePair<ColPriv, int>("+ convertColPriv(shapes[i].Value) + ", " + BotData.BlockData.AssemblePos(shapes[i].Key.x, shapes[i].Key.y, shapes[i].Key.z) + ")");

            }
            //   var v =  (ColPriv)((int)shapes[i].Value + (int)ColPrivAlterations.IGNORE_IN_BUILD_MODE)
            toOut += "};";
            file.WriteLine(toOut);
            Debug.Log(toOut);
        }
    }
}

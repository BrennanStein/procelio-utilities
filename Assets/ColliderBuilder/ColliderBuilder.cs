using BotData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBuilder : MonoBehaviour
{
    public static int RotateCalcNewColliderPos(int partPlacement, int colliderOffset, byte rotation)
    {
        //  Debug.Log(">"+rotation);
        int x = BlockData.GetX(colliderOffset);
        int y = BlockData.GetY(colliderOffset);
        int z = BlockData.GetZ(colliderOffset);
        int temp = 0;

        // AROUND Z AXIS
        switch (rotation & 0x3)
        {
            case 1:
                temp = y;
                y = x;
                x = -temp;
                break;
            case 2:
                x = -x;
                y = -y;
                break;
            case 3:
                temp = y;
                y = -x;
                x = temp;
                break;
        }

        // AROUND X AXIS
        switch ((rotation >> 4) & 0x3)
        {
            case 1:
                temp = z;
                z = y;
                y = -temp;
                break;
            case 2:
                z = -z;
                y = -y;
                break;
            case 3:
                temp = y;
                y = z;
                z = -temp;
                break;
        }

        // AROUND Y AXIS
        switch ((rotation >> 2) & 0x3)
        {
            case 1:
                temp = x;
                x = z;
                z = -temp;
                break;
            case 2:
                x = -x;
                z = -z;
                break;
            case 3:
                temp = z;
                z = x;
                x = -temp;
                break;
        }
        // Debug.Log(">Z " + (z < 0));
        return BlockData.AssemblePos(x + BlockData.GetX(partPlacement), y + BlockData.GetY(partPlacement), z + BlockData.GetZ(partPlacement));
    }

    ColPriv partType = (ColPriv)0;
    Vector3Int position = Vector3Int.zero;
    List<KeyValuePair<Vector3Int, ColPriv>> shapes = new List<KeyValuePair<Vector3Int, ColPriv>>();
    Dictionary<Vector3Int, List<GameObject>> manifest = new Dictionary<Vector3Int, List<GameObject>>();
    GameObject ghost;
    public Material whitetrans;
    public Material bluetrans;
    public Material lightgreen;
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
        partType = (ColPriv)((int)partType & 0xFF);
        ghost = Instantiate(GameObject.Find("" + partType));
        ghost.transform.SetParent(null);
        ghost.SetActive(true);
        ghost.GetComponent<MeshRenderer>().material = buildonly ? buildtrans : bluetrans;
        if (buildonly)
        {
            partType = (ColPriv)((int)partType | (int)ColPrivAlterations.IGNORE_FOR_COLLISION);
        }
        else
        {
            //   partType = (ColPriv)((int)partType & ~(int)ColPrivAlterations.IGNORE_FOR_COLLISION);
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
            partType = (ColPriv)((int)partType & 0xFF);

            partType = (ColPriv)(((int)partType + 1) % (int)ColPriv.COUNT);

            ResetGhost();
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetKeyDown(KeyCode.C))
        {
            partType = (ColPriv)((int)partType & 0xFF);
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

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            Debug.LogError("SAVED MODEL");
            Output();
        }
        if (Input.GetKey(KeyCode.RightCommand) && Input.GetKeyDown(KeyCode.S))
        {
            Debug.LogError("Saving from gameobjects");
            LoadShapesFromGameObjects();
            Output();
        }
        if (Input.GetKeyDown(KeyCode.Insert))
            StartCoroutine(Rotate((byte)UnityEngine.Random.Range(0, 63)));
        if (Input.GetKeyDown(KeyCode.X))
            StartCoroutine(Rotate(BlockData.AssembleRotation(Quaternion.Euler(90, 0, 0))));
        if (Input.GetKeyDown(KeyCode.Y))
            StartCoroutine(Rotate(BlockData.AssembleRotation(Quaternion.Euler(0, 270, 0))));
        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(Rotate(BlockData.AssembleRotation(Quaternion.Euler(0, 0, 180))));


        if (Input.GetKeyDown(KeyCode.Equals))
            StartCoroutine(Reload());
        if (Input.GetKeyDown(KeyCode.Delete))
            Clear();

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
            if (buildonly)
                g.GetComponent<MeshRenderer>().material = lightgreen;
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
                var posKey = new Vector3Int((int)(go.transform.position.x),
                    (int)(go.transform.position.y), (int)(go.transform.position.z));
                foreach (var t in shapes)
                    continue;
                shapes.Add(new KeyValuePair<Vector3Int, ColPriv>(posKey, type));
            }
        Debug.LogError("SaveSuccess");
    }

    string convertColPriv(ColPriv cp)
    {
        int cpa = (~0xFF) & (int)cp;
        cp = (ColPriv)(0xFF & (int)cp);
        if (cpa == 0)//cp >= ColPriv.CUBE && cp <= ColPriv.COUNT)
            return "ColPriv." + cp;

        else return "ColData.Modify(ColPriv." + cp + ",ColPrivAlterations." + (ColPrivAlterations)cpa + ")";// "+(int)cp;

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

                toOut += ("new KeyValuePair<ColPriv, int>(" + convertColPriv(shapes[i].Value) + ", " + BotData.BlockData.AssemblePos(shapes[i].Key.x, shapes[i].Key.y, shapes[i].Key.z) + ")");

            }
            //   var v =  (ColPriv)((int)shapes[i].Value + (int)ColPrivAlterations.IGNORE_IN_BUILD_MODE)
            toOut += "};";
            file.WriteLine(toOut);
            Debug.Log(toOut);
        }
    }
    public static Vector3Int Vec3(int v)
    {
        return new Vector3Int(BlockData.GetX(v), BlockData.GetY(v), BlockData.GetZ(v));
    }

    List<KeyValuePair<Vector3Int, ColPriv>> Clear()
    {
        foreach (var l1 in manifest)
        {
            foreach (var l2 in l1.Value)
                DestroyImmediate(l2);
        }
        var sold = shapes;
        manifest.Clear();
        shapes = new List<KeyValuePair<Vector3Int, ColPriv>>();
        return sold;
    }
    IEnumerator Rotate(byte rot)
    {
        Debug.Log(rot);
        var sold = Clear();
        yield return new WaitForSeconds(0.1f);
        foreach (var shp in sold)
        {
            int newpos = RotateCalcNewColliderPos(0, BlockData.AssemblePos(shp.Key.x, shp.Key.y, shp.Key.z), rot);
            ColPriv newType = CollideData.GetRotation(shp.Value, rot);
            this.partType = newType;
            var pposition = new Vector3Int(BlockData.GetX(newpos), BlockData.GetY(newpos), BlockData.GetZ(newpos));
            buildonly = (ColPrivAlterations)((int)partType & ~0xFF) == ColPrivAlterations.IGNORE_FOR_COLLISION;

            ResetGhost();

            yield return new WaitForEndOfFrame();// WaitForSeconds(.1f);
            shapes.Add(new KeyValuePair<Vector3Int, ColPriv>(pposition, partType));
            ghost.transform.position = pposition;
            GameObject g = Instantiate(ghost);
            g.GetComponent<MeshRenderer>().material = whitetrans;
            if (buildonly)
                g.GetComponent<MeshRenderer>().material = lightgreen;
            if (!manifest.ContainsKey(pposition))
                manifest[pposition] = new List<GameObject>();
            manifest[pposition].Add(g);
        }
    }


    IEnumerator Reload()
    {
        String str = "        public static readonly System.Tuple<ColPriv, int>[] AlphaNewWheel = { new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 0), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 256), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 512), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 768), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 16712448), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 16712192), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 16711936), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 65792), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 66048), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 66304), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 1023), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 767), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 511), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 257), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 513), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.CUBE, ColPrivAlterations.IGNORE_FOR_COLLISION), 769), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_PZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 16712703), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_PZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 16712447), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_PZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 16712191), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_PZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 66047), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_PZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 66303), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_PZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 66559), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_NZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 66305), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_NZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 66049), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_NZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 65793), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_NZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 16711937), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_NZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 16712193), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_NZ_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 16712449), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_PY_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 16711680), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NZ_PY_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 1), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_PY_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 65536), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PZ_PY_PRISM, ColPrivAlterations.IGNORE_FOR_COLLISION), 255), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_PZ_PY_TETRA, ColPrivAlterations.IGNORE_FOR_COLLISION), 16711935), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.PX_NZ_PY_TETRA, ColPrivAlterations.IGNORE_FOR_COLLISION), 16711681), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_NZ_PY_TETRA, ColPrivAlterations.IGNORE_FOR_COLLISION), 65537), new System.Tuple<ColPriv, int>(CollideData.Modify(ColPriv.NX_PZ_PY_TETRA, ColPrivAlterations.IGNORE_FOR_COLLISION), 65791) };";
        String[] data = str.Split('(');
        for (int i = 1; i < data.Length; ++i)
        {
            var poss = "";
            if (data[i] == "CollideData.Modify")
            {
                var splt = data[++i].Split(',');
                partType = (ColPriv)Enum.Parse(typeof(ColPriv), splt[0].Substring(splt[0].IndexOf('.') + 1));
                var alt = splt[1].Substring(splt[1].IndexOf('.') + 1);
                alt = alt.Substring(0, alt.IndexOf(')'));

                poss = splt[2].Trim();
                poss = poss.Substring(0, splt[2].IndexOf(')') - 1);
                var cpaaa = (ColPrivAlterations)Enum.Parse(typeof(ColPrivAlterations), alt);


                partType = (ColPriv)((int)partType | (int)cpaaa);
                buildonly = cpaaa == ColPrivAlterations.IGNORE_FOR_COLLISION;
            }
            else
            {
                var splt = data[i].Split(',');
                partType = (ColPriv)Enum.Parse(typeof(ColPriv), splt[0].Substring(splt[0].IndexOf('.') + 1));
                poss = splt[1].Trim();
                poss = poss.Substring(0, poss.IndexOf(')'));

            }

            Vector3Int pposition = Vec3(int.Parse(poss));
            //     poition = ???;
            ResetGhost();

            yield return new WaitForEndOfFrame();// (.1f);
            shapes.Add(new KeyValuePair<Vector3Int, ColPriv>(pposition, partType));
            ghost.transform.position = pposition;
            GameObject g = Instantiate(ghost);
            g.GetComponent<MeshRenderer>().material = whitetrans;
            if (buildonly)
                g.GetComponent<MeshRenderer>().material = lightgreen;
            if (!manifest.ContainsKey(pposition))
                manifest[pposition] = new List<GameObject>();
            manifest[pposition].Add(g);
        }
    }
}

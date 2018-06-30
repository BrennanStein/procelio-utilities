using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBuilder : MonoBehaviour
{
  

    ColPriv partType = (ColPriv)0;
    Vector3Int position = Vector3Int.zero;
    List<KeyValuePair<Vector3Int, ColPriv>> shapes = new List<KeyValuePair<Vector3Int, ColPriv>>();

    GameObject ghost;
    public Material whitetrans;
    public Material bluetrans;

    void Start()
    {
        ResetGhost();
    }
    void ResetGhost()
    {
        Destroy(ghost);
        ghost = Instantiate(GameObject.Find("" + partType));
        ghost.transform.SetParent(null);
        ghost.SetActive(true);
        ghost.GetComponent<MeshRenderer>().material = bluetrans;
    }
    void Update()
    {
        ghost.transform.position = new Vector3(position.x, position.y, position.z);
        ghost.transform.rotation = Quaternion.identity;
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            partType = (ColPriv)(((int)partType + 1) % (int)ColPriv.COUNT);
            ResetGhost();
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
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

        if (Input.GetKeyDown(KeyCode.Alpha1)) { partType = ColPriv.CUBE; }          // 1: CUBE
        if (Input.GetKeyDown(KeyCode.Alpha2)) { partType = ColPriv.PX_NY_PRISM; }   // 2: PRISM
        if (Input.GetKeyDown(KeyCode.Alpha3)) { partType = ColPriv.PX_PZ_NY_TETRA; }// 3: TETRA
        if (Input.GetKeyDown(KeyCode.Alpha4)) { partType = ColPriv.PX_PY_PZ_INNER; }// 4: INNER
        if (Input.GetKeyDown(KeyCode.Alpha5)) { partType = ColPriv.PX_SLAB; }       // 5: HALFSLAB
        if (Input.GetKeyDown(KeyCode.Alpha6)) { partType = ColPriv.PX_NY_COVER; }   // 6: PRISM COVER
        if (Input.GetKeyDown(KeyCode.Alpha7)) { partType = ColPriv.PX_PZ_NY_COVER; }// 7: TETRA COVER
        if (Input.GetKeyDown(KeyCode.Alpha8)) { partType = ColPriv.PX_PZ_NY_INCOV; }// 8: INNER COVER
        if (Input.GetKeyDown(KeyCode.Alpha9)) { partType = ColPriv.X_PILLAR; }      // 9: MISC RECTANGLES
        if (Input.GetKeyDown(KeyCode.Alpha0)) { partType = ColPriv.X_RIDGE_NY; }    // 0: CENTERED RIDGE



        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            for (int i = 0; i < shapes.Count; ++i)
                if (shapes[i].Key.Equals(position))
                {
                    shapes.RemoveAt(i);
                    break;
                }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var t in shapes)
                if (t.Key == position)
                    return;
            shapes.Add(new KeyValuePair<Vector3Int, ColPriv>(position, partType));
            GameObject g = Instantiate(ghost);
            g.GetComponent<MeshRenderer>().material = whitetrans;
        }
    }
}

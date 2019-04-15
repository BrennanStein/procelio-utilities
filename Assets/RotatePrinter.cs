using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePrinter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        printVariants(ColPriv.NX_NY_PRISM);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void printVariants(ColPriv colp)
    {
        Debug.Log(colp+" "+CollideData.GetRotation(colp, 0x4)+" "+ CollideData.GetRotation(colp, 0x8) + " " + CollideData.GetRotation(colp, 0xC));
    }
}

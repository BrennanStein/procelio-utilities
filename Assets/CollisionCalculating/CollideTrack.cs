using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideTrack : MonoBehaviour {

    public bool col = false;
	// Use this for initialization
	void Start () {
		
	}
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRU");
        col = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

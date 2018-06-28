using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideTrack : MonoBehaviour {

    public bool col = false;
  //  public bool EXIT = false
	// Use this for initialization
	void Start () {
		
	}
    void OnTriggerEnter(Collider other)
    {

        col = true;
  //      EXIT = false;
    }
    void OnTriggerExit(Collider other)
    {
        col = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

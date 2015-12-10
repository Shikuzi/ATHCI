using UnityEngine;
using System.Collections;

public class HmdController : MonoBehaviour {

	// Use this for initialization
	void Start() {
	
	}
	
	// Update is called once per frame
	void Update() {
        if(Input.GetKeyDown("space")) {
            UnityEngine.VR.InputTracking.Recenter();
        }
	}
}

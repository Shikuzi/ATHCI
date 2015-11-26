using UnityEngine;

public class MovementController : MonoBehaviour {
    private bool mCollides;
    private Vector3 mScreenPoint;
    private Vector3 mOffset;

	// Use this for initialization
	void Start() {
	
	}

    void OnMouseDown() {
        var camera = Camera.allCameras[0];

        mScreenPoint = camera.WorldToScreenPoint(
                gameObject.transform.position);
        mOffset = gameObject.transform.position - 
            camera.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, 
                Input.mousePosition.y, 
                mScreenPoint.z
            ));
	}
	
    void OnMouseUp() {
        var camera = Camera.allCameras[0];

        var rend = GetComponent<Renderer>();
        if(mCollides) {
            // Turn red
            rend.material.color = Color.red;
        } else {
            // Turn off color
            rend.material.color = Color.black;
        }
    }

	void OnMouseDrag() {
        var camera = Camera.allCameras[0];

        Vector3 cursorPoint = new Vector3(
                Input.mousePosition.x, 
                Input.mousePosition.y, 
                mScreenPoint.z);
        Vector3 cursorPosition = camera.ScreenToWorldPoint(cursorPoint) + 
            mOffset;
        transform.position = cursorPosition;
	}

    void OnCollisionEnter(Collision collision) {
        Debug.Log("Collision enter!");
        mCollides = true;
    }

    void OnCollisionExit(Collision collision) {
        Debug.Log("Collision exit!");
        mCollides = false;
    }
}

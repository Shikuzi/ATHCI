using UnityEngine;

public class MovementController : MonoBehaviour {
    private bool mCollides;
    private Vector3 mScreenPoint;
    private Vector3 mOffset;

    private enum Highlight {
        None,
        Collision,
        Select
    }

    private Highlight mHighlight = Highlight.None;

    private void SetHighlight(Highlight hl) {
        var rends = GetComponentsInChildren<Renderer>();
        foreach(var rend in rends) {
            foreach(var mat in rend.materials) {
                // Remove old color
                switch(mHighlight) {
                case Highlight.Collision:
                    mat.color -= new Color(0.5f, 0.0f, 0.0f);
                    break;
                case Highlight.Select:
                    mat.color -= new Color(0.0f, 0.3f, 0.3f);
                    break;
                }

                // Add new color
                switch(hl) {
                case Highlight.Collision:
                    mat.color += new Color(0.5f, 0.0f, 0.0f);
                    break;
                case Highlight.Select:
                    mat.color += new Color(0.0f, 0.3f, 0.3f);
                    break;
                }
            }
        }
        
        mHighlight = hl;
    } 

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
        SetHighlight(Highlight.Select);
	}

    void OnMouseUp() {
        if(mCollides) {
            // Turn red
            SetHighlight(Highlight.Collision);
        } else {
            // Turn off color
            SetHighlight(Highlight.None);
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

    void OnTriggerEnter(Collider collider) {
        mCollides = true;
    }

    void OnTriggerExit(Collider collider) {
        mCollides = false;
    }
}

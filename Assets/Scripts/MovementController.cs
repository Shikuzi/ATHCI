using System;
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
                    mat.color -= new Color(0.0f, 0.5f, 0.5f);
                    break;
                }

                // Add new color
                switch(hl) {
                case Highlight.Collision:
                    mat.color += new Color(0.5f, 0.0f, 0.0f);
                    break;
                case Highlight.Select:
                    mat.color += new Color(0.0f, 0.5f, 0.5f);
                    break;
                }
            }
        }
        
        mHighlight = hl;
    } 

    private float SnapToGrid(float f, float grid) {
        return (float)Math.Floor((double)((f + grid / 2) / grid)) * grid;
    }

	// Use this for initialization
	void Start() {
	
	}

    void OnPointingStart() {
        Debug.Log("OnPointingStart");
        SetHighlight(Highlight.Select);
    }

    void OnPointingStop() {

    }

    void OnPointingMove() {

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
        mOffset.y = 0;
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
        Vector3 worldpos = camera.ScreenToWorldPoint(cursorPoint);
        Vector3 restricted = new Vector3(
            worldpos.x,
            transform.position.y,
            worldpos.z
        );
        Vector3 cursorPosition = restricted + mOffset;
        cursorPosition.x = SnapToGrid(cursorPosition.x, 0.5f);
        cursorPosition.z = SnapToGrid(cursorPosition.z, 0.5f);
        transform.position = cursorPosition;
	}

    void OnTriggerEnter(Collider collider) {
        mCollides = true;
    }

    void OnTriggerExit(Collider collider) {
        mCollides = false;
    }
}

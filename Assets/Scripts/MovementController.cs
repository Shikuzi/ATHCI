using System;
using UnityEngine;

public class MovementController : MonoBehaviour {
    public bool mCollides;

    // For use with mouse
    private Vector3 mScreenPoint;
    private Vector3 mOffset;

    // For use with gesture controller
    private Vector3 mOrigin;
    private Vector3 mHitPosition;

    public float kGridSize = 0.5f;

    public enum Highlight {
        None,
        Hover,
        Select,
        Collision,
    }

    public Highlight mHighlight = Highlight.None;

    private void SetHighlight(Highlight hl) {
        var rends = GetComponentsInChildren<Renderer>();
        foreach(var rend in rends) {
            foreach(var mat in rend.materials) {
                // Remove old color
                switch(mHighlight) {
                case Highlight.Hover:
                    mat.color -= new Color(0.7f, 0.7f, 0.7f);
                    break;
                case Highlight.Select:
                    mat.color -= new Color(0.0f, 0.7f, 0.7f);
                    break;
                case Highlight.Collision:
                    mat.color -= new Color(0.7f, 0.0f, 0.0f);
                    break;
                }

                // Add new color
                switch(hl) {
                case Highlight.Hover:
                    mat.color += new Color(0.7f, 0.7f, 0.7f);
                    break;
                case Highlight.Select:
                    mat.color += new Color(0.0f, 0.7f, 0.7f);
                    break;
                case Highlight.Collision:
                    mat.color += new Color(0.7f, 0.0f, 0.0f);
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

    void Update() {
        if(mHighlight != Highlight.Select && mHighlight != Highlight.Hover) {
            if(mCollides) {
                // Turn red
                SetHighlight(Highlight.Collision);
            } else {
                // Turn off color
                SetHighlight(Highlight.None);
            }
        }
    }

    void OnPointingStart() {
        SetHighlight(Highlight.Hover);
    }

    void OnGrabbingStart() {
        SetHighlight(Highlight.Select);
        mOrigin = GestureController.Origin;
        mHitPosition = GestureController.HitPosition;
        // mOffset = mOrigin - mHitPosition;
    }

    void OnPointingStop() {
        SetHighlight(Highlight.None);
    }

    void OnGrabbingStop() {
        SetHighlight(Highlight.Hover);
    }

    private void SnapToWall(int wall) {
        var pos = gameObject.transform.position;
        var angle = gameObject.transform.localRotation.eulerAngles;
        float prevangle = angle.y;

        float w2 = Walls.Width / 2, h2 = Walls.Height / 2;

        switch(wall) {
        case Walls.North:
            pos.z = h2;
            angle.y = 0;
            break;
        case Walls.East:
            pos.x = w2;
            angle.y = 90;
            break;
        case Walls.South:
            pos.z = -h2;
            angle.y = 180;
            break;
        case Walls.West:
            pos.x = -w2;
            angle.y = 270;
            break;
        }

        gameObject.transform.position = pos;
        gameObject.transform.localRotation = Quaternion.Euler(angle);
    }

    void OnPointingMove() {
        var newpos = gameObject.transform.position;

        var wall = Walls.GetWall(GestureController.Origin,
                GestureController.Direction);

        if(wall == Walls.North || wall == Walls.South) {
            // O + tD = hitpos.z
            // tD = hitpos.z - O
            // t = (hitpos.z - O) / D

            var z = (wall == Walls.North ? 1 : -1) * Walls.Height / 2;

            var t = (z - GestureController.Origin.z) /
                    GestureController.Direction.z;
            newpos.x = GestureController.Origin.x +
                    t * GestureController.Direction.x;
            // newpos.x -= mOffset.x;
        } else {
            var x = (wall == Walls.East ? 1 : -1) * Walls.Height / 2;

            var t = (x - GestureController.Origin.x) /
                    GestureController.Direction.x;
            newpos.z = GestureController.Origin.z +
                    t * GestureController.Direction.z;
            // newpos.z -= mOffset.z;
        }

        newpos.x = SnapToGrid(newpos.x, kGridSize);
        newpos.z = SnapToGrid(newpos.z, kGridSize);

        gameObject.transform.position = newpos;

        SnapToWall(wall);
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
        SetHighlight(Highlight.None);
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
        cursorPosition.x = SnapToGrid(cursorPosition.x, kGridSize);
        cursorPosition.z = SnapToGrid(cursorPosition.z, kGridSize);
        transform.position = cursorPosition;

        var ray = camera.ScreenPointToRay(Input.mousePosition);
        var wall = Walls.GetWall(ray.origin, ray.direction);
        SnapToWall(wall);
	}

    void OnTriggerEnter(Collider collider) {
        mCollides = true;
    }

    void OnTriggerStay(Collider collider) {
        mCollides = true;
    }

    void OnTriggerExit(Collider collider) {
        mCollides = false;
    }
}

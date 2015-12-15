﻿using UnityEngine;
using System.Collections;
using Leap;

public class GestureController : MonoBehaviour {
    enum Mode {
        Search,
        Hover,
        Active
    };

    Controller controller;
    // Use this for initialization
    bool inMenu;
    public UIController ui;
    // enum Mode { Grab, GrabReleased, SwipeGesture,  None };
    Mode mode;
    GameObject currentobj;
    GameObject pointingRay;
    GameObject cylinder;

    HandController handController;

    public static Vector3 Origin { get; set; }
    public static Vector3 Direction { get; set; }
    public static Vector3 HitPosition { get; set; }

    public Color kBeamSearchColor = new Color(1,1,1,0.25f);
    public Color kBeamHoverColor  = new Color(1,1,1);
    public Color kBeamActiveColor = new Color(0.2f, 0.8f, 0.7f);
    public float kGrabTreshold = 0.7f;

    private void SetMode(Mode m) {
        mode = m;
        var mat = cylinder.GetComponent<Renderer>().material;
        switch(mode) {
        case Mode.Search:
            mat.color = kBeamSearchColor;
            break;
        case Mode.Hover:
            mat.color = kBeamHoverColor;
            break;
        case Mode.Active:
            mat.color = kBeamActiveColor;
            break;
        }
    }

    private void SendMessageTo(GameObject obj, string msg) {
        obj.BroadcastMessage(msg, null, SendMessageOptions.DontRequireReceiver);
    }

    private void StartPointing(GameObject obj) {
        currentobj = obj;
        SendMessageTo(obj, "OnPointingStart");
        SetMode(Mode.Hover);
    }

    private void StartGrabbing() {
        SendMessageTo(currentobj, "OnGrabbingStart");
        SetMode(Mode.Active);
    }

    private void MovePointing() {
        SendMessageTo(currentobj, "OnPointingMove");
    }

    private void StopPointing() {
        if(currentobj != null) {
            SendMessageTo(currentobj, "OnPointingStop");
            currentobj = null;
        }
        SetMode(Mode.Search);
    }

    private void StopGrabbing() {
        SendMessageTo(currentobj, "OnGrabbingStop");
        SetMode(Mode.Hover);
    }

    private bool IsRoom(GameObject obj) {
        return obj.GetComponent<MovementController>() == null;
    }

    private void Swap<T>(ref T t1, ref T t2) {
        T t = t1;
        t1 = t2;
        t2 = t;
    }

    private void SetPointingRayEnabled(bool e) {
        pointingRay.SetActive(e);
    }

    private void UpdatePointingRay() {
        Debug.DrawRay(Origin, HitPosition - Origin, Color.red);

        var scale = pointingRay.transform.localScale;
        scale.y = (HitPosition - Origin).magnitude;

        pointingRay.transform.localScale = scale;
        pointingRay.transform.position = Origin;
        pointingRay.transform.rotation = 
            Quaternion.FromToRotation(Vector3.up, HitPosition - Origin);
    }

	void Start () {
        controller = new Controller();
        controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        controller.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f);
        controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 20.0f);
        controller.Config.Save();

        handController = GameObject.Find("HandController").
            GetComponent("HandController") as HandController;
        float hcscale = handController.gameObject.transform.localScale.x;

        pointingRay = new GameObject("Pointing Ray");
        cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.localPosition = new Vector3(0, 0.5f, 0);
        cylinder.transform.localScale = new Vector3(
                0.01f * hcscale, 
                0.5f, 
                0.01f * hcscale
        );
        cylinder.transform.parent = pointingRay.transform;
        var mat = cylinder.GetComponent<Renderer>().material;
        mat.shader = Shader.Find("Unlit/Beam");
        mat.color = kBeamSearchColor;

        pointingRay.transform.position = new Vector3(0, 0, 0);


        inMenu = false;

    }

	// Update is called once per frame
	void Update () {
        Frame frame = controller.Frame();
        HandList hands = frame.Hands;
        Hand rightHand = frame.Hands.Rightmost;
        Hand leftHand = frame.Hands.Leftmost;
        Pointable knownPointable = rightHand.Pointables.Frontmost;

        // For some reason the Leap Motion can't tell right from left when
        // head mounted ???
        if(handController.isHeadMounted) {
            Swap(ref leftHand, ref rightHand);
        }

        if(frame.Hands.Count < 2) {
            SetPointingRayEnabled(false);
            SetMode(Mode.Search);
        } else {
            SetPointingRayEnabled(true);

            Finger indexFinger = rightHand.Fingers.FingerType(
                    Finger.FingerType.TYPE_INDEX)[0];

            var tipPosition = indexFinger.TipPosition.ToUnityScaled(false);
            var temporigin = handController.transform.TransformPoint(tipPosition);
            var tempdirection = handController.transform.TransformDirection(
                   indexFinger.Direction.ToUnity());

            RaycastHit hit;

            if(Physics.Raycast(temporigin, tempdirection, out hit) && 
                    indexFinger.IsExtended) {
                // Threshold below which not to update (prevent glitchiness)
                if(hit.distance > 0.1) {
                    Origin = temporigin;
                    Direction = tempdirection;
                    HitPosition = hit.point;
                    UpdatePointingRay();

                    if(mode != Mode.Active) {
                        var newobj = hit.collider.gameObject;

                        if(!IsRoom(newobj) && currentobj != newobj) {
                            StartPointing(newobj);
                        } else if(IsRoom(newobj)) {
                            StopPointing();
                        }
                    }
                }
            } else {
                StopPointing();
                SetPointingRayEnabled(false);
                SetMode(Mode.Search);
            }
        }

        if(mode == Mode.Hover && leftHand.GrabStrength >= kGrabTreshold) {
            StartGrabbing();
        } else if(mode == Mode.Active && 
                leftHand.GrabStrength < kGrabTreshold) {
            StopGrabbing();
        }

        if(mode == Mode.Active) {
            MovePointing();
        }

        /*
        if(mode != Mode.Grab && leftHand.GrabStrength == 1) {
            if (currentobj != null) {
                GrabPointing(currentobj);
            }
            mode = Mode.Grab;
        }

        if(mode == Mode.Grab && leftHand.GrabStrength < 1) {
            mode = Mode.GrabReleased;
            if(currentobj != null) {
                StopPointing();
            }
        }

        if(mode == Mode.Grab) {
            if(currentobj != null) {
                MovePointing(currentobj);
            }
        }*/


        /*GestureList gestures = frame.Gestures();
        foreach(Gesture g in gestures)
        {
            if(g.Type == Gesture.GestureType.TYPE_SWIPE)
            {
                mode = Mode.SwipeGesture;
                Debug.Log("SwipeMode");
                SwipeGesture swipe = new SwipeGesture(g);
                float speed = swipe.Speed;
                if (swipe.Direction.y < -0.8)
                    ui.setDropDown(true);
                else if (swipe.Direction.y > 0.8)
                    ui.setDropDown(false);
                else if (swipe.Direction.x < 0)
                    ui.scrollTo(-speed, 0f);
                else
                    ui.scrollTo(speed, 0f);
            }
        }*/
	}
}

﻿using UnityEngine;
using System.Collections;
using Leap;

public class GestureController : MonoBehaviour {
    Controller controller;
    // Use this for initialization
    bool inMenu;
    public UIController ui;
    enum Mode { Grab, GrabReleased, SwipeGesture,  None};
    Mode mode;
    GameObject currentobj;

    public static Vector3 Origin { get; set; }
    public static Vector3 Direction { get; set; }
    public static Vector3 HitPosition { get; set; }

    private void SendMessageTo(GameObject obj, string msg) {
        obj.BroadcastMessage(msg, null, SendMessageOptions.DontRequireReceiver);
    }

    private void StartPointing(GameObject obj) {
        SendMessageTo(obj, "OnPointingStart");
    }

    private void GrabPointing(GameObject obj)
    {
        SendMessageTo(obj, "OnGrabbingStart");
    }

    private void MovePointing(GameObject obj)
    {
        SendMessageTo(obj, "OnPointingMove");
    }

    private void StopPointing() {
        SendMessageTo(currentobj, "OnPointingStop");
    }

	void Start () {
        controller = new Controller();
        controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        controller.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f);
        controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 20.0f);
        controller.Config.Save();
        inMenu = false;
    }

	// Update is called once per frame
	void Update () {
        Frame frame = controller.Frame();
        HandList hands = frame.Hands;
        Hand rightHand = frame.Hands.Rightmost;
        Hand leftHand = frame.Hands.Leftmost;
        Pointable knownPointable = rightHand.Pointables.Frontmost;
        Vector handCenter = rightHand.PalmPosition;

        Origin = handCenter.ToUnityScaled(false);
        Direction = knownPointable.Direction.ToUnity();

        RaycastHit hit;

        if(Physics.Raycast(Origin, Direction, out hit)) {
            HitPosition = hit.point;
            currentobj = hit.collider.gameObject;
            StartPointing(currentobj);
        }

        if(mode != Mode.Grab && leftHand.GrabStrength == 1) {
            if (currentobj != null)
                GrabPointing(currentobj);
            mode = Mode.Grab;
        }

        if(mode == Mode.Grab && leftHand.GrabStrength < 1) {
            mode = Mode.GrabReleased;
        }

        if(mode == Mode.Grab) {
            if(currentobj != null) {
                MovePointing(currentobj);
            }
            //Debug.Log("Moving furniture..");
        } else if(mode == Mode.GrabReleased) {
            Debug.Log("Released furniture");
            if(currentobj != null) {
                StopPointing();
            }
        }


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
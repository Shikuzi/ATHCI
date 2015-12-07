﻿using UnityEngine;
using System.Collections;
using Leap;

public class GestureController : MonoBehaviour {
    Controller controller;
    // Use this for initialization
    bool inMenu;
    public UIController ui;
    enum Mode { Grab, SwipeGesture, None};
    Mode mode;

    public static Vector3 Origin { get; set; }
    public static Vector3 Direction { get; set; }
    public static Vector3 HitPosition { get; set; }

    private void StartPointing(GameObject obj) {
        obj.BroadcastMessage("OnPointingStart");
    }

    private void StopPointing() {
        var colliders = GameObject.FindObjectsOfType<Collider>() as Collider[];
        foreach(var col in colliders) {
            col.gameObject.BroadcastMessage("OnPointingStop");
        }
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

        Debug.Log("FINGER ID: " + knownPointable.Id);

        Direction = knownPointable.Direction.ToUnity();
        Debug.Log(knownPointable.Direction.ToUnity());

        Origin = handCenter.ToUnityScaled(false);
        Debug.Log(handCenter.ToUnityScaled(false));

        RaycastHit hit;

        if(Physics.Raycast(Origin, Direction, out hit)) {
            HitPosition = hit.point;
            StartPointing(hit.collider.gameObject);
        }

        GestureList gestures = frame.Gestures();
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
        }
	}
}

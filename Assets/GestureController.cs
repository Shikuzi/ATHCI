using UnityEngine;
using System.Collections;
using Leap;

public class GestureController : MonoBehaviour {
    Controller controller;
    // Use this for initialization
    bool inMenu;
    public UIController ui;
    enum Mode { Grab, GrabReleased, SwipeGesture,  None};
    Mode mode;

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

        Vector3 originHandcenter, directionFinger;

        Debug.Log("FINGER ID: " + knownPointable.Id);

        directionFinger = knownPointable.Direction.ToUnity();
        Debug.Log(knownPointable.Direction.ToUnity());

        originHandcenter = handCenter.ToUnityScaled(false);
        Debug.Log(handCenter.ToUnityScaled(false));

        RaycastHit hit;

        if(Physics.Raycast(originHandcenter, directionFinger, out hit)) {
            StartPointing(hit.collider.gameObject);
        }

        if (leftHand.GrabStrength == 1)
            mode = Mode.Grab;

        if (mode == Mode.Grab && leftHand.GrabStrength < 1)
            mode = Mode.GrabReleased;

        if (mode == Mode.Grab)
            Debug.Log("Moving furniture..");
        else if (mode == Mode.GrabReleased)
            Debug.Log("Released furniture");


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

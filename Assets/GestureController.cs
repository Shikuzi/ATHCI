using UnityEngine;
using System.Collections;
using Leap;

public class GestureController : MonoBehaviour {
    Controller controller;
    // Use this for initialization
    bool inMenu;
    public UIController ui;
    enum Mode { Grab, SwipeGesture, None};
    Mode mode;
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
        Pointable knownPointable = leftHand.Pointables.Frontmost;

        Debug.Log("pointable: " + knownPointable.ToString());

        if (rightHand.GrabStrength == 1)
        {
            mode = Mode.Grab;

            Vector handCenter = rightHand.PalmPosition;

            float x = handCenter.x;
            float y = handCenter.y;
            float z = handCenter.z;
            Debug.Log("GrabMode : x=" + x + " y=" + y + " z=" + z);

            float pitch = rightHand.Direction.Pitch;
            float yaw = rightHand.Direction.Yaw;
            float roll = rightHand.PalmNormal.Roll;
            //Debug.Log("GrabMode : pitch="+pitch + " yaw=" + yaw + " roll=" + roll);
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

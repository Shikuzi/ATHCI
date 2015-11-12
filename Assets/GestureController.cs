using UnityEngine;
using System.Collections;
using Leap;

public class GestureController : MonoBehaviour {
    Controller controller;
    // Use this for initialization
    bool inMenu;
    public UIController ui;
	void Start () {
        controller = new Controller();
        controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        controller.Config.SetFloat("Gesture.Swipe.MinLength", 150.0f);
        controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 20.0f);
        controller.Config.Save();
        inMenu = false;
    }
	
	// Update is called once per frame
	void Update () {
        Frame frame = controller.Frame();
        GestureList gestures = frame.Gestures();
        foreach(Gesture g in gestures)
        {
            if(g.Type == Gesture.GestureType.TYPE_SWIPE)
            {
                SwipeGesture swipe = new SwipeGesture(g);
                if (swipe.Direction.y < -0.8)
                    ui.setDropDown(true);
                else if (swipe.Direction.y > 0.8)
                    ui.setDropDown(false);
            
                float speed = swipe.Speed;
                if (swipe.Direction.x < 0)
                    ui.scrollTo(-speed, 0f);
                else
                    ui.scrollTo(speed, 0f);
            }
        }
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {
    Animator animationController;
    public bool showDropDown = false;
    public ScrollRect scroll;
    
	// Use this for initialization
	void Start () {
        animationController = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.F))
        {
            showDropDown = showDropDown ? false : true;
            animationController.SetBool("ShowDropDown", showDropDown);
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            scroll.velocity = new Vector2(-125f, 0f);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            scroll.velocity = new Vector2(125f, 0f);
        }
    }
}

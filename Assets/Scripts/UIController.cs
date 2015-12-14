using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {
    Animator animationController;
    public static bool showDropDown = false;
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
        /*
        if(Input.GetKey(KeyCode.RightArrow))
        {
            scrollTo(-150f, 0f);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            scrollTo(150f, 0f);
        }µ*/
    }

    public void setDropDown(bool v)
    {
        animationController.SetBool("ShowDropDown", v);
        showDropDown = v;
    }
    /*
    public void scrollTo(float x, float y)
    {
        scroll.velocity = new Vector2(x, 0f);
    }*/
}

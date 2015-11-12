using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SnapToItem : MonoBehaviour {
    public UIController controller;
    public RectTransform content;
    public RectTransform center;
    private ScrollRect scroll;
	// Use this for initialization
	void Start () {
        init();
	}

    void init()
    {
        scroll = GetComponent<ScrollRect>();
    }
	
	// Update is called once per frame
	void Update () {
	    if(controller.showDropDown && Mathf.Abs(scroll.velocity.x) < 100)
        {
            float distBetween = Mathf.Abs(content.GetChild(0).gameObject.GetComponent<RectTransform>().position.x - content.GetChild(1).gameObject.GetComponent<RectTransform>().position.x); // +  2 * padding
            int min = -1;
            float minDist = 99999;
            
            for(int i = 0; i < content.childCount; ++i)
            {
                GameObject b = content.GetChild(i).gameObject;
                float curDist = Mathf.Abs(b.GetComponent<RectTransform>().position.x - center.position.x);
                if (curDist < minDist)
                {
                    minDist = curDist;
                    min = i;
                }
            }
            
            RectTransform cpos = content.GetComponent<RectTransform>();
            float newX = (min * -distBetween) - (distBetween/2);
            cpos.anchoredPosition = new Vector2(newX, cpos.anchoredPosition.y);
        }
	}
}

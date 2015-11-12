using UnityEngine;
using System.Collections;

public class ContentManager : MonoBehaviour {
    public GameObject itemPrefab;

	// Use this for initialization
	void Start () {
        loadContent();
    }

    // Load all the models
    void loadContent()
    {
        for (int i = 0; i < 10; ++i)
        {
            GameObject newItem = Instantiate(itemPrefab);
            newItem.transform.SetParent(this.transform);
        }
    }
}

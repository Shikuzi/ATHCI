using UnityEngine;
using UnityEditor;
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
        Object[] assets = Resources.LoadAll("Furniture/");
        foreach (Object asset in assets)
        {   
            if(PrefabUtility.GetPrefabType(asset) != PrefabType.None)
            {
                Debug.Log(asset.name);
            }
            else
            {
                Debug.Log(asset.name + " is not a prefab.");
            }
        }
    }
}

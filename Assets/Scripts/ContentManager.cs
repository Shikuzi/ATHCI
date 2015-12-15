using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class ContentManager : MonoBehaviour {
    public GameObject itemPrefab;
    private static ArrayList itemArray;
    // Use this for initialization
    void Start () {
        loadContent();
    }

    // Load all the models
    void loadContent()
    {
        itemArray = new ArrayList();
        UnityEngine.Object[] assets = Resources.LoadAll("Furniture/");
        // UnityEngine.Object[] assets = {};


        foreach (UnityEngine.Object asset in assets)
        {
            if (PrefabUtility.GetPrefabType(asset) == PrefabType.Prefab) {
                Texture2D texture = null;
                // while (texture == null) {
                    texture = AssetPreview.GetAssetPreview(asset);
                //    System.Threading.Thread.Sleep(15);
                // }
                itemArray.Add(asset);
                int items = itemArray.Count;
                int row = (items / 7);
                int col = (items % 7) - 1;

                
                transform.GetChild(row).transform.GetChild(col).gameObject.GetComponent<Button>().enabled = true;
                transform.GetChild(row).transform.GetChild(col).gameObject.GetComponent<Button>().GetComponent<ButtonInformation>().setInformation(row, col);
                transform.GetChild(row).transform.GetChild(col).gameObject.GetComponent<Image>().enabled = true;
                // transform.GetChild(row).transform.GetChild(col).gameObject.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                break;
            }
            else
            {
                Debug.Log(asset.name + " is not a prefab.");
            }
        }
    }

    public static void spawnObject(int row, int col)
    {
        int item = row * 7 + col;
        GameObject obj = Instantiate(itemArray[item] as GameObject);
        obj.transform.position = new Vector3(0f, 0f, 2f);
        obj.AddComponent<MovementController>();
        GameObject.FindObjectOfType<UIController>().setDropDown(false);
    }
}

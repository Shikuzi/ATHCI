using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.IO;

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
        Hashtable textureMap = new Hashtable() ;
        itemArray = new ArrayList();
        UnityEngine.Object[] assets = Resources.LoadAll("Furniture/");
        // UnityEngine.Object[] assets = {};


        
        //load all Texture2D files in Resources/Images folder
        UnityEngine.Object[] textures = Resources.LoadAll("Images");
        foreach (UnityEngine.Object texture in textures)
            textureMap.Add(texture.name, texture);

        //load all prefabs and add them to the menu
        foreach (UnityEngine.Object asset in assets)
        {
            if (PrefabUtility.GetPrefabType(asset) == PrefabType.Prefab)
            {
                Texture2D t = null;
                //int counter = 0;
                
                t = LoadImage(asset.name);
                if (t == null) {
                    Debug.Log("Please provide a .png file with the same name as the prefab");
                    continue;
                }

                itemArray.Add(asset);
                int items = itemArray.Count;
                int row = (items / 7);
                int col = (items % 7) - 1;

                
                transform.GetChild(row).transform.GetChild(col).gameObject.GetComponent<Button>().enabled = true;
                transform.GetChild(row).transform.GetChild(col).gameObject.GetComponent<Button>().GetComponent<ButtonInformation>().setInformation(row, col);
                transform.GetChild(row).transform.GetChild(col).gameObject.GetComponent<Image>().enabled = true;
                Debug.Log(t.width + ", " + t.height);
                transform.GetChild(row).transform.GetChild(col).gameObject.GetComponent<Image>().sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
            }
        }
    }

    public static void spawnObject(int row, int col)
    {
        Debug.Log("HERE!");
        int item = row * 7 + col;
        GameObject obj = Instantiate(itemArray[item] as GameObject);
        obj.transform.position = new Vector3(0f, 0f, 2f);
        obj.AddComponent<MovementController>();
        GameObject.FindObjectOfType<UIController>().setDropDown(false);
    }

    public static Texture2D LoadImage(string imageName)
    {
        string filePath = Application.dataPath + "/Resources/Furniture/Images/" + imageName + ".png";

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        else
            return null;

        return tex;
    }

    private static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        /*float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);*/
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }
}

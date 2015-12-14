using UnityEngine;
using System.Collections;

public class ButtonInformation : MonoBehaviour {
    private int row;
    private int col;
	// Use this for initialization
	void Start () {
        row = 0;
        col = 0;
	}

    public void setInformation(int row, int col)
    {
        this.row = row;
        this.col = col;
        Debug.Log(this.row + "," + this.col);
    }

    public void click()
    {
        ContentManager.spawnObject(row, col);
    }
}

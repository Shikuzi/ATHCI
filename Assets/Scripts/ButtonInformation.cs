using UnityEngine;
using System.Collections;

public class ButtonInformation : MonoBehaviour {
    private int row;
    private int col;
	// Use this for initialization
    public void setInformation(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void click()
    {
        ContentManager.spawnObject(row, col);
    }
}

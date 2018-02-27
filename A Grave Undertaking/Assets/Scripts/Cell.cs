using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour {

    public Color OriginalColor;
    public enum hiddenValue { Body, Bomb, Empty };
    public hiddenValue hidden;
    public bool uncovered = false;
    public static int bodyTileCount = 24;
    public static int bombTileCount = 8;
    public static int EmptyTileCount = 32; // changed it because I am lazy - didn't want to filter out the corner 6 tiles


    void Start ()
    {
        OriginalColor = GetComponent<Image>().color;
        Debug.Log("Body: " + bodyTileCount);
        Debug.Log("Bomb: " + bombTileCount);
        Debug.Log("Empty: " + EmptyTileCount);
        int randRoll = Random.Range(1, bodyTileCount+bombTileCount+EmptyTileCount);
        Debug.Log("Rand: " + randRoll);
        if(randRoll <= bodyTileCount)
        {
            hidden = hiddenValue.Body;
            bodyTileCount--;
        }
        if(randRoll > bodyTileCount && randRoll <= bodyTileCount+bombTileCount)
        {
            hidden = hiddenValue.Bomb;
            bombTileCount--;
        }
        if(randRoll > bodyTileCount+bombTileCount)
        {
            hidden = hiddenValue.Empty;
            EmptyTileCount--;
        }   
	}

    public Color NormalColor()
    {
        if (uncovered) return new Color(0, 0, 0, 255);
        return new Color(255, 255, 255, 200);
    }
}

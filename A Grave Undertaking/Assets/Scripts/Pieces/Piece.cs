using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour {

    public int xCoord;
    public int yCoord;
    public bool turnIsOver;
    public bool canResurrect;
    public Sprite activeSprite;
    public Sprite deactiveSprite;

    void Start()
    {
        UpdateCoordinates();
    }

    void UpdateCoordinates()
    {
        // Get x and y of current occupied tile
    }

    void Update()
    {
        if(turnIsOver)
        {
            this.GetComponent<Image>().sprite = deactiveSprite;
        }
        else
        {
            this.GetComponent<Image>().sprite = activeSprite;
        }
    }
}

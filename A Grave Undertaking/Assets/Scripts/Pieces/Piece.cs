using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    public int xCoord;
    public int yCoord;
    public bool turnIsOver;
    public bool canResurrect;

    void Start()
    {
        UpdateCoordinates();
    }

    void UpdateCoordinates()
    {
        // Get x and y of current occupied tile
    }
}

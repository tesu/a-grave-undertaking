using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Pawn {

	void Start ()
    {
        maxMovement = 100;
        canResurrect = true;
        turnIsOver = true;
    }
}

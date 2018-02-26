using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Pawn {

	void Start ()
    {
        maxMovement = 1;
        canResurrect = true;
        turnIsOver = true;
    }
}

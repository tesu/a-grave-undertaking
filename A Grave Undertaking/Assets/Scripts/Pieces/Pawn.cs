﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {

    public int maxMovement;

	void Start ()
    {
        maxMovement = 1;
        canResurrect = false;
        turnIsOver = true;
	}
}

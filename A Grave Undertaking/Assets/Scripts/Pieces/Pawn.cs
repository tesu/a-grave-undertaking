using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece {

    public int maxMovement;
    public Sprite RedPawn;
    public Sprite RedPawn_H;
    public Sprite BluePawn;
    public Sprite BluePawn_H;

    void Start ()
    {
        maxMovement = 1;
        canResurrect = false;
        turnIsOver = true;
        if (this.tag == "Player1")
        {
            activeSprite = RedPawn_H;
            deactiveSprite = RedPawn;
        }
        else if (tag == "Player2") {
            activeSprite = BluePawn_H;
            deactiveSprite = BluePawn;
        }
    }
}

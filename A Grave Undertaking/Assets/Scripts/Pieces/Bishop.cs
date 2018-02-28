using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Pawn {

    public Sprite RedBishop;
    public Sprite RedBishop_H;
    public Sprite BlueBishop;
    public Sprite BlueBishop_H;

    void Start ()
    {
        maxMovement = 100;
        canResurrect = true;
        turnIsOver = true;
        if (this.tag == "Player1")
        {
            activeSprite = RedBishop_H;
            deactiveSprite = RedBishop;
        }
        else
        {
            activeSprite = BlueBishop_H;
            deactiveSprite = BlueBishop;
        }
    }
}

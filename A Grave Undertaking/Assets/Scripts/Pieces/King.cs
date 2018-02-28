using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Pawn {

    public Sprite RedKing;
    public Sprite RedKing_H;
    public Sprite BlueKing;
    public Sprite BlueKing_H;

    void Start ()
    {
        maxMovement = 1;
        canResurrect = true;
        turnIsOver = true;
        if(this.tag == "Player1")
        {
            activeSprite = RedKing_H;
            deactiveSprite = RedKing;
        }
        else if (tag == "Player2")
        {
            activeSprite = BlueKing_H;
            deactiveSprite = BlueKing;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece {

    public Sprite RedKnight;
    public Sprite RedKnight_H;
    public Sprite BlueKnight;
    public Sprite BlueKnight_H;

    void Start ()
    {
        canResurrect = false;
        turnIsOver = true;
        if (this.tag == "Player1")
        {
            activeSprite = RedKnight_H;
            deactiveSprite = RedKnight;
        }
        else
        {
            activeSprite = BlueKnight_H;
            deactiveSprite = BlueKnight;
        }
    }
}

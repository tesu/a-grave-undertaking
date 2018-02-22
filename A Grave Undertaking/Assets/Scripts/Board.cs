using System.Collections.Generic;
using UnityEngine;

public class Board {

    public IDictionary<GameObject, int> boardByCell;
    public IDictionary<int, GameObject> boardByCoord;
    public int[,] board;
    public bool isFull;
    public int boardSize = StaticVariables.BoardSize;
    public Coord lastCoord;

    public Board() {
        boardByCell = new Dictionary<GameObject, int>();
        boardByCoord = new Dictionary<int, GameObject>();
        board = new int[boardSize, boardSize];
        isFull = false;
        lastCoord = null;
    }

    public void AddCell(GameObject cell) {
        if (isFull) {
            return;
        }
        Coord nextCoord;
        if (lastCoord == null) {
            nextCoord = new Coord(0, 0);
        }
        else {
            nextCoord = new Coord((lastCoord.x + 1) % boardSize, lastCoord.y + (lastCoord.x == boardSize - 1 ? 1 : 0));
        }
        lastCoord = nextCoord;
        if (nextCoord.x == boardSize - 1 && nextCoord.y == boardSize - 1)
            isFull = true;
        boardByCell.Add(cell, nextCoord.ToInt());
        boardByCoord.Add(nextCoord.ToInt(), cell);
    }

    public GameObject GetCell(int x, int y) {
        Coord coord = new Coord(x, y);
        return boardByCoord[coord.ToInt()];
    }

    public int GetCellX(GameObject cell) {
        return Coord.ToCoord(GetCoord(cell)).x;
    }

    public int GetCellY(GameObject cell) {
        return Coord.ToCoord(GetCoord(cell)).y;
    }

    public int GetCoord(GameObject cell) {
        return boardByCell[cell];
    }

}

public class Coord {
    public int x;
    public int y;

    public Coord(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public int ToInt() {
        return x + StaticVariables.BoardSize * y;
    }

    public static Coord ToCoord(int a) {
        return new Coord(a % StaticVariables.BoardSize, a / StaticVariables.BoardSize);
    }
}
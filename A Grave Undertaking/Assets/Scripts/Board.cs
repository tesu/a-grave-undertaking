using System.Collections.Generic;
using UnityEngine;

public class Board {

    public IDictionary<GameObject, int> boardByCell;
    public IDictionary<int, GameObject> boardByCoord;
    public int[,] board;
    public bool isFull;
    public Coord lastCoord;

    public Board() {
        boardByCell = new Dictionary<GameObject, int>();
        boardByCoord = new Dictionary<int, GameObject>();
        board = new int[8, 8];
        isFull = false;
        lastCoord = null;
    }

    public void PlaceShip(List<int> coords) {
        for (int i = 0; i < coords.Count; i++) {
            int x = Coord.ToCoord(coords[i]).x;
            int y = Coord.ToCoord(coords[i]).y;
            for (int j = -1; j < 2; j++) {
                for (int k = -1; k < 2; k++) {
                    if (x + j < 10 && x + j >= 0 && y + k < 10 && y + k >= 0) {
                        board[x + j, y + k] = 1;
                    }
                }
            }
        }
        for (int i = 0; i < coords.Count; i++) {
            int x = Coord.ToCoord(coords[i]).x;
            int y = Coord.ToCoord(coords[i]).y;
            board[x, y] = 2;
        }
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
            nextCoord = new Coord((lastCoord.x + 1) % 10, lastCoord.y + (lastCoord.x == 9 ? 1 : 0));
        }
        lastCoord = nextCoord;
        if (nextCoord.x == 9 && nextCoord.y == 9)
            isFull = true;
        boardByCell.Add(cell, nextCoord.ToInt());
        boardByCoord.Add(nextCoord.ToInt(), cell);
    }

    public GameObject GetCell(int x, int y) {
        Coord coord = new Coord(x, y);
        return boardByCoord[coord.ToInt()];
    }

    public int GetCoord(GameObject cell) {
        return boardByCell[cell];
    }

    public int GetRight(Coord coord, int of) {
        int right = coord.GetRight(of);
        if (right != -1) {
            Coord rightC = Coord.ToCoord(right);
            if (board[rightC.x, rightC.y] > 0) {
                return -1;
            }
        }
        return right;
    }

    public int GetLeft(Coord coord, int of) {
        int left = coord.GetLeft(of);
        if (left != -1) {
            Coord leftC = Coord.ToCoord(left);
            if (board[leftC.x, leftC.y] > 0) {
                return -1;
            }
        }
        return left;
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
        return x + 10 * y;
    }

    public int GetRight(int of) {
        if (x < 10 - of) {
            return x + of + 10 * y;
        }
        return -1;
    }

    public int GetLeft(int of) {
        if (x > of - 1) {
            return x - of + 10 * y;
        }
        return -1;
    }

    public static Coord ToCoord(int a) {
        return new Coord(a % 10, a / 10);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPiecesOnBoard : MonoBehaviour {

    public GameObject gameManager;
    public GameObject boardPanel;
    public Board board;

    // References to piece prefabs
    public GameObject Pawn;
    public GameObject King;
    public GameObject Knight;
    public GameObject Bishop;
    public GameObject DeadBody;

    public Sprite RedKing;
    public Sprite RedPawn;
    public Sprite RedKnight;
    public Sprite RedBishop;

    public Sprite RedKing_H;
    public Sprite RedPawn_H;
    public Sprite RedKnight_H;
    public Sprite RedBishop_H;

    public Sprite BlueKing;
    public Sprite BluePawn;
    public Sprite BlueKnight;
    public Sprite BlueBishop;

    public Sprite BlueKing_H;
    public Sprite BluePawn_H;
    public Sprite BlueKnight_H;
    public Sprite BlueBishop_H;


    public Transform oldPiece; // for upgrading
    enum PlayerTag { Player1, Player2, Neutral };

    void Start()
    { 
        // This start will run after GameManager (Edit, Proj Settings, Script Exe Order)
        board = gameManager.GetComponent<GameManager>().board;
        boardPanel = gameManager.GetComponent<GameManager>().BoardPanel;
        SpawnPiece(1, 2, Pawn, "Player1");
        SpawnPiece(2, 1, Pawn, "Player1");
        SpawnPiece(1, 1, King, "Player1");
        SpawnPiece(8, 7, Pawn, "Player2");
        SpawnPiece(7, 8, Pawn, "Player2");
        SpawnPiece(8, 8, King, "Player2");
        AssignColorsByTag();
    }

    GameObject SpawnPiece(int xCoord, int yCoord, GameObject piece, string PlayerTag)
    {
        Debug.Log(piece);
        GameObject spawnInCell = board.GetCell(xCoord - 1, yCoord - 1);
        GameObject newPiece = Instantiate(piece, spawnInCell.transform);
        newPiece.tag = PlayerTag;
        newPiece.GetComponent<Piece>().xCoord = xCoord;
        newPiece.GetComponent<Piece>().yCoord = yCoord;
        return newPiece;
    }

    public void action_Upgrade()
    {
        // Only available for pawn, knight
        oldPiece = gameManager.GetComponent<GameManager>().selectedPiece;

        if (oldPiece.name == "Knight(Clone)")
        {
            UpgradeReplacement(Bishop);
            DestroyOldPiece();
            gameManager.GetComponent<GameManager>().ClearHighlights();
            gameManager.GetComponent<GameManager>().legalTiles.Clear();
            gameManager.GetComponent<GameManager>().selectedPiece = null;
            return;
        }
        if (oldPiece.name == "Pawn(Clone)")
        {
            UpgradeReplacement(Knight);
            DestroyOldPiece();
            gameManager.GetComponent<GameManager>().ClearHighlights();
            gameManager.GetComponent<GameManager>().legalTiles.Clear();
            gameManager.GetComponent<GameManager>().selectedPiece = null;
            return;
        }
        gameManager.GetComponent<GameManager>().SetInfoText("This piece cannot be upgraded.");
    }

    void AssignColorsByTag()
    {
        GameObject[] player2pieces = GameObject.FindGameObjectsWithTag("Player2");
        foreach(GameObject piece in player2pieces)
        {
            //piece.GetComponent<Image>().sprite = ;
        }
    }

    void UpgradeReplacement(GameObject upgradedPiece)
    {
        GameObject newPiece = Instantiate(upgradedPiece, oldPiece.parent);
        newPiece.tag = oldPiece.tag;
        newPiece.GetComponent<Piece>().xCoord = oldPiece.GetComponent<Piece>().xCoord;
        newPiece.GetComponent<Piece>().yCoord = oldPiece.GetComponent<Piece>().yCoord;
        if (newPiece.tag == "Player2")
        {
            newPiece.GetComponent<Image>().color = Color.blue;
        }
        newPiece.GetComponent<Piece>().turnIsOver = true;
    }

    public void action_Resurrect()
    {
        Piece piece = gameManager.GetComponent<GameManager>().selectedPiece.GetComponent<Piece>();
        Cell cell = board.GetCell(piece.xCoord-1, piece.yCoord-1).GetComponent<Cell>();
        if (!piece.canResurrect)
        {
            gameManager.GetComponent<GameManager>().SetInfoText("This unit cannot resurrect.");
            return;
        }
        if (!cell.uncovered || cell.hidden != Cell.hiddenValue.Body)
        {
            gameManager.GetComponent<GameManager>().SetInfoText("There are no bodies here to resurrect.");
            return;
        }
        cell.hidden = Cell.hiddenValue.Empty;

        int[] offsets = { -1, 0, 1 };
        bool found = false;
        foreach (int i in offsets)
        {
            if (found) break;
            foreach (int j in offsets)
            {
                int x = piece.xCoord - 1 + i;
                int y = piece.yCoord - 1 + j;
                if (x > 0 && x <= StaticVariables.BoardSize && y > 0 && y <= StaticVariables.BoardSize && board.GetCell(x, y).transform.childCount == 0)
                {
                    gameManager.GetComponent<GameManager>().SetInfoText("You resurrected at " + (x+1) + ", " + (y+1));
                    SpawnPiece(x+1, y+1, Pawn, gameManager.GetComponent<GameManager>().Player1Turn ? "Player1" : "Player2");
                    for (int k = 0; k < board.GetCell(piece.xCoord - 1, piece.yCoord - 1).transform.childCount; k++) {
                        if (board.GetCell(piece.xCoord - 1, piece.yCoord - 1).transform.GetChild(k).gameObject.tag == "Neutral") {
                            Destroy(board.GetCell(piece.xCoord - 1, piece.yCoord - 1).transform.GetChild(k).gameObject);
                        }
                    }
                    found = true;
                    break;
                }
            }
        }
        if (!found)
        {
            gameManager.GetComponent<GameManager>().SetInfoText("No room for resurrection!");
            cell.hidden = Cell.hiddenValue.Body;
        }

        piece.turnIsOver = true;
        gameManager.GetComponent<GameManager>().ClearHighlights();
        gameManager.GetComponent<GameManager>().legalTiles.Clear();
        gameManager.GetComponent<GameManager>().selectedPiece = null;
    }

    public void action_Dig()
    {
        Piece piece = gameManager.GetComponent<GameManager>().selectedPiece.GetComponent<Piece>();
        Cell cell = board.GetCell(piece.xCoord-1, piece.yCoord-1).GetComponent<Cell>();
        if (cell.uncovered)
        {
            gameManager.GetComponent<GameManager>().SetInfoText("This cell has already been dug up.");
            return;
        }
        cell.uncovered = true;
        if (cell.hidden == Cell.hiddenValue.Bomb)
        {
            // If this is king
            if(gameManager.GetComponent<GameManager>().selectedPiece.name == "King(Clone)")
            {
                Destroy(gameManager.GetComponent<GameManager>().selectedPiece.gameObject);
                gameManager.GetComponent<GameManager>().PlayerWinsState(gameManager.GetComponent<GameManager>().Player1Turn ? "Player2" : "Player1", "Bomb");
            }
            else
            {
                Destroy(gameManager.GetComponent<GameManager>().selectedPiece.gameObject);
                cell.hidden = Cell.hiddenValue.Empty;
                gameManager.GetComponent<GameManager>().SetInfoText("You dug up a bomb! All cells on that tile have died.");
            }         
        }
        else if (cell.hidden == Cell.hiddenValue.Empty)
        {
            gameManager.GetComponent<GameManager>().SetInfoText("Nothing found.");
        }
        else if (cell.hidden == Cell.hiddenValue.Body)
        {
            var dead = Instantiate(DeadBody, board.GetCell(piece.xCoord - 1, piece.yCoord - 1).transform);
            gameManager.GetComponent<GameManager>().SetInfoText("You dug up a body that you can resurrect.");
        }

        piece.turnIsOver = true;
        gameManager.GetComponent<GameManager>().ClearHighlights();
        gameManager.GetComponent<GameManager>().legalTiles.Clear();
        gameManager.GetComponent<GameManager>().selectedPiece = null;
    }

    void DestroyOldPiece()
    {
        Destroy(oldPiece.gameObject);
        oldPiece = null;
    }
}

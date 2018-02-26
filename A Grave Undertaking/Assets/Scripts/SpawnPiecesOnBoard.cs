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
        //SpawnPiece(4, 4, Knight, "Player2");
        //SpawnPiece(4, 5, Bishop, "Player2");
        AssignColorsByTag();
    }

    // This will have a third argument with the type of piece to spawn
    void SpawnPiece(int xCoord, int yCoord, GameObject piece, string PlayerTag)
    {
        Debug.Log(piece);
        GameObject spawnInCell = board.GetCell(xCoord - 1, yCoord - 1);
        GameObject newPiece = Instantiate(piece, spawnInCell.transform);
        newPiece.tag = PlayerTag;
        newPiece.GetComponent<Piece>().xCoord = xCoord;
        newPiece.GetComponent<Piece>().yCoord = yCoord;
        if (newPiece.tag == "Player2")
        {
            newPiece.GetComponent<Image>().color = Color.blue;
        }
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
        Debug.Log("You cannot upgrade this piece");
    }

    void AssignColorsByTag()
    {
        GameObject[] player2pieces = GameObject.FindGameObjectsWithTag("Player2");
        foreach(GameObject piece in player2pieces)
        {
            piece.GetComponent<Image>().color = Color.blue;
        }
    }

    void UpgradeReplacement(GameObject upgradedPiece)
    {
        GameObject newPiece = Instantiate(upgradedPiece, oldPiece.parent);
        newPiece.tag = oldPiece.tag;
        if(newPiece.tag == "Player2")
        {
            newPiece.GetComponent<Image>().color = Color.blue;
        }
        newPiece.GetComponent<Piece>().turnIsOver = true;
    }

    public void action_Resurrect()
    {
        Piece piece = gameManager.GetComponent<GameManager>().selectedPiece.GetComponent<Piece>();
        Cell cell = board.GetCell(piece.xCoord-1, piece.yCoord-1).GetComponent<Cell>();
        if (cell.hidden != Cell.hiddenValue.Body)
        {
            gameManager.GetComponent<GameManager>().SetInfoText("You cannot resurrect here!");
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

        gameManager.GetComponent<GameManager>().ClearHighlights();
        gameManager.GetComponent<GameManager>().legalTiles.Clear();
        gameManager.GetComponent<GameManager>().selectedPiece = null;
    }

    public void action_Dig()
    {
        Piece piece = gameManager.GetComponent<GameManager>().selectedPiece.GetComponent<Piece>();
        Cell cell = board.GetCell(piece.xCoord-1, piece.yCoord-1).GetComponent<Cell>();
        gameManager.GetComponent<GameManager>().SetInfoText("You dug up "+cell.hidden);
        if (cell.hidden == Cell.hiddenValue.Bomb)
        {
            Destroy(gameManager.GetComponent<GameManager>().selectedPiece.gameObject);
            cell.hidden = Cell.hiddenValue.Empty;
        }

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

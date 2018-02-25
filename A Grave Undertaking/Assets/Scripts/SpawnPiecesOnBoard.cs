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
        SpawnPiece(4, 4, Knight, "Player2");
        SpawnPiece(4, 5, Bishop, "Player2");
        SpawnPiece(6, 3, DeadBody, "Neutral");
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
    }

    public void action_Upgrade()
    {
        // Only available for pawn, knight
        oldPiece = gameManager.GetComponent<GameManager>().selectedPiece;

        if (oldPiece.name == "Knight(Clone)")
        {
            UpgradeReplacement(Bishop);
            DestroyOldPiece();
            return;
        }
        if (oldPiece.name == "Pawn(Clone)")
        {
            UpgradeReplacement(Knight);
            DestroyOldPiece();
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
    }

    public void action_Resurrect()
    {
        // Available only if the current cell of selected piece contains a neutral piece
        // Just pick a random neutral piece if more than 1? Is there a need to differentiate?
    }

    public void action_Dig()
    {
        // Available only if the current cell of selected piece has not been flipped
    }

    void DestroyOldPiece()
    {
        Destroy(oldPiece.gameObject);
        oldPiece = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiecesOnBoard : MonoBehaviour {

    public GameObject gameManager;
    public GameObject boardPanel;
    public Board board;

    // References to piece prefabs
    public GameObject Pawn;
    public GameObject King;
    public GameObject Knight;
    public GameObject Bishop;

    void Start()
    { 
        // This start will run after GameManager (Edit, Proj Settings, Script Exe Order)
        board = gameManager.GetComponent<GameManager>().board;
        boardPanel = gameManager.GetComponent<GameManager>().BoardPanel;
        SpawnPiece(3, 5);
    }

    // This will have a third argument with the type of piece to spawn
    void SpawnPiece(int xCoord, int yCoord)
    {
        GameObject spawnInCell = board.GetCell(xCoord, yCoord);
        GameObject newPiece = Instantiate(Pawn, spawnInCell.transform.position, Quaternion.identity, spawnInCell.transform);
    }
}

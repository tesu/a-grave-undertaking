using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject WhiteCellPrefab;
    public GameObject BlackCellPrefab;
    public GameObject BoardPanel;

    private int boardSize = 8;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < boardSize; i++) {
            for (int j = 0; j < boardSize; j++) {
                GameObject cell;
                if ((i + j%2) % 2 == 0) {
                    cell = Instantiate(WhiteCellPrefab);
                }
                else {
                    cell = Instantiate(BlackCellPrefab);
                }
                cell.transform.SetParent(BoardPanel.transform);
                //board.addcell(cell)
            }
        }
	}

	// Update is called once per frame
	void Update () {
		
	}
}

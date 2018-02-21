using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private int boardSize;

	// Use this for initialization
	void Start () {
        for (int i=0; i<boardSize*boardSize; i++) {
            GameObject cell = Instantiate(cellPrefab);
            //cell.transform.SetParent(boardpanel.transform);
            //board.addcell(cell)
        }
	}

	// Update is called once per frame
	void Update () {
		
	}
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject WhiteCellPrefab;
    public GameObject BlackCellPrefab;
    public GameObject BoardPanel;
    public Raycaster raycaster;
    public Color HighlightColor;

    private int boardSize = 8;
    private Board board;
    private GameObject highlightedCell;

	// Use this for initialization
	void Start () {
        board = new Board();
        for (int i = 0; i < boardSize; i++) {
            for (int j = 0; j < boardSize; j++) {
                GameObject cell;
                if ((i + j % 2) % 2 == 0) {
                    cell = Instantiate(WhiteCellPrefab);
                }
                else {
                    cell = Instantiate(BlackCellPrefab);
                }
                cell.transform.SetParent(BoardPanel.transform);
                board.AddCell(cell);
            }
        }
	}

    // Update is called once per frame
    void Update() {

        List<RaycastResult> raycastResults = raycaster.GetAllRaycastObjects();
        GameObject resultCell = null;
        foreach (RaycastResult result in raycastResults) {
            if (result.gameObject.tag == "Cell") {
                resultCell = result.gameObject;
            }
        }


        if (resultCell != null) {
            DeHighlightCell();
            highlightedCell = resultCell;
        }
        else {
            DeHighlightCell();
            highlightedCell = null;
        }
        HighlightCell();

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (highlightedCell != null) {
                Debug.Log(board.GetCellX(highlightedCell) + ", " + board.GetCellY(highlightedCell));
            }
            else {
                Debug.Log("No Cell");
            }
        }
    }

    void DeHighlightCell() {
        if (highlightedCell != null) {
            highlightedCell.GetComponent<Image>().color = highlightedCell.GetComponent<Cell>().OriginalColor;
        }
    }

    void HighlightCell() {
        if (highlightedCell != null) {
            highlightedCell.GetComponent<Image>().color = HighlightColor;
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour {

    public GameObject WhiteCellPrefab;
    public GameObject BlackCellPrefab;
    public GameObject BoardPanel;
    public GameObject UnitPanel;
    public GameObject StartGamePanel;
    public GameObject EndGamePanel;
    public Raycaster raycaster;
    public Color HighlightColor;
    public Color ClickedColor;
    public Color LegalMoveColor;
    public Text TurnText;
    public Text InfoText;
    public Text GameOverText;
    public int TextFontSize;
    public Button UpgradeButton;
    public Button RezButton;
    public Button DigButton;
    public Button FinishButton;
    public bool Player1Turn;
    public Transform selectedPiece;
    public List<GameObject> legalTiles = new List<GameObject>();
    public Board board;

    private int boardSize = StaticVariables.BoardSize;
    private GameObject highlightedCell;
    private GameObject selectedCell;
    private Dictionary<GameObject, GameObject> panelToBoardPiece = new Dictionary<GameObject, GameObject>();
    private bool gameStarted = false;
    private bool insistsToFinish = false;

	// Use this for initialization
	void Start () {
        InitUI();
    }

    // Update is called once per frame
    void Update() {
        if (!gameStarted)
            return;
        if (ReadyToFinish()) {
            FinishButton.GetComponent<Image>().color = Color.green;
        }

        SetButtonInteractions();

        List<RaycastResult> raycastResults = raycaster.GetAllRaycastObjects();
        GameObject resultCell = null;
        GameObject resultPiece = null;
        GameObject resultButton = null;
        foreach (RaycastResult result in raycastResults) {
            if (result.gameObject.tag == "Cell") {
                resultCell = result.gameObject;
            }
            if (result.gameObject.tag == "UnitPanelPiece") {
                resultPiece = result.gameObject;
            }
            if (result.gameObject.tag == "Button") {
                resultButton = result.gameObject;
            }
        }

        if (resultCell != null) {
            DeHighlightCell(false);
            highlightedCell = resultCell;
        }
        else {
            DeHighlightCell(false);
            highlightedCell = null;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (highlightedCell != null) {
                DeHighlightCell(true);
                ClearUnitPanel();
                selectedCell = highlightedCell;
                string info = (board.GetCellX(highlightedCell) + 1) + ", " + (board.GetCellY(highlightedCell) + 1);
                Debug.Log(info);
                if (selectedPiece)
                {
                    if(legalTiles.Contains(highlightedCell))
                    {
                        Debug.Log("Moving piece to legal tile");
                        selectedPiece.transform.SetParent(highlightedCell.transform);
                        selectedPiece.transform.position = highlightedCell.transform.position;
                        selectedPiece.GetComponent<Piece>().xCoord = board.GetCellX(highlightedCell) + 1;
                        selectedPiece.GetComponent<Piece>().yCoord = board.GetCellY(highlightedCell) + 1;
                        selectedPiece.GetComponent<Piece>().turnIsOver = true;
                        // This is an attack
                        if(highlightedCell.transform.GetChild(0).tag == "Player" + (Player1Turn ? 2 : 1))
                        {
                            if(highlightedCell.transform.GetChild(0).name == "King(Clone)")
                            {
                                PlayerWinsState(selectedPiece.tag, "Attack");
                                Destroy(highlightedCell.transform.GetChild(0).gameObject);
                            }
                            Destroy(highlightedCell.transform.GetChild(0).gameObject);
                            SetInfoText("You killed an enemy!");
                        }
                        ClearHighlights();
                        legalTiles.Clear();
                        selectedPiece = null;
                    }
                    else if(board.GetCellX(highlightedCell) + 1 == selectedPiece.GetComponent<Piece>().xCoord && board.GetCellY(highlightedCell) + 1 == selectedPiece.GetComponent<Piece>().yCoord)
                    {
                        Debug.Log("You selected the same tile. That piece is now deselected.");
                        ClearHighlights();
                        legalTiles.Clear();
                        selectedPiece = null;
                        selectedCell = null;
                    }
                    else
                    {
                        Debug.Log("This is not a legal move. Please select a highlighted tile");
                    }                 
                }
                else if (highlightedCell.transform.childCount > 0) // Any pieces on this cell?
                {
                    for (int i = 0; i < highlightedCell.transform.childCount; i++) {
                        AddUnitToUnitPanel(highlightedCell.transform.GetChild(i));
                    }
                    // This assumes 1 child for now for simplicity
                    // Neutral pieces cannot be selected
                    
                    if (highlightedCell.transform.childCount == 1 && highlightedCell.transform.GetChild(0).tag != "Neutral")
                    {
                        if(highlightedCell.transform.GetChild(0).GetComponent<Piece>().turnIsOver)
                        {
                            Debug.Log("This piece cannot be selected. It cannot move this turn");
                        }
                        else
                        {
                            selectedPiece = highlightedCell.transform.GetChild(0);
                            CalculateLegalMoves(selectedPiece);
                        }               
                    }        
                    
                }
            }
            else {
                if (resultPiece != null && panelToBoardPiece[resultPiece].tag == "Player" + (Player1Turn ? 1 : 2)) {
                    selectedPiece = panelToBoardPiece[resultPiece].transform;
                    CalculateLegalMoves(selectedPiece);
                }
                else if (resultButton == null) {
                    DeHighlightCell(true);
                    ClearHighlights();
                    legalTiles.Clear();
                    selectedPiece = null;
                    selectedCell = null;
                    Debug.Log("No Cell");
                }
            }
        }

        HighlightCell();
    }

    bool ReadyToFinish() {
        if (insistsToFinish)
            return true;
        foreach (Piece piece in FindObjectsOfType<Piece>()) {
            if (!piece.turnIsOver) {
                return false;
            }
        }
        return true;
    }

    void AddUnitToUnitPanel(Transform child) {
        var clone = Instantiate(child.gameObject);
        if (child.GetComponent<Piece>() != null) {
            var childPiece = child.GetComponent<Piece>();
            clone.GetComponent<Piece>().activeSprite = childPiece.activeSprite;
            clone.GetComponent<Piece>().deactiveSprite = childPiece.deactiveSprite;
            clone.GetComponent<Piece>().turnIsOver = childPiece.turnIsOver;
        }
        clone.tag = "UnitPanelPiece";
        panelToBoardPiece.Add(clone, child.gameObject);
        clone.transform.SetParent(UnitPanel.transform);
    }

    void ClearUnitPanel() {
        for (int i = 0; i < UnitPanel.transform.childCount; i++) {
            Destroy(UnitPanel.transform.GetChild(i).gameObject);
        }
        panelToBoardPiece.Clear();
    }

    void CalculateLegalMoves(Transform selectedPiece)
    {
        // works because inheritance
        int x = selectedPiece.GetComponent<Piece>().xCoord;
        int y = selectedPiece.GetComponent<Piece>().yCoord;
        Debug.Log("x: " + x);
        Debug.Log("y: " + y);

        if (selectedPiece.name == "Pawn(Clone)" || selectedPiece.name == "King(Clone)")
        {
            // NE
            if (x + 1 > 0 && x + 1 <= StaticVariables.BoardSize)
            {
                if (y - 1 > 0 && y - 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x, y - 2));
                }
            }
            // E
            if (x + 1 > 0 && x + 1 <= StaticVariables.BoardSize)
            {
                if (y > 0 && y <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x, y - 1));
                }
            }
            // SE
            if (x + 1 > 0 && x + 1 <= StaticVariables.BoardSize)
            {
                if (y + 1 > 0 && y + 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x, y));
                }
            }
            // S
            if (x > 0 && x <= StaticVariables.BoardSize)
            {
                if (y + 1 > 0 && y + 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 1, y));
                }
            }
            // SW
            if (x - 1 > 0 && x - 1 <= StaticVariables.BoardSize)
            {
                if (y + 1 > 0 && y + 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 2, y));
                }
            }
            // W
            if (x - 1 > 0 && x - 1 <= StaticVariables.BoardSize)
            {
                if (y > 0 && y <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 2, y - 1));
                }
            }
            // NW
            if (x - 1 > 0 && x - 1 <= StaticVariables.BoardSize)
            {
                if (y - 1 > 0 && y - 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 2, y - 2));
                }
            }
            // N
            if (x > 0 && x <= StaticVariables.BoardSize)
            {
                if (y - 1 > 0 && y - 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 1, y - 2));
                }
            }
        }      
        if (selectedPiece.name == "Knight(Clone)")
        {
            // N-NE
            if (x + 1 > 0 && x + 1 <= StaticVariables.BoardSize)
            {
                if (y - 2 > 0 && y - 2 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x, y - 3));
                }
            }
            // E-NE
            if (x + 2 > 0 && x + 2 <= StaticVariables.BoardSize)
            {
                if (y - 1 > 0 && y - 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x + 1, y - 2));
                }
            }
            // E-SE
            if (x + 2 > 0 && x + 2 <= StaticVariables.BoardSize)
            {
                if (y + 1 > 0 && y + 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x + 1, y));
                }
            }
            // S-SE
            if (x + 1 > 0 && x + 1 <= StaticVariables.BoardSize)
            {
                if (y + 2 > 0 && y + 2 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x, y + 1));
                }
            }
            // S-SW
            if (x - 1 > 0 && x - 1 <= StaticVariables.BoardSize)
            {
                if (y + 2 > 0 && y + 2 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 2, y + 1));
                }
            }
            // W-SW
            if (x - 2 > 0 && x - 2 <= StaticVariables.BoardSize)
            {
                if (y + 1 > 0 && y + 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 3, y));
                }
            }
            // W-NW
            if (x - 2 > 0 && x - 2 <= StaticVariables.BoardSize)
            {
                if (y - 1 > 0 && y - 1 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 3, y - 2));
                }
            }
            // N-NW
            if (x - 1 > 0 && x - 1 <= StaticVariables.BoardSize)
            {
                if (y - 2 > 0 && y - 2 <= StaticVariables.BoardSize)
                {
                    legalTiles.Add(board.GetCell(x - 2, y - 3));
                }
            }
        }
        if (selectedPiece.name == "Bishop(Clone)")
        {
            // NE
            int xtemp = x;
            int ytemp = y;
            while(xtemp <= StaticVariables.BoardSize && ytemp >= 1)
            {
                if (xtemp + 1 > 0 && xtemp + 1 <= StaticVariables.BoardSize)
                {
                    if (ytemp - 1 > 0 && ytemp - 1 <= StaticVariables.BoardSize)
                    {                       
                        legalTiles.Add(board.GetCell(xtemp, ytemp - 2));

                        // If a piece there, we want to break out of the while loop.
                        // Can move on top of a piece but not through it
                        if (board.GetCell(xtemp, ytemp - 2).transform.childCount > 0)
                        {
                            goto Southeast;
                        }
                        
                    }
                }
                xtemp++;
                ytemp--;
            }
            // SE
            Southeast:
            xtemp = x;
            ytemp = y;
            while(xtemp <= StaticVariables.BoardSize && ytemp <= StaticVariables.BoardSize)
            {
                if (xtemp + 1 > 0 && xtemp + 1 <= StaticVariables.BoardSize)
                {
                    if (ytemp + 1 > 0 && ytemp + 1 <= StaticVariables.BoardSize)
                    {
                        legalTiles.Add(board.GetCell(xtemp, ytemp));
                        if (board.GetCell(xtemp, ytemp).transform.childCount > 0)
                        {
                            goto Southwest;
                        }
                    }
                }
                xtemp++;
                ytemp++;
            }
            // SW
            Southwest:
            xtemp = x;
            ytemp = y;
            while (xtemp >= 1 && ytemp <= StaticVariables.BoardSize)
            {
                if (xtemp - 1 > 0 && xtemp - 1 <= StaticVariables.BoardSize)
                {
                    if (ytemp + 1 > 0 && ytemp + 1 <= StaticVariables.BoardSize)
                    {
                        legalTiles.Add(board.GetCell(xtemp - 2, ytemp));
                        if (board.GetCell(xtemp - 2, ytemp).transform.childCount > 0)
                        {
                            goto Northwest;
                        }
                    }
                }
                xtemp--;
                ytemp++;
            }
            // NW
            Northwest:
            xtemp = x;
            ytemp = y;
            while (xtemp >= 1 && ytemp >= 1)
            {
                if (xtemp - 1 > 0 && xtemp - 1 <= StaticVariables.BoardSize)
                {
                    if (ytemp - 1 > 0 && ytemp - 1 <= StaticVariables.BoardSize)
                    {
                        legalTiles.Add(board.GetCell(xtemp - 2, ytemp - 2));
                        if (board.GetCell(xtemp - 2, ytemp - 2).transform.childCount > 0)
                        {
                            goto Done;
                        }
                    }
                }
                xtemp--;
                ytemp--;
            }
        }
        Done:
        HighlightLegalTiles(legalTiles);
    }

    void HighlightLegalTiles(List<GameObject> legalTiles)
    {
        foreach(GameObject tile in legalTiles)
        {
            tile.GetComponent<Image>().color = Color.green;
        }
    }

    public void ClearHighlights()
    {
        foreach(GameObject tile in legalTiles)
        {
            tile.GetComponent<Image>().color = tile.GetComponent<Cell>().NormalColor();
        }
        ClearUnitPanel();
    }

    void InitUI() {
        InitBoard();
        
        StartGamePanel.SetActive(true);
        FinishButton.onClick.AddListener(OnFinishButtonClick);
        Player1Turn = false;
        SetTurnText();
    }

    void InitBoard() {
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

    public void OnFinishButtonClick() {
        if (gameStarted && !ReadyToFinish()) {
            SetInfoText("Some pieces haven't moved. Press Finish again if you want to continue anyway.");
            insistsToFinish = true;
            return;
        }
        gameStarted = true;
        insistsToFinish = false;
        FinishButton.GetComponent<Image>().color = Color.white;
        Player1Turn = !Player1Turn;
        selectedPiece = null;
        SetTurnText();
        SetInfoText("");
    }

    void DeHighlightCell(bool dehighlightSelected) {
        if (dehighlightSelected) {
            if (highlightedCell != null) {
                highlightedCell.GetComponent<Image>().color = highlightedCell.GetComponent<Cell>().NormalColor();
            }
            if (selectedCell != null) {
                selectedCell.GetComponent<Image>().color = selectedCell.GetComponent<Cell>().NormalColor();
            }
        }
        else {
            if (highlightedCell != null && legalTiles.Contains(highlightedCell)) {
                highlightedCell.GetComponent<Image>().color = LegalMoveColor;
            }
            else if (highlightedCell != null) {
                highlightedCell.GetComponent<Image>().color = highlightedCell.GetComponent<Cell>().NormalColor();
            }
        }
    }

    void HighlightCell() {
        if (highlightedCell != null) {
            highlightedCell.GetComponent<Image>().color = HighlightColor;
        }
        if (selectedCell != null) {
            selectedCell.GetComponent<Image>().color = ClickedColor;
        }
    }

    void SetButtonInteractions() {
        if (selectedPiece != null) {
            DigButton.interactable = true;
            if (selectedPiece.GetComponent<Piece>().canResurrect) {
                RezButton.interactable = true;
                UpgradeButton.interactable = false;
            }
            else {
                UpgradeButton.interactable = true;
                RezButton.interactable = false;
            }
        }
        else {
            DigButton.interactable = false;
            UpgradeButton.interactable = false;
            RezButton.interactable = false;
        }
    }

    public void SetInfoText(string text) {
        InfoText.text = "<color=white><size=" + TextFontSize + ">" + text + "</size></color>";
    }

    void SetTurnText() {
        if (!gameStarted)
            return;

        int player;
        if (Player1Turn) {
            player = 1;
        }
        else {
            player = 2;
        }

        GameObject[] piecesToBeActivated =  GameObject.FindGameObjectsWithTag("Player" + player);
        foreach(GameObject piece in piecesToBeActivated)
        {
            piece.GetComponent<Piece>().turnIsOver = false;
        }

        // just in case pieces aren't used
        int tempOtherPlayer;
        if (player == 1)
        {
            tempOtherPlayer = 2;
        }
        else
        {
            tempOtherPlayer = 1;
        }

        GameObject[] piecesToBeDeactivated = GameObject.FindGameObjectsWithTag("Player" + tempOtherPlayer);
        foreach (GameObject piece in piecesToBeDeactivated)
        {
            piece.GetComponent<Piece>().turnIsOver = true;
        }

        TurnText.text = "<color=white><size=" + TextFontSize + ">" + ( player == 1 ? "Red" : "Blue" ) + " Player's Turn" + "</size></color>";
    }
    public void PlayerWinsState(string currentPlayer, string reason)
    {
        if(reason == "Bomb")
        {
            GameOverText.text = "Your King has died to a bomb. " + currentPlayer + " wins!";
        }
        else
        {
            GameOverText.text = "The enemy King has fallen. " + currentPlayer + " wins!";
        }
        EndGamePanel.SetActive(true);  
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        StartGamePanel.SetActive(false);
    }
}

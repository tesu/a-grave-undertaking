using System.Collections;
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
    public Color ClickedColor;
    public Text TurnText;
    public Text InfoText;
    public int TextFontSize;
    public Button AttackButton;
    public Button MoveButton;
    public Button DigButton;
    public Button FinishButton;
    public bool Player1Turn;

    public List<GameObject> legalTiles = new List<GameObject>();

    private int boardSize = StaticVariables.BoardSize;
    public Board board;
    private GameObject highlightedCell;
    private GameObject selectedCell;
    public Transform selectedPiece;
    private bool clicked;

	// Use this for initialization
	void Start () {
        InitUI();
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
            DeHighlightCell(false);
            highlightedCell = resultCell;
        }
        else {
            DeHighlightCell(false);
            highlightedCell = null;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (highlightedCell != null) {
                clicked = true;
                DeHighlightCell(true);
                selectedCell = highlightedCell;
                string info = (board.GetCellX(highlightedCell) + 1) + ", " + (board.GetCellY(highlightedCell) + 1);
                Debug.Log(info);
                SetInfoText("You clicked square: " + info);
                if (selectedPiece)
                {
                    Debug.Log("Moving piece");
                    selectedPiece.transform.parent = highlightedCell.transform;
                    selectedPiece.transform.position = highlightedCell.transform.position;
                    selectedPiece.GetComponent<Pawn>().xCoord = board.GetCellX(highlightedCell) + 1;
                    selectedPiece.GetComponent<Pawn>().yCoord = board.GetCellY(highlightedCell) + 1;
                    ClearHighlights();
                    legalTiles.Clear();
                    selectedPiece = null;
                }
                else if (highlightedCell.transform.childCount > 0) // Any pieces on this cell?
                {
                    // This assumes 1 child for now for simplicity
                    selectedPiece = highlightedCell.transform.GetChild(0);
                    CalculateLegalMoves(selectedPiece);
                }
            }
            else {
                DeHighlightCell(true);
                if (selectedCell != null) {
                    selectedCell = null;
                }
                Debug.Log("No Cell");
                SetInfoText("You didn't click a square");
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            clicked = false;
        }

        HighlightCell();
    }

    void CalculateLegalMoves(Transform selectedPiece)
    {
        if(selectedPiece.name == "Pawn(Clone)")
        {
            int x = selectedPiece.GetComponent<Pawn>().xCoord;
            int y = selectedPiece.GetComponent<Pawn>().yCoord;
            Debug.Log("x: " + x);
            Debug.Log("y: " + y);
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
        HighlightLegalTiles(legalTiles);
        if (selectedPiece.name == "King(Clone)")
        {

        }
        if (selectedPiece.name == "Knight(Clone)")
        {

        }
        if (selectedPiece.name == "Bishop(Clone)")
        {

        }
    }

    void HighlightLegalTiles(List<GameObject> legalTiles)
    {
        foreach(GameObject tile in legalTiles)
        {
            tile.GetComponent<Image>().color = Color.blue;
        }
    }
    void ClearHighlights()
    {
        foreach(GameObject tile in legalTiles)
        {
            tile.GetComponent<Image>().color = Color.black;
        }
    }

    void InitUI() {
        InitBoard();

        SetInfoText("Welcome!");
        SetTurnText();

        AttackButton.onClick.AddListener(OnAttackButtonClick);
        MoveButton.onClick.AddListener(OnMoveButtonClick);
        DigButton.onClick.AddListener(OnDigButtonClick);
        FinishButton.onClick.AddListener(OnFinishButtonClick);
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

    void OnAttackButtonClick() {

    }

    void OnMoveButtonClick() {

    }

    void OnDigButtonClick() {

    }

    void OnFinishButtonClick() {
        Player1Turn = !Player1Turn;
        SetTurnText();
    }

    void DeHighlightCell(bool dehighlightSelected) {
        if (highlightedCell != null) {
            highlightedCell.GetComponent<Image>().color = highlightedCell.GetComponent<Cell>().OriginalColor;
        }
        if (dehighlightSelected && selectedCell != null) {
            selectedCell.GetComponent<Image>().color = selectedCell.GetComponent<Cell>().OriginalColor;
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

    void SetInfoText(string text) {
        InfoText.text = "<color=white><size=" + TextFontSize + ">" + text + "</size></color>";
    }

    void SetTurnText() {
        int player;
        if (Player1Turn) {
            player = 1;
        }
        else {
            player = 2;
        }
        TurnText.text = "<color=white><size=" + TextFontSize + ">Player " + player + "'s Turn" + "</size></color>";
    }
}

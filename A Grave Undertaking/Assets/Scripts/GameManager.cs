using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject WhiteCellPrefab;
    public GameObject BlackCellPrefab;
    public GameObject BoardPanel;
    public GameObject StartGamePanel;
    public GameObject EndGamePanel;
    public Raycaster raycaster;
    public Color HighlightColor;
    public Color ClickedColor;
    public Text TurnText;
    public Text InfoText;
    public Text GameOverText;
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
                        if(highlightedCell.transform.GetChild(0).tag != selectedPiece.tag)
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
                    }
                    else
                    {
                        Debug.Log("This is not a legal move. Please select a highlighted tile");
                    }                 
                }
                else if (highlightedCell.transform.childCount > 0) // Any pieces on this cell?
                {
                    // This assumes 1 child for now for simplicity
                    // Neutral pieces cannot be selected
                    if (highlightedCell.transform.GetChild(0).tag != "Neutral")
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
                DeHighlightCell(true);
                if (selectedCell != null) {
                    selectedCell = null;
                }
                Debug.Log("No Cell");
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            clicked = false;
        }

        HighlightCell();
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
            tile.GetComponent<Image>().color = Color.blue;
        }
    }
    public void ClearHighlights()
    {
        foreach(GameObject tile in legalTiles)
        {
            tile.GetComponent<Image>().color = Color.black;
        }
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
        Player1Turn = !Player1Turn;
        selectedPiece = null;
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

    public void SetInfoText(string text) {
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

        TurnText.text = "<color=white><size=" + TextFontSize + ">Player " + player + "'s Turn" + "</size></color>";
    }
    public void PlayerWinsState(string currentPlayer, string reason)
    {
        if(reason == "Bomb")
        {
            GameOverText.text = "Your King has died to a bomb. " + currentPlayer + " wins!";
            Debug.Log("Your King has died to a bomb. " + currentPlayer + " wins!");
        }
        else
        {
            GameOverText.text = "The enemy King has fallen. " + currentPlayer + " wins!";
            Debug.Log("The enemy King has fallen. " + currentPlayer + " wins!");
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

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

    private int boardSize = StaticVariables.BoardSize;
    private Board board;
    private GameObject highlightedCell;
    private bool clicked;

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
        SetInfoText("Welcome!");
        SetTurnText();
        FinishButton.onClick.AddListener(OnFinishButtonClick);
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
                clicked = true;
                string info = (board.GetCellX(highlightedCell) + 1) + ", " + (board.GetCellY(highlightedCell) + 1);
                Debug.Log(info);
                SetInfoText("You clicked square: " + info);
            }
            else {
                Debug.Log("No Cell");
                SetInfoText("You didn't click a square");
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            clicked = false;
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
        Debug.Log("clicked");
    }

    void DeHighlightCell() {
        if (highlightedCell != null) {
            highlightedCell.GetComponent<Image>().color = highlightedCell.GetComponent<Cell>().OriginalColor;
        }
    }

    void HighlightCell() {
        if (highlightedCell != null) {
            Color color = HighlightColor;
            if (clicked) color = ClickedColor;
            highlightedCell.GetComponent<Image>().color = color;
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

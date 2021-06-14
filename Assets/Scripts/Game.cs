using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// Defines a class that represents the game.
/// </summary>
public class Game : MonoBehaviour
{
    /// <summary>
    /// The array of generators that should be used to generate cells.
    /// </summary>
    public TableCellGenerator cellGenerator;

    public Camera gameCamera;
    public Canvas popupCanvas;
    public GameObject popupCanvasText;
    public GameObject PauseButton;
    public int movesRemaining;
    public GameObject movesRemainingLabel;
    public GameObject movesRemainingDesc;
    public GameObject scoreTotal;

    private int _totalCellsCaptured;
    private int _totalCellsToCapture;
    private int _scoreTotal;
    private bool isMovesRemainingSet;
    private gameState _gameState;

    public enum gameState
    {
        Playing,
        Paused,
        gameOver
    }

    void Awake()
    {
        //initialize moves Remaining 
        isMovesRemainingSet = movesRemaining != 0;

        //Since we have no unfloodable cells...
        _totalCellsToCapture = (int)(cellGenerator.size.x * cellGenerator.size.y);

        //initialize totals cells Captured
        _totalCellsCaptured = 1; //we only have one captured cell to start at the moment.
        //cellGenerator.generatedGroups
        //    .SelectMany(g => g.cells)
        //    .GroupBy(g => g)
        //    .Select(g => g.Count(c => c.captured)).Sum();
    }

    void Start()
    {
        _gameState = gameState.Playing;
        popupCanvas.gameObject.SetActive(false);

        //make first pass at grid with starting captured cells
        //UpdateGrid(cellGenerator.startingCell.transform.position, cellGenerator.startingCell.targetMaterial);
        UpdateUI();
    }

    void Update()
    {
        if (_gameState != gameState.Playing) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info))
            {
                Cell hitCell = info.transform.GetComponent<Cell>();
                if (cellGenerator.generatedGroups[0].cells.All(c => c.animationState == CellAnimationState.normal || c == hitCell))
                {
                    var targetMaterial = info.transform.GetComponent<Renderer>().material;
                    Color CapturedColor = cellGenerator.generatedGroups.First().cells.First().GetComponent<Renderer>().material.color;
                    if (CapturedColor != targetMaterial.color)
                    {
                        if (isMovesRemainingSet) movesRemaining--;
                        CheckGameOver();

                        UpdateGrid(info.transform.position, targetMaterial);

                        UpdateUI();

                        CheckWin();
                    }
                }
            }
        }
    }

    private void UpdateGrid(Vector3 hitPos, Material targetMaterial)
    {
        bool hasAnyCellBeenCaptured = false;
        int initialCellsCaptured = _totalCellsCaptured;
        int groupsCaptured = 0;
        foreach (Cell c in cellGenerator.generatedGroups[0].cells.Where(c => c.captured))
        {
            float dist = Vector3.Distance(c.transform.position, hitPos);
            if (!c.enclosed)
            {
                bool allSame = true;
                var siblings = cellGenerator.generatedGroups[0].siblingMap[c].ToArray();
                bool isGroupCaptured = false;
                foreach (Cell sibling in siblings.Where(s => !s.captured))
                {
                    if (sibling.GetComponent<Renderer>().material.color == targetMaterial.color)
                    {
                        sibling.targetMaterial = targetMaterial;
                        sibling.captured = true;
                        _totalCellsCaptured++;

                        if (!isGroupCaptured)
                        {
                            isGroupCaptured = true;
                            groupsCaptured++;
                        }

                        if (!hasAnyCellBeenCaptured)
                        {
                            hasAnyCellBeenCaptured = true;
                            dist = Vector3.Distance(c.transform.position, hitPos);
                        }
                    }

                    if (!sibling.captured)
                    {
                        allSame = false;
                    }
                }
                c.enclosed = allSame;
            }
            var animateSpeed = Mathf.Lerp(5f, (cellGenerator.size.x + cellGenerator.size.y)*1.1f + 5f, _totalCellsCaptured / (cellGenerator.size.x * cellGenerator.size.y * .7f));
            c.Animate(animateSpeed, dist / 30);
            c.targetMaterial = targetMaterial;
        }

        //Calculate Score Based on cells Captured
        int cellsCaptured = _totalCellsCaptured - initialCellsCaptured;
        _scoreTotal += ((isMovesRemainingSet ? movesRemaining : 10) * groupsCaptured) + cellsCaptured;
    }

    private void UpdateUI()
    {
        movesRemainingLabel.gameObject.SetActive(isMovesRemainingSet);
        movesRemainingLabel.gameObject.SetActive(isMovesRemainingSet);

        scoreTotal.GetComponent<Text>().text = _scoreTotal.ToString();

        //If Moves Remaining is set, update text
        movesRemainingLabel.GetComponent<Text>().text = movesRemaining.ToString();
    }
    private void CheckGameOver()
    {
        //This is one possible Game over Scenario
        if (isMovesRemainingSet && movesRemaining == 0)
        {
            _gameState = gameState.gameOver;
            popupCanvasText.GetComponent<Text>().text = "Out Of Moves!";
            popupCanvasText.GetComponent<Text>().color = Color.red;
            popupCanvas.gameObject.SetActive(true);
        }
    }
    private void CheckWin()
    {
        if (_totalCellsCaptured == _totalCellsToCapture)
        {
            _gameState = gameState.gameOver;
            _scoreTotal += movesRemaining * 100;
            UpdateUI();
            popupCanvasText.GetComponent<Text>().text = "You Win!";
            popupCanvasText.GetComponent<Text>().color = Color.green;
            popupCanvas.gameObject.SetActive(true);
        }
    }

    public void PauseGame(GameObject ExitButton)
    {
        _gameState = gameState.Paused;
        PauseButton.SetActive(false);
        popupCanvasText.GetComponent<Text>().text = "Paused";
        popupCanvasText.GetComponent<Text>().color = PauseButton.GetComponentInChildren<Image>().color;

        //Change Play Again Button to "New Game"
        //NewGameButton.GetComponent<Text>().text = "New Game";

        ExitButton.gameObject.SetActive(true);

        popupCanvas.gameObject.SetActive(true);
    }

    public void ResumeGame(GameObject ExitButton)
    {
        _gameState = gameState.Playing;
        PauseButton.SetActive(true);
        popupCanvas.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(false);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public LevelLoader levelLoader;

    public Camera gameCamera;
    private Vector3 targetCameraPosition;
    private Vector3 CameraVelocity;

    //Make this section into Prefab with a Manager Script
    public Canvas popupCanvas;
    public GameObject popupCanvasText;
    public GameObject PauseButton;

    //Moves-related objects
    public int movesRemaining;
    public GameObject movesRemainingTextbox;

    public GameObject scoreTotalTextbox;
    public GameState gameState { get; private set; }

    public int totalCellsCaptured => levelLoader.Cells.Where(c => c.captured).Count();
    public int totalCellsEnclosed => levelLoader.Cells.Where(c => c.enclosed).Count();
    public int totalCellsToCapture => levelLoader.Cells.Where(c => c.canCapture).Count();
    public int scoreTotal { get; private set; }
    private bool isMovesRemainingSet;

    public enum GameState
    {
        Playing,
        Paused,
        gameOver
    }

    public struct UpdateInfo
    {
        public List<Cell> capturedCells { get; set; }
        public Material targetMaterial { get; set; }
        public Vector3 hitPos { get; set; }
    }

    void Awake()
    {
        //initialize moves Remaining 
        isMovesRemainingSet = movesRemaining >= 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Playing;
        popupCanvas.gameObject.SetActive(false);

        UpdateUI();

        CameraVelocity = Vector3.zero;
        targetCameraPosition = new Vector3(-.5f, levelLoader.MapHeight + .5f, levelLoader.MapWidth * -1.5f);
    }

    public void UpdateUI()
    {
        movesRemainingTextbox.gameObject.SetActive(isMovesRemainingSet);

        scoreTotalTextbox.GetComponent<Text>().text = scoreTotal.ToString();

        //If Moves Remaining is set, update text
        movesRemainingTextbox.GetComponent<Text>().text = movesRemaining.ToString();
    }

    public void CheckGameOver()
    {
        //This is one possible Game over Scenario
        if (isMovesRemainingSet && movesRemaining <= 0)
        {
            gameState = GameState.gameOver;
            popupCanvasText.GetComponent<Text>().text = "Out Of Moves!";
            popupCanvasText.GetComponent<Text>().color = Color.red;
            popupCanvas.gameObject.SetActive(true);
        }
    }

    public void CheckWin()
    {
        if (totalCellsCaptured == totalCellsToCapture)
        {
            gameState = GameState.gameOver;
            scoreTotal += movesRemaining * 100;
            UpdateUI();
            popupCanvasText.GetComponent<Text>().text = "You Win!";
            popupCanvasText.GetComponent<Text>().color = Color.green;
            popupCanvas.gameObject.SetActive(true);
        }
    }

    public bool CanCaptureCell(Cell cell, Material targetMaterial)
    {
        if (!cell.canCapture || cell.captured) return false;
        return cell.GetComponent<Renderer>().material.color == targetMaterial.color;
    }

    public bool CaptureMatchingSiblings(ref UpdateInfo info, Cell cell)
    {
        bool allSame = true;
        var siblings = levelLoader.GetNeighborCells(cell);
        foreach (Cell sibling in siblings.Where(s => !s.captured))
        {
            if (CanCaptureCell(sibling, info.targetMaterial))
            {
                sibling.Capture();
                CaptureMatchingSiblings(ref info, sibling);
                if (sibling.enclosed) scoreTotal += 100;
                levelLoader.Cells[sibling.cellIndex] = sibling;
                //info.capturedCells.Add(cell);
            }

            if (!sibling.captured && sibling.canCapture)
            {
                allSame = false;
            }
        }

        if (allSame) cell.Enclose();

        return cell;
    }

    public float GetAnimateSpeed(UpdateInfo info, Cell cell)
    {
        //Calculate AnimationSpeed
        var initialSpeed = 7f;
        var animateSpeed = Mathf.Lerp(
            a: initialSpeed,
            b: initialSpeed + (levelLoader.MapWidth + levelLoader.MapHeight) * 1.3f,
            t: totalCellsCaptured / totalCellsToCapture * .7f
        );

        //Vector3 interpolatedPosition = Vector3.Lerp(Vector3.up, Vector3.forward, animateSpeed);
        //Debug.DrawLine(Vector3.zero, interpolatedPosition, Color.yellow);

        return animateSpeed;
    }

    public void UpdateGrid(Vector3 hitPos, Material targetMaterial, out int cellsCaptured, out int cellsEnclosed)
    {
        cellsCaptured = 0;
        cellsEnclosed = 0;
        if (gameState != GameState.Playing) return; //only Update Grid when game is playing

        //declare variables for updating Grid
        int initialCellsCaptured = totalCellsCaptured;
        int initialCellsEnclosed = totalCellsEnclosed;
        var info = new UpdateInfo()
        {
            capturedCells = levelLoader.Cells.Where(c => c.captured).ToList(),
            targetMaterial = targetMaterial,
            hitPos = hitPos
        };

        //loop through grid
        for(int i=0;i<info.capturedCells.Count();i++)
        {
            //declare variables for updating cell
            Cell cell = info.capturedCells[i];
            //Update Cell
            if (!cell.enclosed) //skip cells that are surrounded by captured cells
            {
                CaptureMatchingSiblings(ref info, cell);
            }

            //animate cell coloring
            CellColor cellColor = cell.GetComponent<CellColor>();
            if (cellColor != null)
            {
                var animateSpeed = GetAnimateSpeed(info, cell);
                cellColor.targetMaterial = info.targetMaterial;

                float dist = Vector3.Distance(cell.transform.position, info.hitPos);
                cellColor.StartAnimating(animateSpeed, dist / 30);
            }
        }

        //calculate cells captured
        cellsCaptured = totalCellsCaptured - initialCellsCaptured;
        cellsEnclosed = totalCellsEnclosed - initialCellsEnclosed;
    }

    public bool TryGetRayCastHitOnMouseButtonDown(out RaycastHit info)
    {
        info = new RaycastHit();
        if(Input.GetMouseButtonDown(0))
        {
            var ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out info);
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != GameState.Playing) return;

        //Update Camera Position
        gameCamera.transform.localPosition = Vector3.SmoothDamp(gameCamera.transform.localPosition, targetCameraPosition, ref CameraVelocity, 0.3f);

        RaycastHit info;
        if(TryGetRayCastHitOnMouseButtonDown(out info))
        {
            var targetMaterial = info.transform.GetComponent<Renderer>().material;
            var targetCanCapture = info.transform.GetComponent<Cell>().canCapture;
            Material CapturedMaterial = levelLoader.Cells.FirstOrDefault(c => c.captured).GetComponent<Renderer>().material;
            if (CapturedMaterial != targetMaterial && targetCanCapture)
            {
                if (isMovesRemainingSet) movesRemaining--;

                int cellsCaptured, cellsEnclosed = 0;
                UpdateGrid(info.transform.position, targetMaterial, out cellsCaptured, out cellsEnclosed);
                scoreTotal += 10 * cellsCaptured + 45 * cellsEnclosed;

                CheckGameOver();
                CheckWin();
                UpdateUI();
            }
        }

    }

    public void PauseGame(GameObject ExitButton)
    {
        gameState = GameState.Paused;
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
        gameState = GameState.Playing;
        PauseButton.SetActive(true);
        popupCanvas.gameObject.SetActive(false);
        ExitButton.gameObject.SetActive(false);
    }
}

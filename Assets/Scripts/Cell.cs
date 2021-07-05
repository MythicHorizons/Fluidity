using System;
using UnityEngine;
using System.Collections;
using static Game;

/// <summary>
/// Defines a behavior that represents a cell that can be flooded.
/// </summary>
public class Cell : MonoBehaviour
{
    public bool canCapture { get; set; }

    /// <summary>
    /// Whether the cell has been captured by the player.
    /// </summary>
    public bool captured { get; private set; }

    /// <summary>
    /// Whether the cell has been captured and enclosed by other captured cells.
    /// </summary>
    public bool enclosed { get; private set; }

    public Vector2 cellPos { get; private set; }
    public int cellIndex { get; set; }
    public void SetCellPos(int posX, int PosY) { cellPos = new Vector2(posX, PosY); }

    public bool TryGetCellColor(out CellColor cellColor)
    {
        cellColor = GetComponent<CellColor>();
        return cellColor != null;
    }

    public void Capture(bool sendNotifications = true)
    {
        if (!canCapture || captured) return;

        captured = true;

        //Notify all other Components with INotifyOnCellCaptured 
        if (sendNotifications)
        {
            var comps = this.GetComponents<INotifyOnCellCaptured>();
            foreach (var comp in comps)
            {
                comp.NotifyCapture();
            }
        }
    }

    public void Enclose()
    {
        //No contextual validation here

        enclosed = true;

        //No notification at this time
    }

    void Awake()
    {
        //Initialize the ICell properties
        canCapture = true;
        captured = false;
        enclosed = false;
    }

    void Update()
    {
        
    }

    //public GameObject GenerateColorCell()
    //{
    //    var obj = new GameObject();
    //    obj.AddComponent<Cell>();
    //    obj.AddComponent<CellColor>();

    //    return obj;
    //}
}
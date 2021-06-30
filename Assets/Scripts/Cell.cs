using System;
using UnityEngine;
using System.Collections;

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
    public bool enclosed { get; set; }

    public Vector2 CellPos { get; private set; }
    public void SetCellPos(int posX, int PosY) { CellPos = new Vector2(posX, PosY); }

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
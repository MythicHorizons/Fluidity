using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Defines a behavior that represents a cell that can be flooded.
/// </summary>
public class Cell : MonoBehaviour, ICell
{
    public bool canCapture { get; set; }

    /// <summary>
    /// Whether the cell has been captured by the player.
    /// </summary>
    public bool captured { get; set; }

    /// <summary>
    /// Whether the cell has been captured and enclosed by other captured cells.
    /// </summary>
    public bool enclosed { get; set; }

    protected virtual void Init()
    {
        //Initialize the ICell properties
        canCapture = true;
        captured = false;
        enclosed = false;
    }

    protected virtual void OnUpdate()
    {
        
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        OnUpdate();
    }
}
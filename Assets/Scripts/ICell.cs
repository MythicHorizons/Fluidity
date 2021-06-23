using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the properties and methods for a cell
/// </summary>
public interface ICell
{
    public bool captured { get; set; }
    public bool enclosed { get; set; }
}

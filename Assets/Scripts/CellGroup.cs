using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Defines a class that represents a group of cells. 
/// This grouping does not have to extend in any general direction,
/// it only has to be a collection of cells.
/// In addition, Cells are not grouped by status. 
/// These groups are designed to represent how cells are placed in the world.
/// </summary>
/// <seealso cref="Cell"/>
public class CellGroup
{
    /// <summary>
    /// The list of cells that this group contains.
    /// </summary>
    public Cell[] cells { get; private set; }

    public Dictionary<Cell, Cell[]> siblingMap;

    public CellGroup(Cell[] cells, Dictionary<Cell, Cell[]> siblingMap)
    {
        this.cells = cells;
        this.siblingMap = siblingMap;
    }
}

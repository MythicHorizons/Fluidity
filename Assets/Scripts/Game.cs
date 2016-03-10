using UnityEngine;
using System.Collections;
using System.Linq;

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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info))
            {
                Cell hitCell = info.transform.GetComponent<Cell>();
                if (cellGenerator.generatedGroups[0].cells.All(c => c.animationState == CellAnimationState.normal || c == hitCell))
                {
                    Vector3 hitPos = info.transform.position;
                    Material targetMaterial = info.transform.GetComponent<Renderer>().material;
                    foreach (Cell c in cellGenerator.generatedGroups[0].cells)
                    {
                        float dist = Vector3.Distance(c.transform.position, hitPos);
                        c.animateOffset = dist / 30;
                        c.animationState = CellAnimationState.animating;

                        if (c.captured)
                        {
                            c.targetMaterial = targetMaterial;
                            if (!c.enclosed)
                            {
                                bool allSame = true;
                                var siblings = cellGenerator.generatedGroups[0].siblingMap[c].ToArray();
                                foreach (Cell sibling in siblings)
                                {
                                    if (sibling.GetComponent<Renderer>().material.color == targetMaterial.color)
                                    {
                                        sibling.targetMaterial = targetMaterial;
                                        sibling.captured = true;
                                    }

                                    if (!sibling.captured)
                                    {
                                        allSame = false;
                                    }
                                }

                                c.enclosed = allSame;
                            }
                        }
                    }
                }
            }
        }
    }
}

using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Defines a behavior that represents a cell that can be flooded.
/// </summary>
public class Cell : MonoBehaviour
{
    /// <summary>
    /// The current state that this cell is in.
    /// </summary>
    public CellAnimationState animationState = CellAnimationState.normal;

    /// <summary>
    /// Whether the cell has been captured by the player.
    /// </summary>
    public bool captured = false;

    /// <summary>
    /// Whether the cell has been captured and enclosed by other captured cells.
    /// </summary>
    public bool enclosed = false;

    /// <summary>
    /// The material that the cell should change to after being captured.
    /// </summary>
    public Material targetMaterial;

    public float animateSpeed = 5f;

    public float animateOffset = -1f;

    public float zMagnitude = 1f;

    void Start()
    {
        animationState = CellAnimationState.normal;
        enclosed = false;
    }

    void Update()
    {
        float targetZ = 0;
        if (animationState == CellAnimationState.hovered)
        {
            if (captured)
            {
                targetZ = -0.5f;
            }
            else
            {
                targetZ = -1f;
            }
            targetZ *= zMagnitude;
        }
        else if (animationState == CellAnimationState.animating)
        {
            targetZ = 1f * zMagnitude;
            if (Math.Abs(transform.position.z - targetZ) < 0.01f)
            {
                animationState = CellAnimationState.normal;
                if (targetMaterial != null)
                {
                    GetComponent<Renderer>().material = targetMaterial;
                    //captured = true;
                }
            }
        }
        else if (captured)
        {
            targetZ = 0.5f;
        }
        if (animateOffset >= 0)
        {
            animateOffset -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                Mathf.MoveTowards(transform.localPosition.z, targetZ, animateSpeed * Time.deltaTime));
        }
    }

    public void Animate(float animateSpeed=5f, float animateOffset=-1f)
    {
        this.animateSpeed = animateSpeed;
        this.animateOffset = animateOffset;
        if (this.animationState != CellAnimationState.animating) this.animationState = CellAnimationState.animating;
    }

    void OnMouseEnter()
    {
        if (animationState == CellAnimationState.normal && this.tag == "Paints")
        {
            animationState = CellAnimationState.hovered;
        }
    }

    void OnMouseExit()
    {
        if (animationState == CellAnimationState.hovered && this.tag == "Paints")
        {
            animationState = CellAnimationState.normal;
        }
    }

}
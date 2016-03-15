using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Defines a behavior that represents a cell that can be flooded.
/// </summary>
public class Cell : MonoBehaviour
{
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
            targetZ = -1f * zMagnitude;
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
                    captured = true;
                }
            }
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

    void OnMouseEnter()
    {
        if (animationState == CellAnimationState.normal)
        {
            animationState = CellAnimationState.hovered;
        }
    }

    void OnMouseExit()
    {
        if (animationState == CellAnimationState.hovered)
        {
            animationState = CellAnimationState.normal;
        }
    }

}
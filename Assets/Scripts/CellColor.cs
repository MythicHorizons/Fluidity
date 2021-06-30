using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellColor : MonoBehaviour, INotifyOnCellCaptured
{
    /// <summary>
    /// The current state that this cell is in.
    /// </summary>
    public CellAnimationState animationState { get; set; }

    /// <summary>
    /// The material that the cell should change to after being captured.
    /// </summary>
    public Material targetMaterial { get; set; }
    public float animateSpeed { get; set; }
    public float animateOffset { get; set; }
    public float zMagnitude { get; set; }

    private Cell cell;

    public void NotifyCapture()
    {
        
    }

    public void StartAnimating(float animateSpeed = 5f, float animateOffset = -1f)
    {
        ////Handle scenario where StartAnimating is called while already animating
        //if(this.animationState == CellAnimationState.animating)
        //{
        //    if (targetMaterial != null)
        //    {
        //        GetComponent<Renderer>().material = targetMaterial;
        //        transform.localPosition = new Vector3(
        //        transform.localPosition.x,
        //        transform.localPosition.y,
        //        GetTargetZOnNormal());
        //    }
        //}

        if (this.animationState != CellAnimationState.animating)
        {
            this.animateSpeed = animateSpeed;
            this.animateOffset = animateOffset;
            this.animationState = CellAnimationState.animating;
        }
    }

    protected virtual float GetTargetZOnHover()
    {
        var targetZ = (cell.captured) ? -0.5f : -1f;
        targetZ *= zMagnitude;
        return targetZ;
    }

    protected virtual float GetTargetZOnAnimating()
    {
        return 1f * zMagnitude;
    }

    protected virtual float GetTargetZOnNormal()
    {
        return 0.5f;
    }

    // Awake is called before the first frame update
    void Awake()
    {
        //Initialize IAnimateColorCell properties
        animationState = CellAnimationState.normal;
        animateSpeed = 0f;
        animateOffset = -1f;
        zMagnitude = -1f;

        cell = GetComponent<Cell>();
    }

    // Update is called once per frame
    void Update()
    {
        float targetZ = 0;
        switch (animationState)
        {
            case CellAnimationState.hovered:
                targetZ = GetTargetZOnHover();
                break;
            case CellAnimationState.animating:
                targetZ = GetTargetZOnAnimating();
                if(Math.Abs(transform.position.z - targetZ) < 0.01f)
                {
                    if (targetMaterial != null)
                    {
                        GetComponent<Renderer>().material = targetMaterial;
                    }
                    animationState = CellAnimationState.normal;
                }
                break;
            case CellAnimationState.normal:
                targetZ = GetTargetZOnNormal();
                break;
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

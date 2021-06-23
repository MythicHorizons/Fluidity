using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCell : Cell, IAnimateColorCell
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

    public void Animate(float animateSpeed = 5f, float animateOffset = -1f)
    {
        this.animateSpeed = animateSpeed;
        this.animateOffset = animateOffset;
        if (this.animationState != CellAnimationState.animating) this.animationState = CellAnimationState.animating;
    }

    protected override void Init()
    {
        //Initialize ICell properties
        base.Init();

        //Initialize IAnimateColorCell properties
        animationState = CellAnimationState.normal;
        animateSpeed = 5f;
        animateOffset = -1f;
        zMagnitude = -1f;
    }

    protected override void OnUpdate()
    {
        if (captured && targetMaterial != null)
        {
            GetComponent<Renderer>().material = targetMaterial;
        }
    }

    protected void OnMouseEnterAnimate()
    {
        if (animationState == CellAnimationState.normal)
        {
            animationState = CellAnimationState.hovered;
        }
    }

    protected void OnMouseExitAnimate()
    {
        if (animationState == CellAnimationState.hovered)
        {
            animationState = CellAnimationState.normal;
        }
    }

    protected virtual float OnUpdateAnimateStateHovered(float targetZ)
    {
        targetZ = (captured) ? -0.5f : -1f;
        targetZ *= zMagnitude;
        return targetZ;
    }

    protected virtual float onUpdateAnimateStateAnimating(float targetZ)
    {
        targetZ = 1f * zMagnitude;
        if (Math.Abs(transform.position.z - targetZ) < 0.01f)
        {
            animationState = CellAnimationState.normal;
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
        return targetZ;
    }

    protected virtual float onUpdateAnimateStateNormal(float targetZ)
    {
        targetZ = 0.5f;
        return targetZ;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        float targetZ = 0;
        switch (animationState)
        {
            case CellAnimationState.hovered:
                targetZ = OnUpdateAnimateStateHovered(targetZ);
                break;
            case CellAnimationState.animating:
                targetZ = onUpdateAnimateStateAnimating(targetZ);
                break;
            case CellAnimationState.normal:
                targetZ = onUpdateAnimateStateNormal(targetZ);
                break;
        }
        OnUpdate();
    }

    void OnMouseEnter()
    {
        OnMouseEnterAnimate();
    }

    void OnMouseExit()
    {
        OnMouseExitAnimate();
    }
}

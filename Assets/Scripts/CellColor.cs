using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellColor : MonoBehaviour
{
    public CellMouseState mouseState { get; set; }
    public CellDisplayState displayState { get; set; }

    /// <summary>
    /// The material that the cell should change to after being captured.
    /// </summary>
    public Material targetMaterial { get; set; }

    public float animateSpeed { get; set; }
    public float animateOffset { get; set; }
    public float zMagnitude { get; set; }

    private Cell cell;

    public enum CellMouseState
    {
        None,
        Hover
    }

    public enum CellDisplayState
    {
        None,
        Wave,
        ChangingColor
    }

    public void StartAnimating(float animateSpeed = 1f, float animateOffset = -1f)
    {
        if (this.displayState == CellDisplayState.None)
        {
            this.animateSpeed = animateSpeed;
            this.animateOffset = animateOffset;
            this.displayState = CellDisplayState.Wave;
        }
    }

    // Awake is called before the first frame update
    void Start()
    {
        mouseState = CellMouseState.None;
        displayState = CellDisplayState.None;
        animateSpeed = 5f;
        animateOffset = -1f;

        cell = GetComponent<Cell>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animateOffset >= 0)
        {
            animateOffset -= Time.deltaTime;
        }
        else
        {
            float targetZ = 0;
            if (mouseState == CellMouseState.Hover)
            {
                targetZ = (cell.captured) ? -0.5f : -1f;
            }
            else if (mouseState == CellMouseState.None)
            {
                targetZ = (cell.captured) ? 0.5f : 0f;
            }

            if(displayState == CellDisplayState.Wave)
            {
                targetZ = 1f;
                if (Math.Abs(transform.localPosition.z - targetZ) < 0.01f)
                {
                    displayState = CellDisplayState.ChangingColor;
                }
            }

            if (displayState == CellDisplayState.ChangingColor)
            {
                displayState = CellDisplayState.None;
                if (targetMaterial != null)
                {
                    GetComponent<Renderer>().material = targetMaterial;
                }
            }

            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.y,
                Mathf.MoveTowards(transform.localPosition.z, targetZ, animateSpeed * Time.deltaTime));
        }
    }

    void OnMouseEnter()
    {
        mouseState = CellMouseState.Hover;
    }

    void OnMouseExit()
    {
        mouseState = CellMouseState.None;
    }
}

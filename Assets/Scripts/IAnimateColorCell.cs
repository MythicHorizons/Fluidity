using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimateColorCell
{
    public CellAnimationState animationState { get; set; }
    public Material targetMaterial { get; set; }
    public float animateSpeed { get; set; }
    public float animateOffset { get; set; }
    public float zMagnitude { get; set; }
    public void Animate(float animateSpeed, float animateOffset);
}

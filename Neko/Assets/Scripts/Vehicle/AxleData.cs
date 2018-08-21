using System;
using UnityEngine;

[Serializable]
public class AxleData
{
    public GameObject LeftWheel;
    public WheelCollider LeftWheelCollider;

    public GameObject RightWheel;
    public WheelCollider RightWheelCollider;

    public bool Motor;
    public bool Steering;
}
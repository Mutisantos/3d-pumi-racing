using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CarTracePosition 
{

    public Vector3 Position;
    public Quaternion Rotation;
    public float Speed;

    public CarTracePosition(Vector3 position, Quaternion rotation, float speed)
    {
        Position = position;
        Rotation = rotation;
        Speed = speed;
    }


}

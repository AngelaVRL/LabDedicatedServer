using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class PositionData
{
    public float posX;
    public float posY;
    public float posZ;

    public static PositionData FromVector(Vector3 position)
    {
        return new PositionData
        {
            posX = position.x,
            posY = position.y,
            posZ = position.z
        };
    }

    public Vector3 ToVector()
    {
        return new Vector3(posX, posY, posZ);
    }
}
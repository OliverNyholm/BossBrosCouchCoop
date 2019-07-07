using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorXZ
{
    public float x;
    public float y;
    public float z;

    public VectorXZ(Vector3 aVector3)
    {
        x = aVector3.x;
        y = 0.0f;
        z = aVector3.z;
    }

    public VectorXZ(float aX, float aZ)
    {
        x = aX;
        y = 0.0f;
        z = aZ;
    }

    public static Vector3 operator -(VectorXZ aLeft, VectorXZ aRight)
    {
        Vector3 returnData;

        returnData.x = aLeft.x - aRight.x;
        returnData.y = 0.0f;
        returnData.z = aLeft.z - aRight.z;

        return returnData;
    }

    public static Vector3 operator -(VectorXZ aLeft, Vector3 aRight)
    {
        Vector3 returnData;

        returnData.x = aLeft.x - aRight.x;
        returnData.y = 0.0f;
        returnData.z = aLeft.z - aRight.z;

        return returnData;
    }
}

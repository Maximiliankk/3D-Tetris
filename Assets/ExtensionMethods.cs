using UnityEngine;
using System.Collections;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class ExtensionMethods
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static float LookTowardOnAxis(this Transform t, Vector3 rotateThis, Vector3 aroundThis, Vector3 towardThis)
    {
        float angle = GetAngleLookTowardOnAxis(t.position, rotateThis, aroundThis, towardThis);

        // apply the rotation in worldspace because all our parameters are in worldspace
        t.Rotate(aroundThis, angle, Space.World);

        return angle;
    }

    public static float GetAngleLookTowardOnAxis(Vector3 transformPosition, Vector3 rotateThis, Vector3 aroundThis, Vector3 towardThis)
    {
        // this vector is needed to decide whether to rotate with a positive or negative angle
        Vector3 crossed = Vector3.Cross(rotateThis, aroundThis);

        // this points toward the target and will be projected
        Vector3 projected = towardThis - transformPosition;

        // now it is projected into the plane defined by the normal vector aroundThis
        projected = Vector3.ProjectOnPlane(projected, aroundThis);

        // if this is zero, angle is zero and we don't need to rotate
        float dotProduct = Vector3.Dot(crossed, projected);

        // now we just need to get the angle between two vectors which are now in the same plane
        float angle = 0;
        if (dotProduct > 0)
        {
            angle = -Vector3.Angle(rotateThis, projected);
        }
        else if (dotProduct < 0)
        {
            angle = Vector3.Angle(rotateThis, projected);
        }

        return angle;
    }
}

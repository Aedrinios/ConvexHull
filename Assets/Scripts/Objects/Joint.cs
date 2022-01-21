using UnityEngine;

public class Joint
{
    public Vector3 pos;
    public int nbJunctions = 0;

    public Joint(Vector3 position)
    {
        pos = position;
        nbJunctions = 1;
    }
}
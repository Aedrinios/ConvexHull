using UnityEngine;

public class Matrix3x3
{
    private float[] tab;

    public Matrix3x3(float x1, float x2, float x3, float y1, float y2, float y3, float z1, float z2, float z3)
    {
        this.tab = new float[] { x1, x2, x3, y1, y2, y3, z1, z2, z3 };
    }

    public Matrix3x3() : this(0, 0, 0, 0, 0, 0, 0, 0, 0){ }

    public float this[int i]
    {
        get => this.tab[i];
        set => this.tab[i] = value;
    }
    
    public float this[int x, int y]
    {
        get => this.tab[x * 3 + y];
        set => this.tab[x * 3 + y] = value;
    }
    
    public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector) 
        => new Vector3(
            matrix.tab[0] * vector.x + matrix.tab[1] * vector.y + matrix.tab[2] * vector.z,
            matrix.tab[3] * vector.x + matrix.tab[4] * vector.y + matrix.tab[5] * vector.z,
            matrix.tab[6] * vector.x + matrix.tab[7] * vector.y + matrix.tab[8] * vector.z
        );
}
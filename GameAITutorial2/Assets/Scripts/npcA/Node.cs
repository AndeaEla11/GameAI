using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPos;
    public int x, z; 

    public Node(bool walkable, Vector3 worldPos, int x, int z)
    {
        this.walkable = walkable;
        this.worldPos = worldPos;
        this.x = x;
        this.z = z;
    }
}

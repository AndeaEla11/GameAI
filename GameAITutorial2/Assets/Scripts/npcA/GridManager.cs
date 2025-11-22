using UnityEngine;
using System.Collections.Generic; 

public class GridManager : MonoBehaviour
{
    public int gridWidth = 50; 
    public int gridHeight = 50;
    public float cellSize = 1f; 
    public Vector3 origin = Vector3.zero;
    public LayerMask obstacleMask;

    public Node[,] grid;

    private void Awake()
    {
        CreateGrid(); 
    }

    void CreateGrid()
    {
        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        for (int z = 0; z < gridHeight; z++)
        {
                Vector3 p = origin + new Vector3(x * cellSize + cellSize * 0.5f, 0f, z * cellSize + cellSize * 0.5f);

                bool walkabel = !Physics.CheckBox(p + Vector3.up * 0.5f, new Vector3(cellSize * 0.45f,0.6f, cellSize * 0.45f), Quaternion.identity, obstacleMask);

                grid[x,z] = new Node(walkabel, p, x, z);

        }
    }

    public Node WorldToNode(Vector3 worldPos)
    {
        float px = Mathf.Clamp01((worldPos.x - origin.x) / (gridWidth * cellSize)); 
        float pz = Mathf.Clamp01((worldPos.z - origin.z) / (gridHeight * cellSize));
        int x = Mathf.Clamp(Mathf.FloorToInt(gridWidth * px), 0, gridWidth - 1);
        int z = Mathf.Clamp(Mathf.FloorToInt(gridHeight * pz), 0, gridHeight - 1);
        return grid[x,z];

    }

    public IEnumerable<Node> GetNeighbours(Node n)
    {
        int x = n.x, z = n.z;

        bool left = x > 0 && grid[x - 1, z].walkable;
        bool right = x < gridWidth - 1 && grid[x + 1, z].walkable;
        bool down = z > 0 && grid[x, z - 1].walkable;
        bool up = z < gridHeight - 1 && grid[x, z + 1].walkable;

        if (left) yield return grid[x - 1, z];
        if (right) yield return grid[x + 1, z];
        if (down) yield return grid[x, z - 1];
        if (up) yield return grid[x, z + 1];

        if (x > 0 && z > 0 && left && down && grid[x - 1, z - 1].walkable)
            yield return grid[x - 1, z - 1];
        if (x > 0 && z < gridHeight - 1 && left && up && grid[x - 1, z + 1].walkable)
            yield return grid[x - 1, z + 1];
        if (x < gridWidth - 1 && z > 0 && right && down && grid[x + 1, z - 1].walkable)
            yield return grid[x + 1, z - 1]; 
        if (x < gridWidth - 1 && z < gridHeight - 1 && right && up && grid[x + 1, z + 1].walkable)
            yield return grid[x + 1, z + 1];
    }

}



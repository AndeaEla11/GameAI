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
    public bool[,] occupied; 

    private void Awake()
    {
        CreateGrid(); 
    }

    void CreateGrid()
    {
        grid = new Node[gridWidth, gridHeight];
        occupied = new bool[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        for (int z = 0; z < gridHeight; z++)
        {
                Vector3 p = origin + new Vector3(x * cellSize + cellSize * 0.5f, 0f, z * cellSize + cellSize * 0.5f);

                bool walkabel = !Physics.CheckBox(p + Vector3.up * 0.5f, new Vector3(cellSize * 0.5f, 0.5f, cellSize * 0.5f), Quaternion.identity, obstacleMask);

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

    public bool IsOccupied(Node n)
    {
        return occupied[n.x, n.z]; 
    }

    public void SetOccupied(Node n, bool value)
    {
        occupied[n.x, n.z] = value;
    }

    public List<Node> GetNeighbours(Node n)
    {
        List<Node> neighbours = new List<Node>();

        int x = n.x;
        int z = n.z;

        bool left = x > 0 && grid[x - 1, z].walkable;
        bool right = x < gridWidth - 1 && grid[x + 1, z].walkable;
        bool down = z > 0 && grid[x, z - 1].walkable;
        bool up = z < gridHeight - 1 && grid[x, z + 1].walkable;

        //4-direction neighbours
        if (left) neighbours.Add(grid[x - 1, z]);
        if (right) neighbours.Add(grid[x + 1, z]);
        if (down) neighbours.Add(grid[x, z - 1]);
        if (up) neighbours.Add(grid[x, z + 1]);

        //corner cutting prevention
        if (x > 0 && z > 0 && left && down && grid[x - 1, z - 1].walkable)
            neighbours.Add(grid[x - 1, z - 1]);

        if (x > 0 && z < gridHeight - 1 && left && up && grid[x - 1, z + 1].walkable)
            neighbours.Add(grid[x - 1, z + 1]);

        if (x < gridWidth - 1 && z > 0 && right && down && grid[x + 1, z - 1].walkable)
            neighbours.Add(grid[x + 1, z - 1]);

        if (x < gridWidth - 1 && z < gridHeight - 1 && right && up && grid[x + 1, z + 1].walkable)
            neighbours.Add(grid[x + 1, z + 1]);

        return neighbours;
    }

}
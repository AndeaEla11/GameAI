using UnityEngine;
using System.Collections.Generic;


public class npcABFSController : MonoBehaviour
{
    public GridManager grid;
    public Transform target;
    public float moveSpeed = 3f;
    public float repathEvery = 0.5f;

    Rigidbody rb;
    List<Vector3> path;
    int idx;
    float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        if (grid == null || target ==  null)
            return;

        timer -= Time.deltaTime; ;
        if (timer < 0f)
        {
            path = BFSPath(grid, transform.position, target.position);
            idx = 0;
            timer = repathEvery;
        }
    }

    private void FixedUpdate()
    {
        if (path == null || idx >= path.Count)
            return; 

        Vector3 goal = path[idx];
        Vector3 to = goal - transform.position;
        to.y = 0f; 

        if (to.magnitude < 0.1f)
        {
            idx++;
            return; 
        }

        Vector3 step = to.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + step);

        if (to != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(to);
    }

    List<Vector3> BFSPath (GridManager g, Vector3 startW, Vector3 targetW)
    {
        Node start = g.WorldToNode(startW);
        Node goal = g.WorldToNode(targetW);
        if (!goal.walkable) 
            return null;

        var q = new Queue<Node>();
        var cameFrom = new Dictionary<Node, Node>();
        var visited = new HashSet<Node>();

        q.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = null; 

        while (q.Count > 0)
        {
            Node cur = q.Dequeue();
            if (cur == goal) 
                break;

            foreach (var nb in g.GetNeighbours(cur))
            {
                if (!nb.walkable || visited.Contains(nb)) 
                    continue;
                visited.Add(nb);
                cameFrom[nb] = cur;
                q.Enqueue(nb);
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null;

        var rev = new List<Vector3>();
        Node c = goal;
        while (c != null && c != start)
        {
            rev.Add(c.worldPos);
            c = cameFrom[c];
        }
        rev.Reverse();
        return rev;
    }

}

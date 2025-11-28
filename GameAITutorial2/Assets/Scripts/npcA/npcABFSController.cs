using UnityEngine;
using UnityEngine.UI;
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

    public float detectionRange = 10f;
    public enum State { Patrol, Chase, Attack}
    public State state = State.Patrol;
    public Text stateText;

    public float stopDistance = 5f;
    public GameObject bulletPrefab;   
    public Transform firePoint;       
    public float bulletSpeed = 16f;
    public float shootRange = 8f; 
    public float shootCooldown = 0.6f;
    float shootTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        var startNode = grid.WorldToNode(transform.position);
        grid.SetOccupied(startNode, true);
        SetState(State.Patrol);
    }

    private void Update()
    {
        if (grid == null || target ==  null)
            return;

        timer -= Time.deltaTime;

        if (timer < 0f)
        {
            path = BFSPath(grid, transform.position, target.position);
            idx = 0;
            timer = repathEvery;

            if 
                (path != null && path.Count > 0) 
                SetState(State.Chase);

            else 
                SetState(State.Patrol);
        }

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > detectionRange)
        {
            path = null;
            SetState(State.Patrol);
            return;
        }
    }

    private void FixedUpdate()
    {
        shootTimer -= Time.fixedDeltaTime;

        if (target)
        {
            Vector3 toPlayerMove = target.position - transform.position;
            toPlayerMove.y = 0f;

            if (toPlayerMove.magnitude <= stopDistance && shootTimer <= 0f)
            {
                SetState(State.Attack);


                if (toPlayerMove.sqrMagnitude > 0.0001f)
                {
                    float yaw = Quaternion.LookRotation(toPlayerMove).eulerAngles.y;
                    transform.rotation = Quaternion.Euler(0f, yaw, 0f);
                }

                if (bulletPrefab && firePoint)
                {
                    GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                    
                    var brb = b.GetComponent<Rigidbody>();
                    if (brb)
                    {
                        Vector3 dir = toPlayerMove.normalized;
                        brb.linearVelocity = (toPlayerMove.normalized) * bulletSpeed;
                    }
                }

                shootTimer = shootCooldown;
                return;

            }
        }

        if (path == null || idx >= path.Count)
        {
            SetState(State.Patrol);
            return;
        }


        Node currentNode = grid.WorldToNode(transform.position);
        Vector3 goal = path[idx];
        Node nextNode = grid.WorldToNode(goal);

        if (grid.IsOccupied(nextNode) && nextNode != currentNode)
        {
            timer = 0f;
            return;
        }

        Vector3 to = goal - transform.position;
        to.y = 0f;

        if (to.magnitude < 0.1f)
        {
            grid.SetOccupied(currentNode, false);
            grid.SetOccupied(nextNode, true);

            idx++;
            return;
        }

        Vector3 step = to.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + step);

        if (to.sqrMagnitude > 0.0001f)
        { 
            float yaw = Quaternion.LookRotation(to).eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
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


    void SetState(State s) 
    { 
        state = s; 
        if (stateText) stateText.text = s.ToString().ToUpper(); 
    }

}

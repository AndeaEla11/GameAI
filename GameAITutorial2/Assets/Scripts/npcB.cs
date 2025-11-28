using UnityEngine.UI; 
using UnityEngine;
using UnityEngine.AI;


public class npcB : MonoBehaviour
{
    
    public Transform target;
    public float repathEvery = 0.5f;
    public float attackRange = 1.8f;
    public int damagePerHit = 5;
    public float attackCooldown = 0.7f;
    public float detectionRange = 10f; 
    public LayerMask obstacleMask; 

    NavMeshAgent agent;
    float repathTimer;
    float attackTimer;

    public enum npcBState { Patrol, Chase, Attack, Dead }
    public npcBState state = npcBState.Patrol;
    public Text stateText;

    public bool useWaypoints = true;
    public Transform[] waypoints;
    public float waypointTolerance = 0.5f;
    int wpIndex;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        
    }

    
    void Update()
    {
       if (target == null)
            return;

        Vector3 toPlayer = target.position - transform.position;
        float distSq = toPlayer.sqrMagnitude;
        float detectSq = detectionRange * detectionRange;
        bool inRange = distSq <= detectSq;

        if (!inRange)
        {
            state = npcBState.Patrol;
            PatrolTick();
            UpdateStateUI();
            return;
        }

        state = npcBState.Chase;

        agent.isStopped = false;

        repathTimer -= Time.deltaTime;
        if (repathTimer < 0f)
        {
            agent.SetDestination(target.position);
            repathTimer = repathEvery;
        }

        attackTimer -= Time.deltaTime;
        if (!agent.pathPending && agent.remainingDistance <= attackRange && attackTimer <= 0f && HasLineOfSight())
        {
            state = npcBState.Attack;

            var player = target.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamege(damagePerHit);
                attackTimer = attackCooldown;
            }
        }
        UpdateStateUI(); 
    }

    private void UpdateStateUI()
    {

        if (stateText != null)
            stateText.text = state.ToString().ToUpper();
    }

    bool HasLineOfSight()
    {
        Vector3 from = transform.position + Vector3.up * 0.5f;
        Vector3 to = target.position + Vector3.up * 0.5f;
        Vector3 dir = to - from;
        float dist = dir.magnitude; 
        dir /= dist;

        if (Physics.Raycast(from, dir, out RaycastHit hit, dist, obstacleMask))
            return false;

        return true;
    }


    void OnEnable()
    {
        wpIndex = 0;
        if (useWaypoints && waypoints != null && waypoints.Length > 0)
            agent.SetDestination(waypoints[wpIndex].position);
    }


    void PatrolTick()
    {
        if (!useWaypoints || waypoints == null || waypoints.Length == 0)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        
        if (!agent.hasPath)
            agent.SetDestination(waypoints[wpIndex].position);

        
        if (!agent.pathPending &&
            agent.remainingDistance <= Mathf.Max(waypointTolerance, agent.stoppingDistance))
        {
            wpIndex = (wpIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[wpIndex].position);
        }
    }

}

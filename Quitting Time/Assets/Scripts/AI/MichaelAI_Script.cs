using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public class MichaelAI_Script : MonoBehaviour
{
    [Header ("Chase Settings")]
    public Transform PlayerTarget;
    public float AttackDistance = 2f;
    public float chaseSpeed = 6f;

    [Header("Patrol Settings")]
    public Transform[] patrolWaypoints;
    public float patrolSpeed = 3f;
    public float waypointPauseTime = 1f;
    private int currentWaypoint = 0;
    private float waypointWaitTimer;

    [Header("Detection Settings")]
    public float visionRange = 15f;
    public float visionAngle = 110f;
    public float proximityRadius = 3f;

    [Header("Investigation Settings")]
    public float investigationTime = 10f;
    public float investigationRadius = 2f;
    private Vector3 lastknownPosition;
    private float investigationTimer;
    private bool isInvestigating;

    [Header("Chase Termination")]
    public float chaseEndDistance = 20f;
    public bool IsChasing { get; private set; }

    private NavMeshAgent m_Agent;
    private float m_Distance;
    private bool isChasing;
    private bool hasDamaged;
    
    public PlayerHealthValue health;
    public FP_Movement playerMovement;
    public float damageCooldown = 1f;
    private float damageTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        health.damageValue = 1f;
        m_Agent.speed = patrolSpeed;

        if(patrolWaypoints.Length > 0)
        {
            m_Agent.SetDestination(patrolWaypoints[currentWaypoint].position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_Distance = Vector3.Distance(transform.position, PlayerTarget.position);
        bool canSeePlayer = CheckPlayerDetection();

        if(IsChasing && m_Distance > chaseEndDistance)
        {
            IsChasing = false;
            HandlePatrolBehaviour();
            return;
        }

        if(canSeePlayer || isChasing)
        {
            IsChasing = true;
            HandleChaseBehaviour();
            HandleAttack();
        }
        
        else
        {
            HandlePatrolBehaviour();
        }

        if(playerMovement.isHidden && !isInvestigating)
        {
            StartInvestigation(PlayerTarget.position);
        }

        if(isInvestigating)
        {
            HandleInvestigation();
            return;
        }
        
    }

    bool CheckPlayerDetection()
    {
        if (isChasing) return true;
        Vector3 directionToPlayer = PlayerTarget.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);

        int layerMask = ~(1 << LayerMask.NameToLayer("Hidden"));

        if (angle < visionAngle/2 && directionToPlayer.magnitude < visionRange)
        {
            if(Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, visionRange, layerMask))
            {
                if(hit.transform == PlayerTarget) return true;
            }
        }

        if (m_Distance < proximityRadius) return true;
        if (playerMovement.isHidden) return false;

        return CheckProximity();
    }

    bool CheckProximity()
    {

        int layerMask = ~(1 << LayerMask.NameToLayer("Hidden"));
        if (Vector3.Distance(transform.position, PlayerTarget.position) < proximityRadius)
        {
            Vector3 directionToPlayer = PlayerTarget.position - transform.position;
            if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, proximityRadius, layerMask))
            {
                return hit.transform == PlayerTarget;
            }
        }
        return false;
    }

    void StartInvestigation(Vector3 position)
    {
        isInvestigating = true;
        investigationTimer = investigationTime;
        lastknownPosition = position;
        m_Agent.SetDestination(position);
    }

    void HandleInvestigation()
    {
        if (m_Agent.remainingDistance < 0.5f)
        {
            investigationTimer -= Time.deltaTime;

            transform.Rotate(0, 45 * Time.deltaTime, 0);

            if(investigationTimer <= 0)
            {
                isInvestigating = false;
                HandlePatrolBehaviour();
            }
        }
    }

    void HandleChaseBehaviour()
    {
        IsChasing = true;
        m_Agent.speed = chaseSpeed;
        m_Agent.SetDestination(PlayerTarget.position);
        m_Agent.isStopped = false;
    }

    void HandlePatrolBehaviour()
    {
        IsChasing = false;
        m_Agent.speed = patrolSpeed;

        if (m_Agent.remainingDistance < 1f && !m_Agent.pathPending)
        {
            if (waypointWaitTimer <= 0)
            {
                currentWaypoint = (currentWaypoint + 1) % patrolWaypoints.Length;
                m_Agent.SetDestination(patrolWaypoints[currentWaypoint].position);
                waypointWaitTimer = waypointPauseTime;
            }
            else
            {
                waypointWaitTimer -= Time.deltaTime;
            }
        }
    }

    void HandleAttack()
    {
        if (m_Distance < AttackDistance && !playerMovement.isHidden)
        {
            m_Agent.isStopped = true;
        
            if (!hasDamaged)
            {
                health.healthValue -= health.damageValue;
                hasDamaged = true;
                damageTimer = damageCooldown;
                playerMovement.ResetHealingTimer();
            }

            if (hasDamaged)
            {
                damageTimer -= Time.deltaTime;
                if (damageTimer <= 0)
                {
                    hasDamaged = false;
                }
            }
        }
        else
        {
            m_Agent.isStopped = false;
        }
    }

    void OnDrawGizmos()
    {
        
        Gizmos.color = Color.yellow;
        Vector3 leftBound = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * visionRange;
        Vector3 rightBound = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * visionRange;
        Gizmos.DrawRay(transform.position, leftBound);
        Gizmos.DrawRay(transform.position, rightBound);

        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, proximityRadius);
    }
}

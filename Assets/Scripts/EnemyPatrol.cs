using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    // Dictates whether the agent waits on each node
    [SerializeField]
    bool patrolWaiting;

    // The total time we wait at each node
    [SerializeField]
    float totalWaitTime = 3f;

    // The probability of switching direction
    [SerializeField]
    float switchProbability = 0.2f;

    // The list of all patrol nodes to visit
    [SerializeField]
    List<EnemyWaypoint> patrolPoints;

    [SerializeField]
    public GameObject player;

    // Private variables for base behaviour
    NavMeshAgent navMeshAgent;
    int currentPatrolIndex;
    bool travelling;
    bool waiting;
    bool patrolForward;
    float waitTimer;
    Enemy enemy;

    // Use this for initialization
    void Start()
    {
        enemy = GetComponentInChildren<Enemy>();

        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent is not attached to " + gameObject.name);
        }
        else
        {
            if (patrolPoints != null && patrolPoints.Count >= 2)
            {
                currentPatrolIndex = 0;
                SetDestination();
            }
            else
            {
                Debug.Log("Insufficient patrol points for basic patrolling behaviour");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (travelling && navMeshAgent.remainingDistance <= 1.0f && enemy.alive && !enemy.playerSpotted)        // Check if we're close to the destination
        {
            travelling = false;

            if (patrolWaiting)      // if we're going to wait, then wait
            {
                waiting = true;
                waitTimer = 0f;
            }
            else
            {
                ChangePatrolPoint();
                SetDestination();
            }
        }

        if (waiting && !enemy.playerSpotted && enemy.alive)     // Instead if we're waiting
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= totalWaitTime)
            {
                waiting = false;
                ChangePatrolPoint();
                SetDestination();
            }
        }

        if (enemy.playerSpotted && enemy.alive)     // If the enemy has spotted the player
        {
            ChasePlayer();
        }

        if (!enemy.alive)       // If the enemy is dead
        {
            navMeshAgent.isStopped = true;
        }
    }

    private void SetDestination()
    {
        if (patrolPoints != null)
        {
            Vector3 targetVector = patrolPoints[currentPatrolIndex].transform.position;
            navMeshAgent.SetDestination(targetVector);
            travelling = true;
        }
    }

    /// <summary>
    /// Sets the player's current position as the destination
    /// </summary>
    private void ChasePlayer()
    {
        Vector3 targetVector = Camera.main.transform.position;
        navMeshAgent.SetDestination(targetVector);
    }

    ///<summary>
    ///Selects a new patrol point in the available list, but also with a small probability allows for us to move forward or backwards.
    /// </summary>
    private void ChangePatrolPoint()
    {
        if (UnityEngine.Random.Range(0f, 1f) <= switchProbability)
        {
            patrolForward = !patrolForward;
        }

        if (patrolForward)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
        else
        {
            if (--currentPatrolIndex < 0)
            {
                currentPatrolIndex = patrolPoints.Count - 1;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")     // If the player gets too close to the enemy
        {
            enemy.playerSpotted = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")     // If the player moves too far from the enemy
        {
            enemy.playerSpotted = false;
        }
    }
}

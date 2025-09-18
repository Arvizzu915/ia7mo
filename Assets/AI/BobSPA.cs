using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
public class BobSPA : MonoBehaviour
{
    private int health = 50;

    [SerializeField] private Transform playerTrans;

    private float distanceToPlayer = 0;
    [SerializeField] private float fleeDistance = 4;
    private bool lineOfSight = false;

    private Dictionary<string, float> actionScores;

    public Transform[] patrolPoints = new Transform[4];
    private int patrolIndex = 0;
    [SerializeField] private float distanceCheck = 1;

    private NavMeshAgent agent;

    private void Start()
    {
        actionScores = new()
        {
            {"Flee", 0f },
            {"Chase", 0f },
            {"Patrol", 0f },
        };

        gameObject.TryGetComponent(out agent);
    }

    private void Update()
    {
        //SENSE
        distanceToPlayer = Vector3.Distance(transform.position, playerTrans.position);
        Ray ray = new Ray(transform.position + (Vector3.up * 0.5f), playerTrans.position - transform.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            lineOfSight = hit.collider.gameObject.TryGetComponent<PlayerMovement>(out PlayerMovement player);
        }
        if (Vector3.Distance(patrolPoints[patrolIndex].position, transform.position) < distanceCheck)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        //PLAN
        actionScores["Flee"] = (distanceToPlayer < fleeDistance ? 10 : 0) + (health < (health * .5) ? 5f : 0) * (lineOfSight ? 1: 0);
        actionScores["Chase"] = (distanceCheck >= fleeDistance ? 8f : 0) + (health > (health * .5) ? 5f : 0) * (lineOfSight ? 1 : 0);
        actionScores["Patrol"] = 3f;

        UpdatePrediction();

        //ACT
        string chosenAction = actionScores.Aggregate((l,r) => l.Value > r.Value ? l : r).Key;
        switch (chosenAction)
        {
            case "Flee":
                Flee();
                break;
            case "Chase":
                Chase();
                break;
            case "Patrol":
                Patrol();
                break;

            default:
                break;
        }
    }

    private void Flee()
    {
        Vector3 fleeDir = (transform.position - playerTrans.position).normalized * 2;
        if (NavMesh.SamplePosition(fleeDir, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(fleeDir);
        }
        else
        {
            agent.SetDestination(FindFleeAlternative(fleeDir));
        }
            
    }

    private void Chase()
    {
        agent.SetDestination(predictedPlayerPosition);
    }

    private void Patrol()
    {
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }

    #region FleeAlternative
    public float maxDistFromDirection = 100f;
    public float step = 10f;
    public float fleeLength = 3f;
    private Vector3 FindFleeAlternative(Vector3 fleeDirection)
    {
        float maxDistFromPlayer = 0f;
        Vector3 bestPosition = transform.position;

        for (float angle = -maxDistFromDirection; angle <= maxDistFromDirection; angle += step)
        {
            Vector3 dir = Quaternion.Euler(0, angle, 0) * fleeDirection;
            Vector3 candidate = transform.position + dir * fleeLength;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                float distToPlayer = Vector3.Distance(transform.position, playerTrans.position);
                if (distToPlayer > maxDistFromPlayer)
                {
                    maxDistFromPlayer = distToPlayer;
                    bestPosition = hit.position;
                }
            }
        }
        return bestPosition;
    }
    #endregion

    Vector3 lastPlayerPosition = new();
    Vector3 predictedPlayerPosition = new();

    private void UpdatePrediction()
    {
        Vector3 currentPlayerPosition = playerTrans.position;
        Vector3 moveDirection = (currentPlayerPosition - lastPlayerPosition).normalized;

        float predictionDistance = distanceToPlayer * 0.5f;

        predictedPlayerPosition = currentPlayerPosition + moveDirection * predictionDistance;

        lastPlayerPosition = currentPlayerPosition;
    }
}

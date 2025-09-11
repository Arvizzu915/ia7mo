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
        agent.SetDestination(fleeDir);
    }

    private void Chase()
    {
        agent.SetDestination(playerTrans.position);
    }

    private void Patrol()
    {
        agent.SetDestination(patrolPoints[patrolIndex].position);
    }
}

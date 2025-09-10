using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AISpaV2 : MonoBehaviour
{
    [SerializeField] private Transform playerTrans;
    [SerializeField] private float fleeRange = 3f;
    private NavMeshAgent agent;

    private bool inSight = false;

    public Transform[] patrolPoints = new Transform[3];
    private int patrolIndex = 0;

    private void Start()
    {
        gameObject.TryGetComponent(out agent);
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTrans.position);

        Ray ray = new Ray(transform.position + (Vector3.up * 0.5f), playerTrans.position - transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.TryGetComponent(out PlayerMovement player))
            {
                inSight = true;
            }
        }

        float patrolPointDistance = Vector3.Distance(transform.position, patrolPoints[patrolIndex].position);
        if (!inSight)
        {
            if (patrolPointDistance < 0.5f)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            }
        }

        if (inSight)
        {
            if (distance > fleeRange)
            {
                agent.SetDestination(playerTrans.position);
            }
            else
            {
                Vector3 dir = (transform.position - playerTrans.position).normalized;
                Vector3 fleePos = transform.position + dir * 5f;
                agent.SetDestination(fleePos);
            }
        }
        else
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }
}

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AISpa : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float fleeRange = 3f;
    private NavMeshAgent agent;

    private void Start()
    {
        gameObject.TryGetComponent(out agent);
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < fleeRange)
        {
            Vector3 dir = (transform.position - player.position).normalized;
            Vector3 fleePos = transform.position + dir * 5f;
            agent.SetDestination(fleePos);
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }
}

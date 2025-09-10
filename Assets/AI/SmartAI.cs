using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SmartAI : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    
    private void Start()
    {
        gameObject.TryGetComponent(out agent);
    }

    private void Update()
    {
        agent.SetDestination(player.position);
    }
}

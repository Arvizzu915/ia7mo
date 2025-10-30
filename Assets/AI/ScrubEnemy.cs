using BehaviourTrees;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ScrubEnemy : MonoBehaviour
{
    public BehaviourTree tree;
    private NavMeshAgent agent;
    public List<Transform> patrolPoints = new List<Transform>();

    private void Start()
    {
        TryGetComponent(out agent);
        tree = new BehaviourTree("El Scrub Tree");
        IStrategy patrolStratey = new PatrolStrategy(transform, agent, patrolPoints, 3f);
        tree.AddChild(new Leaf("Patrullando", patrolStratey));
    }
    private void Update()
    {
        tree.Process();
    }
}

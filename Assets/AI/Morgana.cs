using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTrees
{
    public class Morgana : MonoBehaviour
    {
        public BehaviourTree tree;
        public GameObject prize;
        public List<Transform> patrolPoints = new();

        public NavMeshAgent agent;

        private void Awake()
        {
            tree = new("El morgana");

            agent = GetComponent<NavMeshAgent>();

            var foo = new Condition(() => prize.activeSelf);

            Leaf isPrizePresent = new("IsPrizePresent", foo);

            Leaf moveToPrize = new("MoveToPrize", new ActionStrategy(() => agent.SetDestination(prize.transform.position)));

            Sequence findPrize = new("FindPrize");
            findPrize.AddChild(isPrizePresent);
            findPrize.AddChild(moveToPrize);

            Selector baseSelector = new("Base Selector");
            baseSelector.AddChild(findPrize);
            baseSelector.AddChild(new Leaf("Patrol", new PatrolStrategy(transform, agent, patrolPoints, 4f)));

            tree.AddChild(baseSelector);
        }

        private void Update()
        {
            tree.Process();
        }
    }
}


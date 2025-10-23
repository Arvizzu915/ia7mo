using UnityEngine;

namespace BehaviourTrees
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset();
    }
}



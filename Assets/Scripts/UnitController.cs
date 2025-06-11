using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 target)
    {
        if (agent != null)
        {
            agent.SetDestination(target);
        }
    }
}
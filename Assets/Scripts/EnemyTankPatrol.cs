using UnityEngine;
using UnityEngine.AI;

public class EnemyTankPatrol : MonoBehaviour
{
    public Transform[] patrolPoints;
    private NavMeshAgent agent;
    private Unit unit;

    public float patrolPointChangeInterval = 10f; 
    private float patrolTimer = 0f;

    void Start()
    {
        unit = GetComponent<Unit>();
        agent = GetComponent<NavMeshAgent>();

        if (unit == null || agent == null)
        {
            Debug.LogError("Відсутній Unit або NavMeshAgent на танку");
            enabled = false;
            return;
        }

        if (unit.owner == FactoryBuilding.OwnerType.Enemy)
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                GameObject[] points = GameObject.FindGameObjectsWithTag("EnemyPatrolPoint");
                patrolPoints = new Transform[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    patrolPoints[i] = points[i].transform;
                }
            }

            if (patrolPoints.Length > 0)
            {
                GoToRandomPoint();
            }
        }
        else
        {
            
            enabled = false;
        }
    }

    void Update()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        patrolTimer += Time.deltaTime;

        
        if ((!agent.pathPending && agent.remainingDistance < 0.5f) || patrolTimer >= patrolPointChangeInterval)
        {
            GoToRandomPoint();
            patrolTimer = 0f;
        }
    }

    void GoToRandomPoint()
    {
        int randomIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[randomIndex].position);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAIManager : MonoBehaviour
{
    private int lastFactoryIndex = -1;
    public int maxEnemyTanks = 10;  
    public static EnemyAIManager Instance { get; private set; }

    public GameObject factoryPrefab;
    public Transform[] factoryBuildPoints; 

    public float timeBetweenFactoryBuilds = 30f; 
    public float timeBetweenTankBuilds = 10f;    

    public LayerMask obstacleLayers; 

    private List<FactoryBuilding> enemyFactories = new List<FactoryBuilding>();
    private List<GameObject> enemyTanks = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(FactoryBuildingRoutine());
        StartCoroutine(TankBuildingRoutine());
    }

    IEnumerator FactoryBuildingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenFactoryBuilds);

            BuildFactory();
        }
    }

    IEnumerator TankBuildingRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenTankBuilds);

            BuildTankFromFactories();
        }
    }

    void BuildFactory()
    {
        foreach (var point in factoryBuildPoints)
        {
            if (IsPointOccupied(point.position))
                continue; 

            bool alreadyHasFactory = false;
            foreach (var factory in enemyFactories)
            {
                if (Vector3.Distance(factory.transform.position, point.position) < 5f)
                {
                    alreadyHasFactory = true;
                    break;
                }
            }
            if (alreadyHasFactory)
                continue;

            
            GameObject newFactoryGO = Instantiate(factoryPrefab, point.position, Quaternion.identity);
            FactoryBuilding newFactory = newFactoryGO.GetComponent<FactoryBuilding>();
            if (newFactory != null)
            {
                newFactory.owner = FactoryBuilding.OwnerType.Enemy;
                enemyFactories.Add(newFactory);
            }
            break; 
        }
    }
    public void UnregisterEnemyTank(GameObject tank)
    {
        if (enemyTanks.Contains(tank))
        {
            enemyTanks.Remove(tank);
        }
    }
    void BuildTankFromFactories()
    {
        enemyTanks.RemoveAll(tank => tank == null);

        if (enemyTanks.Count >= 7)
            return; 

        if (enemyFactories.Count == 0)
            return;

        int factoriesCount = enemyFactories.Count;
        for (int i = 1; i <= factoriesCount; i++)
        {
            int index = (lastFactoryIndex + i) % factoriesCount;
            var factory = enemyFactories[index];

            if (!IsFactoryBusy(factory))
            {
                factory.StartBuildingTank();
                lastFactoryIndex = index;
                break;
            }
        }
    }

    bool IsFactoryBusy(FactoryBuilding factory)
    {
        return factory.isBuilding;
    }

    bool IsPointOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 2f, obstacleLayers);
        foreach (var collider in colliders)
        {
            FactoryBuilding factory = collider.GetComponent<FactoryBuilding>();
            if (factory != null)
            {
                
                if (factory.owner == FactoryBuilding.OwnerType.Enemy || factory.owner == FactoryBuilding.OwnerType.Player)
                    return true;
            }
        }
        return false;
    }


    public void RegisterEnemyTank(GameObject tank)
    {
        if (!enemyTanks.Contains(tank))
        {
            enemyTanks.Add(tank);
        }
    }
}
